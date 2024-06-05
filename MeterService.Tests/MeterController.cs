using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MeterService.Controllers;
using MeterService.Data;
using MeterService.Models;

namespace MeterService.Tests
{
    public class MeterControllerTests
    {
        private readonly MeterController _controller;
        private readonly MeterContext _context;

        public MeterControllerTests()
        {
            var options = new DbContextOptionsBuilder<MeterContext>()
                .UseInMemoryDatabase(databaseName: "MeterDatabase")
                .Options;
            _context = new MeterContext(options);
            _controller = new MeterController(_context);
        }

        [Fact]
        public async Task CreateMeterData_ShouldReturnCreatedResult()
        {
            var meterData = new MeterData
            {
                MeterSerialNumber = "12345678",
                MeasurementTime = DateTime.UtcNow,
                LastIndex = 100,
                Voltage = 220,
                Current = 10
            };

            var result = await _controller.CreateMeterData(meterData);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<MeterData>(createdResult.Value);
            Assert.Equal(meterData.MeterSerialNumber, returnValue.MeterSerialNumber);
        }
    }
}
  