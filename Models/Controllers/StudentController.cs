using Microsoft.AspNetCore.Mvc;
using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // MANAGE CLASS LOGINS
        // =====================================================

        public IActionResult Index()
        {
            var students = _context.Students
                .OrderBy(s => s.ClassName)
                .ToList();

            return View(students);
        }


        // =====================================================
        // STUDENT LOGIN
        // =====================================================

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

            var student = _context.Students
                .FirstOrDefault(s =>
                    s.Username == username &&
                    s.Password == password);

            if (student == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            HttpContext.Session.SetString(
                "StudentClass",
                student.ClassName ?? "");

            HttpContext.Session.SetString(
    "StudentDepartment",
    student.Department ?? "");

            return RedirectToAction(nameof(SelectSemester));
        }
        // =====================================================
        // SELECT SEMESTER
        // =====================================================

        [HttpGet]
        public IActionResult SelectSemester()
        {
            var className =
                HttpContext.Session.GetString("StudentClass");

            if (string.IsNullOrWhiteSpace(className))
            {
                return RedirectToAction(nameof(Login));
            }

            var semesters = _context.Classes
                .Where(c => c.ClassName == className)
                .Select(c => c.Semester)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Semesters = semesters;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectSemester(int semester)
        {
            var className =
                HttpContext.Session.GetString("StudentClass");

            if (string.IsNullOrWhiteSpace(className))
            {
                return RedirectToAction(nameof(Login));
            }

            var validSemester = _context.Classes
                .Any(c =>
                    c.ClassName == className &&
                    c.Semester == semester);

            if (!validSemester)
            {
                TempData["ErrorMessage"] =
                    "Invalid semester selected.";

                return RedirectToAction(nameof(SelectSemester));
            }

            HttpContext.Session.SetString(
                "StudentSelectedSemester",
                semester.ToString());

            return RedirectToAction(nameof(Timetable));
        }


        // =====================================================
        // CLASS TIMETABLE
        // =====================================================

        public IActionResult Timetable()
        {
            string? className =
                HttpContext.Session.GetString("StudentClass");

            string? department =
                HttpContext.Session.GetString("StudentDepartment");

            string? semesterString =
    HttpContext.Session.GetString("StudentSelectedSemester");

            if (string.IsNullOrWhiteSpace(className))
            {
                return RedirectToAction(nameof(Login));
            }
            if (string.IsNullOrWhiteSpace(semesterString))
            {
                return RedirectToAction(nameof(SelectSemester));
            }

            int semester = int.Parse(semesterString);

            var timetable = _context.Timetables
    .Where(t =>
        t.ClassName == className &&
        t.Department == department &&
        t.Semester == semester)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.TimeSlot)
                .ToList();

            ViewBag.ClassName = className;
            ViewBag.Department = department;
            ViewBag.Semester = semester;

            return View(timetable);
        }


        // =====================================================
        // CREATE CLASS LOGIN - GET
        // =====================================================

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ClassLoginViewModel();

            model.Student = new Student();

            model.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            return View(model);
        }


        // =====================================================
        // CREATE CLASS LOGIN - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClassLoginViewModel model)
        {
            // Safety check
            if (model == null)
            {
                model = new ClassLoginViewModel();
            }

            // Safety check
            if (model.Student == null)
            {
                model.Student = new Student();
            }

            // ALWAYS load classes
            model.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();


            // =================================================
            // VALIDATE USERNAME
            // =================================================

            if (string.IsNullOrWhiteSpace(model.Student.Username))
            {
                ModelState.AddModelError(
                    "Student.Username",
                    "Username is required.");
            }


            // =================================================
            // VALIDATE PASSWORD
            // =================================================

            if (string.IsNullOrWhiteSpace(model.Student.Password))
            {
                ModelState.AddModelError(
                    "Student.Password",
                    "Password is required.");
            }


            // =================================================
            // VALIDATE CLASS
            // =================================================

            if (string.IsNullOrWhiteSpace(model.Student.ClassName))
            {
                ModelState.AddModelError(
                    "Student.ClassName",
                    "Please select a class.");
            }


            // =================================================
            // FIND SELECTED CLASS
            // =================================================

            Class? selectedClass = null;

            if (!string.IsNullOrWhiteSpace(model.Student.ClassName))
            {
                selectedClass = model.Classes
     .FirstOrDefault(c =>
         c.ClassName == model.Student.ClassName);
            }


            if (selectedClass == null &&
                !string.IsNullOrWhiteSpace(model.Student.ClassName))
            {
                ModelState.AddModelError(
                    "Student.ClassName",
                    "Selected class was not found in Manage Classes.");
            }


            // =================================================
            // CHECK DUPLICATE USERNAME
            // =================================================

            bool usernameExists = _context.Students
    .Any(s => s.Username == model.Student.Username);


            if (usernameExists)
            {
                ModelState.AddModelError(
                    "Student.Username",
                    "This username already exists.");
            }


            // =================================================
            // IF ERROR
            // =================================================

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // =================================================
            // AUTOMATICALLY SET DEPARTMENT
            // =================================================

            model.Student.Department =
                selectedClass!.Department;


            // =================================================
            // SAVE
            // =================================================

            _context.Students.Add(model.Student);

            _context.SaveChanges();


            TempData["SuccessMessage"] =
                "Class login created successfully.";


            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // LOGOUT
        // =====================================================

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("StudentClass");

            HttpContext.Session.Remove("StudentDepartment");

            return RedirectToAction(nameof(Login));
        }
    }
}