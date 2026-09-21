using Microsoft.AspNetCore.Mvc;
using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class TeacherLeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherLeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // LEAVE LIST
        // =====================================================

        public IActionResult Index()
        {
            var leaves = _context.TeacherLeaves
                .OrderByDescending(x => x.LeaveDate)
                .ToList();

            return View(leaves);
        }

        // =====================================================
        // APPLY LEAVE
        // =====================================================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TeacherLeave leave)
        {
            if (ModelState.IsValid)
            {
                leave.Status = "Pending";

                _context.TeacherLeaves.Add(leave);
                _context.SaveChanges();

                TempData["Success"] = "Teacher leave submitted successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(leave);
        }
    }
}