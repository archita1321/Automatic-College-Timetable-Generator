using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class SemesterSelectionViewModel
    {
        [Required(ErrorMessage = "Please select a semester.")]
        public int Semester { get; set; }
    }
}