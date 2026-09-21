using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department Name is required.")]
        [RegularExpression(@"^[A-Za-z\s.]+$",
            ErrorMessage = "Department Name must contain letters only.")]
        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s.,()-]+$",
            ErrorMessage = "Description contains invalid characters.")]
        public string? Description { get; set; }
    }
}