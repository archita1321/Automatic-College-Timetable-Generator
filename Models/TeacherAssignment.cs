using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class TeacherAssignment
    {
        public int Id { get; set; }

        // Which teacher this assignment belongs to
        [Required]
        public int TeacherId { get; set; }

        // Class
        [Required(ErrorMessage = "Class is required.")]
        public string? ClassName { get; set; }

        // Semester
        
        [Required(ErrorMessage = "Semester is required.")]
        [Range(1, 12, ErrorMessage = "Semester must be between 1 and 12.")]
        public int Semester { get; set; }

        // Subject
        [Required(ErrorMessage = "Subject is required.")]
        public string? Subject { get; set; }

        // Number of lectures per week
        [Required(ErrorMessage = "Lecture count is required.")]
        public int LectureCount { get; set; }

        // Duration of each lecture
        [Required(ErrorMessage = "Duration is required.")]
        public string? Duration { get; set; }

        [Required(ErrorMessage = "Subject type is required.")]
        public string SubjectType { get; set; } = "Theory";

        // Relationship with Teacher
        public Teacher? Teacher { get; set; }
    }

}