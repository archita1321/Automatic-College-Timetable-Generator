using Microsoft.AspNetCore.Mvc;
using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var notifications = _context.Notifications
                                        .OrderByDescending(n => n.CreatedDate)
                                        .ToList();

            return View(notifications);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.CreatedDate = DateTime.Now;
                notification.Status = "New";

                _context.Notifications.Add(notification);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(notification);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == id);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();

                TempData["Success"] = "Notification deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}