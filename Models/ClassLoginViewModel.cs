using System.Collections.Generic;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class ClassLoginViewModel
    {
        public Student Student { get; set; } = new Student();

        public List<Class> Classes { get; set; } = new List<Class>();
    }
}