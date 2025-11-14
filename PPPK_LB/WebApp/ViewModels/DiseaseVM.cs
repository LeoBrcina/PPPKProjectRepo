using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class DiseaseVM
    {
        public int DiseaseId { get; set; }

        [Required]
        [StringLength(100)]
        public string DiseaseName { get; set; } = string.Empty;
    }
}
