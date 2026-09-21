namespace AutomaticCollegeTimetableGenerator.Models
{
    public class TeacherLeave
    {
        public int Id { get; set; }

        public string? TeacherName { get; set; }

        public DateTime LeaveDate { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }
    }
}