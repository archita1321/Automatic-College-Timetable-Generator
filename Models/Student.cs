using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string? StudentName { get; set; }

        public string? RollNumber { get; set; }

        public string? ClassName { get; set; }

        public string? Department { get; set; }

        public int Semester { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }
    }
}