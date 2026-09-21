namespace AutomaticCollegeTimetableGenerator.Models
{
    public class TimetableHistory
    {
        public int Id { get; set; }

        public string? ClassName { get; set; }

        public string? Department { get; set; }

        public string? Action { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}