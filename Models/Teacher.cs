using System.ComponentModel.DataAnnotations;

namespace AutomaticCollegeTimetableGenerator.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Teacher Name is required.")]
        [RegularExpression(
            @"^[A-Za-z\s]+$",
            ErrorMessage = "Teacher Name must contain letters and spaces only.")]
        public string? TeacherName { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [RegularExpression(
            @"^[A-Za-z\s.]+$",
            ErrorMessage = "Department must contain text only.")]
        public string? Department { get; set; }

        // Kept because this column already exists in the database.
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(
            @"^[0-9]{10}$",
            ErrorMessage = "Phone number must contain exactly 10 digits.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(
            8,
            MinimumLength = 4,
            ErrorMessage = "Password must be between 4 and 8 characters.")]
        public string? Password { get; set; }
    }
}