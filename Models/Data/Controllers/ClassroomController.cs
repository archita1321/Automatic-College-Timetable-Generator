using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class ClassroomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassroomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public IActionResult Index()
        {
            var classrooms = _context.Classrooms.ToList();

            return View(classrooms);
        }


        // =========================
        // CREATE - GET
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.ToList();

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            return View();
        }


        // =========================
        // CREATE - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Classroom classroom)
        {
            if (ModelState.IsValid)
            {
                _context.Classrooms.Add(classroom);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.ToList();

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            return View(classroom);
        }


        // =========================
        // EDIT - GET
        // =========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var classroom = _context.Classrooms.Find(id);

            if (classroom == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments.ToList();

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            return View(classroom);
        }


        // =========================
        // EDIT - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Classroom classroom)
        {
            if (ModelState.IsValid)
            {
                _context.Classrooms.Update(classroom);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.ToList();

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();

            return View(classroom);
        }


        // =========================
        // DELETE - GET
        // =========================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var classroom = _context.Classrooms.Find(id);

            if (classroom == null)
            {
                return NotFound();
            }

            return View(classroom);
        }


        // =========================
        // DELETE - POST
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var classroom = _context.Classrooms.Find(id);

            if (classroom != null)
            {
                _context.Classrooms.Remove(classroom);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}