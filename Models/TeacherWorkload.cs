namespace AutomaticCollegeTimetableGenerator.Models
{
    public class TeacherWorkload
    {
        public string TeacherName { get; set; } = string.Empty;

        public int TotalLectures { get; set; }

        public string Subjects { get; set; } = string.Empty;

        public int WorkingDays { get; set; }

        public string Classes { get; set; } = string.Empty;
    }
}