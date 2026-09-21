using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Classroom
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room Number is required.")]
        [RegularExpression(@"^[0-9A-Za-z-]+$",
            ErrorMessage = "Room Number can contain letters, numbers and hyphen only.")]
        public string? RoomNumber { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 500, ErrorMessage = "Capacity must be between 1 and 500.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Room Type is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$",
            ErrorMessage = "Room Type must contain letters only.")]
        public string? RoomType { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [RegularExpression(@"^[A-Za-z\s.]+$",
            ErrorMessage = "Department must contain text only.")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public string? ClassName { get; set; }
    }
}