using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class MedicineVM
    {
        public int MedicineId { get; set; }

        [Required]
        [StringLength(100)]
        public string MedicineName { get; set; } = string.Empty;
    }
}
