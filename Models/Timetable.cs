using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Timetable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s.]+$",
            ErrorMessage = "Class Name can contain letters, numbers and spaces only.")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [RegularExpression(@"^[A-Za-z\s.]+$",
            ErrorMessage = "Department must contain text only.")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Semester is required.")]
        [Range(1, 12, ErrorMessage = "Semester must be between 1 and 12.")]
        public int? Semester { get; set; }

        [Required(ErrorMessage = "Day is required.")]
        [RegularExpression(@"^[A-Za-z]+$",
            ErrorMessage = "Day must contain letters only.")]
        public string? Day { get; set; }

        [Required(ErrorMessage = "Time Slot is required.")]
        public string? TimeSlot { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s.]+$",
            ErrorMessage = "Subject can contain letters, numbers and spaces only.")]
        public string? Subject { get; set; }

        // Theory / Practical / Project / Theory + Practical
        [Required(ErrorMessage = "Subject Type is required.")]
        public string SubjectType { get; set; } = "Theory";

        [Required(ErrorMessage = "Teacher is required.")]
        [RegularExpression(@"^[A-Za-z\s.]+$",
            ErrorMessage = "Teacher name must contain letters only.")]
        public string? Teacher { get; set; }

        [Required(ErrorMessage = "Classroom is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s-]+$",
            ErrorMessage = "Classroom can contain letters, numbers and hyphen only.")]
        public string? Classroom { get; set; }

        [Required(ErrorMessage = "Academic Year is required.")]
        [RegularExpression(@"^\d{4}-\d{2}$",
            ErrorMessage = "Academic Year must be in format YYYY-YY, e.g. 2026-27.")]
        public string? AcademicYear { get; set; }
    }
}