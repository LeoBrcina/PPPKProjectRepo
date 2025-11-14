using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class PrescriptionVM
    {
        [Required(ErrorMessage = "Prescription ID is required.")]
        public int PrescriptionId { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientId { get; set; }
        public string PatientName { get; set; }

        [Required(ErrorMessage = "At least one medicine must be selected.")]
        public List<int> MedicineIds { get; set; } = new List<int>();

        public List<string> MedicineNames { get; set; } = new List<string>();
    }
}
