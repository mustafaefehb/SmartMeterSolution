using Microsoft.EntityFrameworkCore;
using MeterService.Models;

namespace MeterService.Data
{
    public class MeterContext : DbContext
    {
        public MeterContext(DbContextOptions<MeterContext> options) : base(options) { }

        public DbSet<MeterData> MeterData { get; set; }
    }
}
