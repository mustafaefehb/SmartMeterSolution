using System;
using System.ComponentModel.DataAnnotations;

namespace WebInterface.Models
{
    public class Report
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Meter serial number must be 8 characters long.")]
        public string MeterSerialNumber { get; set; }

        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }
}