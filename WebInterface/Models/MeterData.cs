using System;
using System.ComponentModel.DataAnnotations;

namespace WebInterface.Models
{
    public class MeterData
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Meter serial number must be 8 characters long.")]
        public string MeterSerialNumber { get; set; }

        [Required]
        public DateTime MeasurementTime { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Last index must be a positive number.")]
        public decimal LastIndex { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Voltage must be a positive number.")]
        public decimal Voltage { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Current must be a positive number.")]
        public decimal Current { get; set; }
    }
}