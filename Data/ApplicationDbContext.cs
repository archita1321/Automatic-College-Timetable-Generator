using Microsoft.EntityFrameworkCore;
using AutomaticCollegeTimetableGenerator.Models;

namespace AutomaticCollegeTimetableGenerator.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherLeave> TeacherLeaves { get; set; }
        public DbSet<SubstituteTeacher> SubstituteTeachers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TimetableHistory> TimetableHistories { get; set; }
        public DbSet<Department> Departments { get; set;}
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<Class> Classes { get; set; }
        




    }
}