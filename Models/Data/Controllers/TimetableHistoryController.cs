using Microsoft.AspNetCore.Mvc;
using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class TimetableHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimetableHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var history = _context.TimetableHistories
                                 .OrderByDescending(h => h.CreatedDate)
                                 .ToList();

            return View(history);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(TimetableHistory history)
        {
            if (ModelState.IsValid)
            {
                history.CreatedDate = DateTime.Now;

                _context.TimetableHistories.Add(history);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(history);
        }
    }
}