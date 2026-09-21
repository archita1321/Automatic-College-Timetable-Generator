using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Subject Name is required.")]
        public string SubjectName { get; set; } = "";

        [Required(ErrorMessage = "Class Name is required.")]
        public string ClassName { get; set; } = "";

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; } = "";

        [Required(ErrorMessage = "Semester is required.")]
        [Range(1, 12)]
        public int Semester { get; set; }

        [Required(ErrorMessage = "Teacher is required.")]
        public string Teacher { get; set; } = "";

        [Required(ErrorMessage = "Lecture count is required.")]
        [Range(1, 20)]
        public int LectureCount { get; set; } = 1;

        [Required(ErrorMessage = "Duration is required.")]
        public string Duration { get; set; } = "1 Hour";

        [Required(ErrorMessage = "Subject Type is required.")]
        public string SubjectType { get; set; } = "Theory";
    }
}