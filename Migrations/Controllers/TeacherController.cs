using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // MANAGE TEACHERS
        // =========================================================

        public IActionResult Index()
        {
            var teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            // IMPORTANT:
            // Manage Teachers displays subjects directly
            // from the Subjects table.
            //
            // Subjects is the master source.
            // Therefore Edit/Delete/Delete All from Manage Subjects
            // automatically reflects here.

            ViewBag.TeacherSubjects = _context.Subjects
                .OrderBy(s => s.Teacher)
                .ThenBy(s => s.ClassName)
                .ThenBy(s => s.Semester)
                .ThenBy(s => s.SubjectName)
                .ToList();

            return View(teachers);
        }

        // =========================================================
        // ADD TEACHER
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(teacher);
            }

            // Existing database has a required Subject column.
            // It is no longer used for teacher information.
            teacher.Subject = "Multiple Subjects";

            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Teacher added successfully.";

            return RedirectToAction(nameof(Index));
        }
        // =========================================================
        // EDIT TEACHER
        // =========================================================

        // =========================================================
        // EDIT TEACHER
        // =========================================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var teacher = _context.Teachers
                .Find(id);

            if (teacher == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments
                .ToList();

            return View(teacher);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments
                    .ToList();

                return View(teacher);
            }

            var existingTeacher = _context.Teachers
                .Find(teacher.Id);

            if (existingTeacher == null)
            {
                return NotFound();
            }

            existingTeacher.TeacherName = teacher.TeacherName;
            existingTeacher.Department = teacher.Department;
            existingTeacher.Email = teacher.Email;
            existingTeacher.Phone = teacher.Phone;
            existingTeacher.Username = teacher.Username;
            existingTeacher.Password = teacher.Password;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // DELETE TEACHER
        // =========================================================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var teacher = _context.Teachers
                .Find(id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Delete this teacher's teaching assignments first
            var assignments = _context.TeacherAssignments
                .Where(a => a.TeacherId == id)
                .ToList();

            if (assignments.Any())
            {
                _context.TeacherAssignments.RemoveRange(assignments);
            }

            _context.Teachers.Remove(teacher);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // ADD TEACHING DETAILS PAGE
        // =========================================================

        [HttpGet]
        public IActionResult AddTeachingDetails(int teacherId)
        {
            var teacher = _context.Teachers
                .Find(teacherId);

            if (teacher == null)
            {
                return NotFound();
            }

            var viewModel = new TeacherTeachingDetailsViewModel
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.TeacherName
            };

            // IMPORTANT:
            // Send actual Classes to the view.
            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            // Send all Subjects to the view.
            ViewBag.Subjects = _context.Subjects
                .OrderBy(s => s.ClassName)
                .ThenBy(s => s.Semester)
                .ThenBy(s => s.SubjectName)
                .ToList();

            return View(viewModel);
        }


        // =========================================================
        // SAVE ALL TEACHING DETAILS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTeachingDetails(
            TeacherTeachingDetailsViewModel viewModel)
        {
            var teacher = _context.Teachers
                .Find(viewModel.TeacherId);

            if (teacher == null)
            {
                return NotFound();
            }

            if (viewModel.Assignments == null ||
                !viewModel.Assignments.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Please add at least one subject."
                );
            }

            if (ModelState.IsValid)
            {
                foreach (var assignment in viewModel.Assignments)
                {
                    // Always connect assignment to
                    // the existing teacher.
                    assignment.TeacherId = teacher.Id;

                    // Prevent same teacher from getting
                    // the same subject twice for the
                    // same class and semester.
                    bool alreadyExists =
                        _context.TeacherAssignments.Any(a =>
                            a.TeacherId == teacher.Id &&
                            a.ClassName == assignment.ClassName &&
                            a.Semester == assignment.Semester &&
                            a.Subject == assignment.Subject
                        );

                    if (!alreadyExists)
                    {
                        _context.TeacherAssignments.Add(assignment);
                    }
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] =
                    $"{teacher.TeacherName}'s teaching details saved successfully.";

                return RedirectToAction(nameof(Index));
            }


            // If validation fails, reload dropdown data.
            viewModel.TeacherName = teacher.TeacherName;

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            ViewBag.Subjects = _context.Subjects
                .OrderBy(s => s.ClassName)
                .ThenBy(s => s.Semester)
                .ThenBy(s => s.SubjectName)
                .ToList();

            return View(viewModel);
        }


        // =========================================================
        // TEACHER LOGIN
        // =========================================================

        // =========================================================
        // TEACHER LOGIN
        // =========================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            username = username?.Trim() ?? "";
            password = password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter username and password.";
                return View();
            }

            var teacher = _context.Teachers
                .AsEnumerable()
                .FirstOrDefault(t =>
                    !string.IsNullOrWhiteSpace(t.Username) &&
                    t.Username.Trim().Equals(
                        username,
                        StringComparison.OrdinalIgnoreCase) &&

                    !string.IsNullOrWhiteSpace(t.Password) &&
                    t.Password.Trim() == password);

            if (teacher != null)
            {
                HttpContext.Session.SetString(
                    "TeacherName",
                    teacher.TeacherName ?? "");

                HttpContext.Session.SetString(
                    "TeacherId",
                    teacher.Id.ToString());

                return RedirectToAction(nameof(MyTimetable));
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }
        public IActionResult MyTimetable()
        {
            var teacherIdString = HttpContext.Session.GetString("TeacherId");

            if (string.IsNullOrEmpty(teacherIdString))
            {
                return RedirectToAction(nameof(Login));
            }

            if (!int.TryParse(teacherIdString, out int teacherId))
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            var teacher = _context.Teachers
                .FirstOrDefault(t => t.Id == teacherId);

            if (teacher == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            var timetable = _context.Timetables
                .Where(t => t.Teacher == teacher.TeacherName)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.TimeSlot)
                .ToList();

            ViewBag.TeacherName = teacher.TeacherName;
            ViewBag.Teacher = teacher;

            return View("~/Views/Timetable/MyTimetable.cshtml", timetable);
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("TeacherName");

            return RedirectToAction(nameof(Login));
        }
    }
}