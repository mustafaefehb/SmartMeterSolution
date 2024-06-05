using Microsoft.Extensions.Configuration;
using Shared.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebInterface.Models;

namespace WebInterface.Services
{
    public class ReportServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _reportServiceUrl;

        public ReportServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _reportServiceUrl = configuration["Services:ReportService"];
        }

        public async Task<PagedResult<Report>> GetReportsAsync(int pageNumber, int pageSize)
        {
            var d = $"{_reportServiceUrl}/api/reports?pageNumber={pageNumber}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync($"{_reportServiceUrl}/api/report?pageNumber={pageNumber}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagedResult<Report>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Report> GetReportByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_reportServiceUrl}/api/report/{id}");
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Report>(responseBody);
        }

        public async Task CreateReportAsync(string meterSerialNumber)
        {
            var requestBody = new { MeterSerialNumber = meterSerialNumber };
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_reportServiceUrl}/api/report", jsonContent);
            response.EnsureSuccessStatusCode();
        }
    }
}