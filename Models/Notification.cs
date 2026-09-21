namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? Status { get; set; }
    }
}