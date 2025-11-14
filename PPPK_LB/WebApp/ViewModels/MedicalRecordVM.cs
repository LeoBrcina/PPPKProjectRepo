using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class MedicalRecordVM : IValidatableObject
    {
        [Required(ErrorMessage = "Record ID is required.")]
        public int RecordId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "At least one disease must be selected.")]
        public List<int> DiseaseIds { get; set; } = new List<int>(); 

        public List<string> DiseaseNames { get; set; } = new List<string>(); 

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please provide a valid date.")]
        public DateOnly StartDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please provide a valid date.")]
        public DateOnly? EndDate { get; set; }

        public PatientVM? Patient { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End Date cannot be earlier than Start Date.",
                    new[] { nameof(EndDate) }
                );
            }
        }
    }
}
