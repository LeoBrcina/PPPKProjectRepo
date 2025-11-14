using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.ViewModels
{
    public class PatientVM
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "OIB is required.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "OIB must be exactly 11 digits.")]
        public string Oib { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please provide a valid date.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(1, ErrorMessage = "Gender must be a single character (e.g., M/F).")]
        public string Gender { get; set; } = string.Empty;
        public List<PrescriptionVM> Prescriptions { get; set; } = new List<PrescriptionVM>();

        public List<AppointmentVM> Appointments { get; set; } = new List<AppointmentVM>();
        public List<MedicalRecordVM> MedicalRecords { get; set; } = new List<MedicalRecordVM>();
    }
}
