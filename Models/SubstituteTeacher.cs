namespace AutomaticCollegeTimetableGenerator.Models
{
    public class SubstituteTeacher
    {
        public int Id { get; set; }

        public string? AbsentTeacher { get; set; }

        public string? SubstituteTeacherName { get; set; }
        public DateTime Date { get; set; }

        public string? Day { get; set; }

        public string? TimeSlot { get; set; }

        public string? Subject { get; set; }

        public string? ClassName { get; set; }

        public string? Status { get; set; }
    }
}