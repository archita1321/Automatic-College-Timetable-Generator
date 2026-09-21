using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Class
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s.]+$",
            ErrorMessage = "Class Name can contain letters, numbers and spaces only.")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        [RegularExpression(@"^[A-Za-z\s.]+$",
            ErrorMessage = "Department must contain text only.")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Semester is required.")]
        [Range(1, 12, ErrorMessage = "Semester must be between 1 and 12.")]
        public int Semester { get; set; }
    }
}