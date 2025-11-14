using System;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class AppointmentVM
    {
        [Required(ErrorMessage = "Appointment ID is required.")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Appointment Date is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "Please provide a valid date and time.")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }
        public string AppointmentTypeName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;

    }
}
