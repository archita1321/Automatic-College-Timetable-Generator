using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var departments = _context.Departments.ToList();
            return View(departments);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(department);
        }


        // Display Edit Page
        public IActionResult Edit(int id)
        {
            var department = _context.Departments.Find(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }


        // Save Edited Department
        [HttpPost]
        public IActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Update(department);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(department);
        }


        // Delete Department
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Find(id);

            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        // Department Information Pages

        public IActionResult ComputerScience()
        {
            return View();
        }

        public IActionResult InformationTechnology()
        {
            return View();
        }

        public IActionResult Commerce()
        {
            return View();
        }

        public IActionResult HotelManagement()
        {
            return View();
        }
    }
}