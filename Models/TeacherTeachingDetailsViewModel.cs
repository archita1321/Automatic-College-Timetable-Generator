using System.Collections.Generic;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class TeacherTeachingDetailsViewModel
    {
        public int TeacherId { get; set; }

        public string? TeacherName { get; set; }

        public List<TeacherAssignment> Assignments { get; set; }
            = new List<TeacherAssignment>();
    }
}