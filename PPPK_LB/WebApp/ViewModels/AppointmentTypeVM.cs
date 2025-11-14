using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class AppointmentTypeVM
    {
        [Required(ErrorMessage = "Appointment Type ID is required.")]
        public int AppointmentTypeId { get; set; }

        [Required(ErrorMessage = "Appointment Type Name is required.")]
        [StringLength(100, ErrorMessage = "Appointment Type Name cannot exceed 100 characters.")]
        public string AppointmentTypeName { get; set; } = string.Empty;
    }
}
