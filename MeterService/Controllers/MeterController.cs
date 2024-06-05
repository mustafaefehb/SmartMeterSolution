using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MeterService.Data;
using MeterService.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Shared.Models;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace MeterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeterController : ControllerBase, IDisposable
    {
        private readonly MeterContext _context;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MeterController(MeterContext context)
        {
            _context = context;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "meterQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeterData([FromBody] MeterData meterData)
        {
            if (meterData == null)
            {
                return BadRequest();
            }

            meterData.Id = Guid.NewGuid();
            _context.MeterData.Add(meterData);
            await _context.SaveChangesAsync();

            var message = JsonSerializer.Serialize(meterData);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: "meterQueue",
                                  basicProperties: null,
                                  body: body);

            return CreatedAtAction(nameof(GetMeterData), new { id = meterData.Id }, meterData);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeterData(Guid id)
        {
            var meterData = await _context.MeterData.FindAsync(id);
            if (meterData == null)
            {
                return NotFound();
            }
            return Ok(meterData);
        }

        [HttpGet("serial/{serialNumber}")]
        public async Task<IActionResult> GetLatestMeterDataBySerial(string serialNumber)
        {
            var meterData = await _context.MeterData
                .FirstOrDefaultAsync(m => m.MeterSerialNumber == serialNumber);

            if (meterData == null)
            {
                return NotFound();
            }

            return Ok(meterData);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMeterData(int pageNumber = 1, int pageSize = 10, string meterSerialNumber = null)
{
            var query = _context.MeterData.AsQueryable();

            if (!string.IsNullOrEmpty(meterSerialNumber))
            {
                query = query.Where(m => m.MeterSerialNumber.Contains(meterSerialNumber));
            }

            var meterData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.MeasurementTime)
                .ToListAsync();

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var response = new PagedResult<MeterData>
            {
                Data = meterData,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            return Ok(response);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _context?.Dispose();
        }
    }
}
