using Microsoft.AspNetCore.Mvc;
using AutomaticCollegeTimetableGenerator.Data;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Open Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Check Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("Admin", "LoggedIn");
                    return
                RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }
        [ResponseCache(Duration =0, Location = ResponseCacheLocation.None , NoStore = true)]
        public IActionResult Dashboard()
        {
            if
        (HttpContext.Session.GetString("Admin") == null)
            {
                return
                    RedirectToAction("Login");
            }
            ViewBag.TeacherCount = _context.Teachers.Count();
            ViewBag.SubjectCount = _context.Subjects.Count();
            ViewBag.DepartmentCount = _context.Departments.Count();
            ViewBag.ClassroomCount = _context.Classrooms.Count();
            ViewBag.ClassCount = _context.Classes.Count();
            ViewBag.TimetableCount = _context.Timetables.Count();

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}