using System;
using System.ComponentModel.DataAnnotations;

namespace WebInterface.Models
{
    public class CreateReportViewModel
    {
        [Required(ErrorMessage = "Meter serial number is required.")]
        [Display(Name = "Meter Serial Number")]
        public string MeterSerialNumber { get; set; }
    }
}