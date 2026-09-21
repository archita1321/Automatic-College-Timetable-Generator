using Microsoft.AspNetCore.Mvc;
using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using System;
using System.Linq;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class SubstituteTeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubstituteTeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // SUBSTITUTE LIST
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            var substitutes = _context.SubstituteTeachers
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(substitutes);
        }


        // =====================================================
        // CREATE PAGE
        // =====================================================

        [HttpGet]
        public IActionResult Create(string? teacherName, DateTime? leaveDate)
        {
            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            ViewBag.LeaveTeacher = teacherName;
            ViewBag.LeaveDate = leaveDate?.ToString("yyyy-MM-dd");

            if (leaveDate.HasValue)
            {
                ViewBag.LeaveDay = leaveDate.Value.DayOfWeek.ToString();
            }

            return View();
        }

        // =====================================================
        // GET ABSENT TEACHER'S LECTURES
        // =====================================================

        [HttpGet]
        public IActionResult GetTeacherLectures(
            string teacherName,
            string day)
        {
            if (string.IsNullOrWhiteSpace(teacherName) ||
                string.IsNullOrWhiteSpace(day))
            {
                return Json(Array.Empty<object>());
            }

            teacherName = teacherName.Trim();
            day = day.Trim();

            var lectures = _context.Timetables
                .AsEnumerable()
                .Where(t =>
                    !string.IsNullOrWhiteSpace(t.Teacher) &&
                    t.Teacher.Trim().Equals(
                        teacherName,
                        StringComparison.OrdinalIgnoreCase) &&

                    !string.IsNullOrWhiteSpace(t.Day) &&
                    t.Day.Trim().Equals(
                        day,
                        StringComparison.OrdinalIgnoreCase))
                .Select(t => new
                {
                    timeSlot = t.TimeSlot?.Trim(),
                    subject = t.Subject?.Trim(),
                    className = t.ClassName?.Trim()
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.timeSlot))
                .OrderBy(x => x.timeSlot)
                .ToList();

            return Json(lectures);
        }


        // =====================================================
        // GET TEACHERS FREE AT EXACT TIME
        // =====================================================

        [HttpGet]
        public IActionResult GetFreeTeachers(
            string day,
            string timeSlot,
            string absentTeacher)
        {
            if (string.IsNullOrWhiteSpace(day) ||
                string.IsNullOrWhiteSpace(timeSlot))
            {
                return Json(Array.Empty<object>());
            }

            day = day.Trim();
            timeSlot = timeSlot.Trim();
            absentTeacher = absentTeacher?.Trim() ?? "";

            // Teachers already teaching at this exact
            // day + time
            var busyTeachers = _context.Timetables
                .AsEnumerable()
                .Where(t =>
                    !string.IsNullOrWhiteSpace(t.Day) &&
                    t.Day.Trim().Equals(
                        day,
                        StringComparison.OrdinalIgnoreCase) &&

                    !string.IsNullOrWhiteSpace(t.TimeSlot) &&
                    t.TimeSlot.Trim().Equals(
                        timeSlot,
                        StringComparison.OrdinalIgnoreCase) &&

                    !string.IsNullOrWhiteSpace(t.Teacher))
                .Select(t => t.Teacher!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();


            // All teachers except:
            // 1. Absent teacher
            // 2. Teachers already busy
            var freeTeachers = _context.Teachers
                .AsEnumerable()
                .Where(t =>
                    !string.IsNullOrWhiteSpace(t.TeacherName) &&

                    !t.TeacherName.Trim().Equals(
                        absentTeacher,
                        StringComparison.OrdinalIgnoreCase) &&

                    !busyTeachers.Contains(
                        t.TeacherName.Trim(),
                        StringComparer.OrdinalIgnoreCase))
                .OrderBy(t => t.TeacherName)
                .Select(t => new
                {
                    name = t.TeacherName,
                    department = t.Department
                })
                .ToList();

            return Json(freeTeachers);
        }


        // =====================================================
        // SAVE SUBSTITUTE
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubstituteTeacher substitute)
        {
            if (ModelState.IsValid)
            {
                substitute.Status = "Assigned";

                _context.SubstituteTeachers.Add(substitute);

                _context.SaveChanges();

                TempData["Success"] =
                    "Substitute teacher assigned successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            return View(substitute);
        }


        // =====================================================
        // DELETE SUBSTITUTE
        // =====================================================

        // =====================================================
        // EDIT SUBSTITUTE
        // =====================================================
        [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Delete(int id)
{
    var substitute = _context.SubstituteTeachers
        .FirstOrDefault(x => x.Id == id);

    if (substitute == null)
    {
        return NotFound();
    }

    _context.SubstituteTeachers.Remove(substitute);
    _context.SaveChanges();

    TempData["Success"] =
        "Substitute teacher deleted successfully.";

    return RedirectToAction(nameof(Index));
}

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var substitute = _context.SubstituteTeachers
                .FirstOrDefault(x => x.Id == id);

            if (substitute == null)
            {
                return NotFound();
            }

            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            return View(substitute);
        }


        // =====================================================
        // SAVE EDIT
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, SubstituteTeacher substitute)
        {
            if (id != substitute.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                substitute.Status = "Assigned";

                _context.SubstituteTeachers.Update(substitute);
                _context.SaveChanges();

                TempData["Success"] =
                    "Substitute teacher updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            return View(substitute);
        }
    }
}
