using MeterService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportService.Data;
using ReportService.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReportService.Services
{
    public class ReportProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQService _rabbitMQService;

        public ReportProcessor(IServiceProvider serviceProvider, RabbitMQService rabbitMQService)
        {
            _serviceProvider = serviceProvider;
            _rabbitMQService = rabbitMQService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQService.ReceiveMessages<CreateReportRequest>(async request =>
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ReportContext>();
                var meterContext = scope.ServiceProvider.GetRequiredService<MeterContext>();

                var reportId = Guid.NewGuid();

                // Create initial report record with "inprogress" status
                var report = new Report
                {
                    Id = reportId,
                    MeterSerialNumber = request.MeterSerialNumber,
                    RequestDate = DateTime.Now,
                    Status = "InProgress",
                    Content = string.Empty,
                    Url = string.Empty
                };

                context.Reports.Add(report);
                await context.SaveChangesAsync();

                // Query the latest meter data for the given serial number
                var meterData = await meterContext.MeterData
                    .Where(m => m.MeterSerialNumber == request.MeterSerialNumber)
                    .OrderByDescending(m => m.MeasurementTime)
                    .ToListAsync();

                if (meterData == null || !meterData.Any())
                {
                    // Handle case where no data is found
                    report.Status = "Failed";
                    await context.SaveChangesAsync();
                    return;
                }

                // Create the SharedReports directory if it doesn't exist
                var sharedReportsDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "SharedReports");
                if (!Directory.Exists(sharedReportsDirectory))
                {
                    Directory.CreateDirectory(sharedReportsDirectory);
                }

                // Create a CSV file from the meter data
                var csv = new StringBuilder();
                csv.AppendLine("MeterSerialNumber,MeasurementTime,LastIndex,Voltage,Current");

                foreach (var data in meterData)
                {
                    csv.AppendLine($"{data.MeterSerialNumber},{data.MeasurementTime},{data.LastIndex},{data.Voltage},{data.Current}");
                }

                var fileName = $"report_{reportId}.csv";
                var filePath = Path.Combine(sharedReportsDirectory, fileName);

                await System.IO.File.WriteAllTextAsync(filePath, csv.ToString());

                // Generate the URL for the file pointing to the DownloadReport method
                var fileUrl = $"/api/report/download/{reportId}";

                // Update report status to "generated" and set file path and URL
                report.Status = "Generated";
                report.Content = filePath;
                report.Url = fileUrl;

                context.Reports.Update(report);
                await context.SaveChangesAsync();
            });

            return Task.CompletedTask;
        }
    }
}
