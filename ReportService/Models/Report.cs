using System;

namespace ReportService.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public string MeterSerialNumber { get; set; }
        public string Url { get; set; }
    }
}
