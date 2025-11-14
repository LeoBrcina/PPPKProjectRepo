using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class AttachmentVM
    {
        [Required(ErrorMessage = "Attachment ID is required.")]
        public int AttachmentId { get; set; }

        [Required(ErrorMessage = "Appointment ID is required.")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "File Name is required.")]
        [StringLength(255, ErrorMessage = "File Name cannot exceed 255 characters.")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "File Path is required.")]
        [StringLength(500, ErrorMessage = "File Path cannot exceed 500 characters.")]
        public string FilePath { get; set; } = string.Empty;

        [DataType(DataType.DateTime, ErrorMessage = "Please provide a valid date and time.")]
        public DateTime? UploadedAt { get; set; } 

    }
}
