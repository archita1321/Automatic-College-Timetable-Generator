using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class BulkSubjectViewModel
    {
        [Required(ErrorMessage = "Please select a teacher.")]
        public int TeacherId { get; set; }

        public List<BulkSubjectRow> Subjects { get; set; }
            = new List<BulkSubjectRow>();
    }

    public class BulkSubjectRow
    {
        [Required(ErrorMessage = "Subject name is required.")]
        public string? SubjectName { get; set; }

        [Required(ErrorMessage = "Class is required.")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Semester is required.")]
        [Range(1, 12, ErrorMessage = "Semester must be between 1 and 12.")]
        public int Semester { get; set; }

        // Theory = normally 2 lectures/week
        // Practical/Project = according to practical/project requirement
        [Range(1, 20, ErrorMessage = "Lecture count must be between 1 and 20.")]
        public int LectureCount { get; set; } = 2;

        // Automatically controlled by SubjectType
        public string Duration { get; set; } = "1 Hour";

        [Required(ErrorMessage = "Subject type is required.")]
        public string SubjectType { get; set; } = "Theory";
    }
}