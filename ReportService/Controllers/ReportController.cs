using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeterService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReportService.Data;
using ReportService.Models;
using ReportService.Services;
using Shared.Models;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportContext _context;
        private readonly MeterContext _meterContext;
        private readonly RabbitMQService _rabbitMQService;

        public ReportController(ReportContext context, MeterContext meterContext, RabbitMQService rabbitMQService)
        {
            _context = context;
            _meterContext = meterContext;
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        public IActionResult CreateReport([FromBody] CreateReportRequest request)
        {
            if (string.IsNullOrEmpty(request.MeterSerialNumber))
            {
                return BadRequest("Meter serial number is required.");
            }

            // Send message to queue
            _rabbitMQService.SendMessage(request);

            return Accepted();
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadReport(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            var filePath = report.Content;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "text/csv", Path.GetFileName(filePath));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(Guid id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            return Ok(report);
        }

        [HttpGet]
        public async Task<IActionResult> GetReports(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Reports.AsQueryable();

            var reports = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.RequestDate)
                .ToListAsync();

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var response = new PagedResult<Report>
            {
                Data = reports,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            return Ok(response);
        }
    }
}
