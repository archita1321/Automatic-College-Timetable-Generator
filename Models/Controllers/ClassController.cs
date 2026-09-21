using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;  

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class ClassController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var classes =
                _context.Classes.ToList();
            return View(classes);
        }
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Class classObj)
        {
            if (ModelState.IsValid)
            {
                _context.Classes.Add(classObj);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.ToList();

            return View(classObj);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classObj = _context.Classes.Find(id);

            if (classObj == null)
            {
                return NotFound();
            }

            ViewBag.Departments = _context.Departments.ToList();

            return View(classObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Class classObj)
        {
            if (ModelState.IsValid)
            {
                _context.Classes.Update(classObj);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.ToList();

            return View(classObj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classObj = _context.Classes.Find(id);

            if (classObj == null)
            {
                return NotFound();
            }

            return View(classObj);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var classObj = _context.Classes.Find(id);

            if (classObj != null)
            {
                _context.Classes.Remove(classObj);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}