using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class TimetableController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimetableController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // TIMETABLE INDEX
        // =========================
        public IActionResult Index()
        {
            var dayOrder = new Dictionary<string, int>
    {
        { "Monday", 1 },
        { "Tuesday", 2 },
        { "Wednesday", 3 },
        { "Thursday", 4 },
        { "Friday", 5 },
        { "Saturday", 6 }
    };

            var timetables = _context.Timetables
                .ToList()
                .OrderBy(t =>
                    dayOrder.ContainsKey(t.Day ?? "")
                        ? dayOrder[t.Day!]
                        : 99)
                .ThenBy(t => GetTimeSlotOrder(t.TimeSlot))
                .ToList();

            return View(timetables);
        }


        // =========================
        // CREATE - GET
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Classes = _context.Classes.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            ViewBag.Classrooms = _context.Classrooms.ToList();

            return View();
        }


        // =========================
        // CREATE - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Timetable timetable)
        {
            // ==========================================
            // LOAD DROPDOWN DATA
            // ==========================================

            void LoadDropdowns()
            {
                ViewBag.Classes = _context.Classes
                    .OrderBy(c => c.ClassName)
                    .ToList();

                ViewBag.Departments = _context.Departments
                    .OrderBy(d => d.DepartmentName)
                    .ToList();

                ViewBag.Teachers = _context.Teachers
                    .OrderBy(t => t.TeacherName)
                    .ToList();

                ViewBag.Classrooms = _context.Classrooms
                    .OrderBy(c => c.RoomNumber)
                    .ToList();
            }


            // ==========================================
            // VALIDATION
            // ==========================================

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(timetable);
            }


            // ==========================================
            // TEACHER CLASH
            // ==========================================

            bool teacherClash = _context.Timetables.Any(t =>
                t.Teacher == timetable.Teacher &&
                t.Day == timetable.Day &&
                t.TimeSlot == timetable.TimeSlot &&
                t.Id != timetable.Id);

            if (teacherClash)
            {
                ModelState.AddModelError(
                    "",
                    "Teacher is already assigned at this day and time.");

                LoadDropdowns();
                return View(timetable);
            }


            // ==========================================
            // CLASSROOM CLASH
            // ==========================================

            bool classroomClash = _context.Timetables.Any(t =>
                t.Classroom == timetable.Classroom &&
                t.Day == timetable.Day &&
                t.TimeSlot == timetable.TimeSlot &&
                t.Id != timetable.Id);

            if (classroomClash)
            {
                ModelState.AddModelError(
                    "",
                    "Classroom is already occupied at this day and time.");

                LoadDropdowns();
                return View(timetable);
            }


            // ==========================================
            // CLASS CLASH
            // ==========================================

            bool classClash = _context.Timetables.Any(t =>
                t.ClassName == timetable.ClassName &&
                t.Day == timetable.Day &&
                t.TimeSlot == timetable.TimeSlot &&
                t.Id != timetable.Id);

            if (classClash)
            {
                ModelState.AddModelError(
                    "",
                    "This class already has a lecture at this day and time.");

                LoadDropdowns();
                return View(timetable);
            }


            // ==========================================
            // SAVE
            // ==========================================

            _context.Timetables.Add(timetable);
            _context.SaveChanges();

            TempData["Success"] =
                "Timetable saved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var timetable = _context.Timetables.FirstOrDefault(t => t.Id == id);

            if (timetable == null)
            {
                return NotFound();
            }

            LoadEditDropdowns(timetable);

            return View(timetable);
        }
        //Edit-post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Timetable timetable)
        {
            if (id != timetable.Id)
            {
                return NotFound();
            }

            // ==========================================
            // TEACHER CLASH
            // ==========================================

            bool teacherClash = _context.Timetables.Any(t =>
                t.Teacher == timetable.Teacher &&
                t.Day == timetable.Day &&
                t.TimeSlot == timetable.TimeSlot &&
                t.Id != timetable.Id);

            if (teacherClash)
            {
                ModelState.AddModelError(
                    "",
                    "Teacher is already assigned at this time.");

                LoadEditDropdowns(timetable);

                return View(timetable);
            }

            // ==========================================
            // CLASSROOM CLASH
            // ==========================================

            bool classroomClash = _context.Timetables.Any(t =>
                t.Classroom == timetable.Classroom &&
                t.Day == timetable.Day &&
                t.TimeSlot == timetable.TimeSlot &&
                t.Id != timetable.Id);

            if (classroomClash)
            {
                ModelState.AddModelError(
                    "",
                    "Classroom is already occupied at this time.");

                LoadEditDropdowns(timetable);

                return View(timetable);
            }

            // ==========================================
            // CLASS CLASH
            // ==========================================

            bool classClash = _context.Timetables.Any(t =>
                t.ClassName == timetable.ClassName &&
                t.Day == timetable.Day &&
                t.TimeSlot == timetable.TimeSlot &&
                t.Id != timetable.Id);

            if (classClash)
            {
                ModelState.AddModelError(
                    "",
                    "This class already has a lecture at this time.");

                LoadEditDropdowns(timetable);

                return View(timetable);
            }

            // ==========================================
            // MODEL VALIDATION
            // ==========================================

            if (!ModelState.IsValid)
            {
                LoadEditDropdowns(timetable);

                return View(timetable);
            }

            // ==========================================
            // UPDATE
            // ==========================================

            _context.Timetables.Update(timetable);
            _context.SaveChanges();

            // 📜 Automatic History
            var history = new TimetableHistory
            {
                ClassName = timetable.ClassName,
                Department = timetable.Department,
                Action = "Timetable Updated",
                CreatedDate = DateTime.Now
            };

            _context.TimetableHistories.Add(history);
            _context.SaveChanges();

            TempData["Success"] =
                "Timetable updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var timetable = _context.Timetables.Find(id);

            if (timetable != null)
            {
                _context.Timetables.Remove(timetable);
                _context.SaveChanges();

                TempData["Success"] = "Timetable deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // WEEKLY TIMETABLE
        // =========================
        [HttpGet]
        public IActionResult WeeklyTimetable(string? className)
        {
            var query = _context.Timetables.AsQueryable();

            if (!string.IsNullOrWhiteSpace(className))
            {
                query = query.Where(t => t.ClassName == className);
            }

            var dayOrder = new Dictionary<string, int>
    {
        { "Monday", 1 },
        { "Tuesday", 2 },
        { "Wednesday", 3 },
        { "Thursday", 4 },
        { "Friday", 5 },
        { "Saturday", 6 }
    };

            var timetables = query
                .ToList()
                .OrderBy(t =>
                    dayOrder.ContainsKey(t.Day ?? "")
                        ? dayOrder[t.Day!]
                        : 99)
                .ThenBy(t => t.TimeSlot)
                .ToList();

            ViewBag.ClassName = className;

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .Select(c => c.ClassName)
                .Distinct()
                .ToList();

            return View(timetables);
        }


        // =========================
        // VIEW TIMETABLE
        // =========================
        // =========================
        // VIEW GENERATED TIMETABLE
        // =========================
        [HttpGet]
        public IActionResult ViewTimetable(string? className)
        {
            var query = _context.Timetables.AsQueryable();

            // Show ONLY the selected class
            if (!string.IsNullOrWhiteSpace(className))
            {
                query = query.Where(t => t.ClassName == className);
            }

            var dayOrder = new Dictionary<string, int>
    {
        { "Monday", 1 },
        { "Tuesday", 2 },
        { "Wednesday", 3 },
        { "Thursday", 4 },
        { "Friday", 5 },
        { "Saturday", 6 }
    };

            var timetables = query
                .ToList()
                .OrderBy(t =>
                    dayOrder.ContainsKey(t.Day ?? "")
                        ? dayOrder[t.Day!]
                        : 99)
                .ThenBy(t => GetTimeSlotOrder(t.TimeSlot))
                .ToList();

            ViewBag.ClassName = className;

            // =====================================================
            // LAB NUMBER MAPPING
            // Physical room → Lab number
            // Example:
            // Room 111 → Lab 1
            // Room 114 → Lab 2
            // Room 115 → Lab 3
            // =====================================================

            var labMap = _context.Classrooms
     .Where(r =>
         !string.IsNullOrWhiteSpace(r.RoomNumber) &&
         !string.IsNullOrWhiteSpace(r.RoomType))
     .AsEnumerable()
     .Where(r =>
         r.RoomType!.Contains(
             "lab",
             StringComparison.OrdinalIgnoreCase))
     .OrderBy(r => r.Id)
     .Select((r, index) => new
     {
         RoomNumber = r.RoomNumber!,
         LabNumber = $"Lab {index + 1}"
     })
     .ToDictionary(
         x => x.RoomNumber,
         x => x.LabNumber);

            ViewBag.LabMap = labMap;

            return View(timetables);
        }

        // =========================
        // GET CLASS DETAILS
        // =========================
        [HttpGet]
        public IActionResult GetClassDetails(string className)
        {
            var classData = _context.Classes
                .FirstOrDefault(c => c.ClassName == className);

            if (classData == null)
            {
                return Json(new
                {
                    department = "",
                    semester = 0
                });
            }

            return Json(new
            {
                department = classData.Department,
                semester = classData.Semester
            });
        }

        // =========================
        // GET SUBJECTS
        // =========================
        [HttpGet]
        public IActionResult GetSubjects(string department, int semester)
        {
            var subjects = _context.Subjects
                .Where(s =>
                    s.Department == department &&
                    s.Semester == semester)
                .ToList();

            return Json(subjects);
        }

        // =========================
        // GET TEACHERS
        // =========================
        // =========================
        // GET TEACHERS FOR SUBJECT
        // =========================

        [HttpGet]
        public IActionResult GetTeachers(string department, string subject)
        {
            if (string.IsNullOrWhiteSpace(department) ||
                string.IsNullOrWhiteSpace(subject))
            {
                return Json(Array.Empty<object>());
            }

            var teachers = (
                from assignment in _context.TeacherAssignments
                join teacher in _context.Teachers
                    on assignment.TeacherId equals teacher.Id
                where assignment.Subject == subject
                      && teacher.Department == department
                select new
                {
                    teacherName = teacher.TeacherName
                }
            )
            .Distinct()
            .OrderBy(x => x.teacherName)
            .ToList();

            return Json(teachers);
        }

        // =========================
        // GET CLASSROOMS
        // =========================
        [HttpGet]
        public IActionResult GetClassrooms(string department)
        {
            var classrooms = _context.Classrooms
                .Where(c => c.Department == department)
                .ToList();

            return Json(classrooms);
        }




        // =========================
        // TEACHER WORKLOAD
        // =========================
        [HttpGet]
        public IActionResult TeacherWorkload()
        {
            var timetableData = _context.Timetables
                .Where(t => !string.IsNullOrEmpty(t.Teacher))
                .ToList();

            var workload = timetableData
                .GroupBy(t => t.Teacher)
                .Select(g => new TeacherWorkload
                {
                    TeacherName = g.Key ?? "Unknown",

                    TotalLectures = g.Count(),

                    Subjects = string.Join(
                        ", ",
                        g.Where(x => !string.IsNullOrEmpty(x.Subject))
                         .Select(x => x.Subject)
                         .Distinct()
                    ),

                    WorkingDays = g.Where(x => !string.IsNullOrEmpty(x.Day))
                                   .Select(x => x.Day)
                                   .Distinct()
                                   .Count(),

                    Classes = string.Join(
                        ", ",
                        g.Where(x => !string.IsNullOrEmpty(x.ClassName))
                         .Select(x => x.ClassName)
                         .Distinct()
                    )
                })
                .OrderByDescending(x => x.TotalLectures)
                .ToList();

            return View(workload);
        }

        // =========================
        // TEACHER TIMETABLE
        // =========================
        [HttpGet]
        public IActionResult TeacherTimetable(string? teacherName)
        {
            var query = _context.Timetables.AsQueryable();

            if (!string.IsNullOrWhiteSpace(teacherName))
            {
                query = query.Where(t => t.Teacher == teacherName);
            }

            var dayOrder = new Dictionary<string, int>
    {
        { "Monday", 1 },
        { "Tuesday", 2 },
        { "Wednesday", 3 },
        { "Thursday", 4 },
        { "Friday", 5 },
        { "Saturday", 6 }
    };

            var timetables = query
                .ToList()
                .OrderBy(t =>
                    dayOrder.ContainsKey(t.Day ?? "")
                        ? dayOrder[t.Day!]
                        : 99)
               .ThenBy(t => GetTimeSlotOrder(t.TimeSlot))
                .ToList();

            ViewBag.TeacherName = teacherName;

            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .Select(t => t.TeacherName)
                .Distinct()
                .ToList();

            return View(timetables);
        }

        // =========================
        // AUTOMATIC TIMETABLE GENERATION PAGE
        // =========================
        // =========================================================
        // AUTOMATIC TIMETABLE GENERATION PAGE
        // =========================================================

        [HttpGet]
        public IActionResult AutoGenerate()
        {
            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.Department)
                .ThenBy(c => c.Semester)
                .ThenBy(c => c.ClassName)
                .ToList();

            return View();
        }

        // =========================================================
        // AUTOMATIC TIMETABLE GENERATION
        // =========================================================
        // =========================================================
        // PROFESSIONAL CLASS-WISE TIMETABLE GENERATION
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Generate(int classId)
        {
            const string academicYear = "2026-27";

            // =====================================================
            // 1. GET SELECTED CLASS
            // =====================================================

            var selectedClass = _context.Classes
                .FirstOrDefault(c => c.Id == classId);

            if (selectedClass == null)
            {
                TempData["Error"] = "Please select a valid class.";
                return RedirectToAction(nameof(AutoGenerate));
            }

            string className = selectedClass.ClassName?.Trim() ?? "";
            string department = selectedClass.Department?.Trim() ?? "";
            int semester = selectedClass.Semester;

            // =====================================================
            // 2. GET TEACHING DETAILS FOR SELECTED CLASS ONLY
            // =====================================================

            var assignments = _context.TeacherAssignments
                .Where(a =>
                    a.ClassName == className &&
                    a.Semester == semester)
                .ToList();

            if (!assignments.Any())
            {
                TempData["Error"] =
                    $"No teaching details found for {className}. " +
                    "Please add the subjects, teachers and subject types first.";

                return RedirectToAction(nameof(AutoGenerate));
            }

            // =====================================================
            // 3. GET REQUIRED TEACHERS
            // =====================================================

            var teacherIds = assignments
                .Select(a => a.TeacherId)
                .Distinct()
                .ToList();

            var teachers = _context.Teachers
                .Where(t => teacherIds.Contains(t.Id))
                .ToList();

            // =====================================================
            // 4. GET AVAILABLE ROOMS / LABS
            // =====================================================

            // =====================================================
            // 4. GET AVAILABLE CLASSROOMS AND ALL COLLEGE LABS
            // =====================================================

            // Normal classrooms for theory/project
            var classrooms = _context.Classrooms
                .Where(r =>
                    r.Department == department ||
                    string.IsNullOrWhiteSpace(r.Department))
                .ToList();

            // IMPORTANT:
            // Labs are shared by ALL classes and ALL departments.
            // Therefore, do NOT filter labs by department.
            var allLabs = _context.Classrooms
    .Where(r =>
        !string.IsNullOrWhiteSpace(r.RoomNumber) &&
        !string.IsNullOrWhiteSpace(r.RoomType))
    .AsEnumerable()
    .Where(r => r.RoomType.Contains("lab", StringComparison.OrdinalIgnoreCase))
    .OrderBy(r => r.Id)
    .ToList();

            if (!classrooms.Any() && !allLabs.Any())
            {
                TempData["Error"] =
                    $"No classrooms or labs are available for {className}.";

                return RedirectToAction(nameof(AutoGenerate));
            }

            if (!allLabs.Any())
            {
                TempData["Error"] =
                    "No practical labs are available. Please add Lab 1, Lab 2 or Lab 3.";

                return RedirectToAction(nameof(AutoGenerate));
            }

            // =====================================================
            // 5. REMOVE OLD TIMETABLE FOR SELECTED CLASS ONLY
            // =====================================================

            var oldEntries = _context.Timetables
                .Where(t =>
                    t.ClassName == className &&
                    t.Semester == semester &&
                    t.AcademicYear == academicYear)
                .ToList();

            if (oldEntries.Any())
            {
                _context.Timetables.RemoveRange(oldEntries);
                _context.SaveChanges();
            }

            // =====================================================
            // 6. WORKING DAYS
            // =====================================================

            string[] days =
            {
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
    };

            // =====================================================
            // 7. ONE-HOUR PERIODS
            // =====================================================

            string[] periods =
            {
        "9:00 AM - 10:00 AM",
        "10:00 AM - 11:00 AM",
        "11:30 AM - 12:30 PM",
        "12:30 PM - 1:30 PM"
    };

            // =====================================================
            // 8. TWO-HOUR PRACTICAL / PROJECT BLOCKS
            // =====================================================

            var twoHourBlocks = new[]
            {
        new
        {
            First = "9:00 AM - 10:00 AM",
            Second = "10:00 AM - 11:00 AM"
        },

        new
        {
            First = "11:30 AM - 12:30 PM",
            Second = "12:30 PM - 1:30 PM"
        }
    };

            // =====================================================
            // 9. GENERATED RESULT
            // =====================================================

            var generated = new List<Timetable>();

            var failedSubjects = new List<string>();

            // =====================================================
            // 10. CHECK SLOT AGAINST:
            //     - SELECTED CLASS
            //     - TEACHER
            //     - CLASSROOM
            //     - OTHER ALREADY GENERATED CLASSES
            // =====================================================

            bool IsSlotFree(
                string teacher,
                string day,
                string time,
                string classroom)
            {
                // Check entries generated during this operation
                bool generatedClash = generated.Any(t =>
                    t.Day == day &&
                    t.TimeSlot == time &&
                    (
                        t.Teacher == teacher ||
                        t.Classroom == classroom ||
                        t.ClassName == className
                    ));

                if (generatedClash)
                {
                    return false;
                }

                // Check timetable entries belonging to OTHER classes
                bool databaseClash = _context.Timetables.Any(t =>
                    t.AcademicYear == academicYear &&
                    t.Day == day &&
                    t.TimeSlot == time &&
                    (
                        t.Teacher == teacher ||
                        t.Classroom == classroom
                    ));

                return !databaseClash;
            }

            // =====================================================
            // 11. SUBJECT ALREADY ON THAT DAY
            // =====================================================

            bool SubjectAlreadyOnDay(
                string subject,
                string day)
            {
                return generated.Any(t =>
                    t.ClassName == className &&
                    t.Subject == subject &&
                    t.Day == day);
            }

            // =====================================================
            // 12. CLASS DAILY LOAD
            // =====================================================

            int ClassDailyLoad(string day)
            {
                return generated.Count(t =>
                    t.ClassName == className &&
                    t.Day == day);
            }

            // =====================================================
            // 13. TEACHER DAILY LOAD
            // =====================================================

            int TeacherDailyLoad(
                string teacher,
                string day)
            {
                int generatedLoad = generated.Count(t =>
                    t.Teacher == teacher &&
                    t.Day == day);

                int databaseLoad = _context.Timetables.Count(t =>
                    t.AcademicYear == academicYear &&
                    t.Teacher == teacher &&
                    t.Day == day &&
                    t.ClassName != className);

                return generatedLoad + databaseLoad;
            }

            // =====================================================
            // 14. FIND NORMAL CLASSROOM
            // =====================================================

            string? FindNormalRoom(
                string teacher,
                string day,
                string time)
            {
                return classrooms
                    .Where(r =>
                        !string.IsNullOrWhiteSpace(r.RoomNumber))

                    .Where(r =>
                        string.IsNullOrWhiteSpace(r.RoomType) ||
                        !r.RoomType.Contains(
                            "lab",
                            StringComparison.OrdinalIgnoreCase))

                    .Where(r =>
                        string.IsNullOrWhiteSpace(r.Department) ||
                        r.Department.Equals(
                            department,
                            StringComparison.OrdinalIgnoreCase))

                    .Where(r =>
                        IsSlotFree(
                            teacher,
                            day,
                            time,
                            r.RoomNumber!))

                    .Select(r => r.RoomNumber)
                    .FirstOrDefault();
            }

            // =====================================================
            // 15. FIND COMPUTER / PRACTICAL LAB
            // =====================================================

            // =====================================================
            // FIND AVAILABLE LAB FOR PRACTICAL
            // =====================================================

            string? FindLab(
                string teacher,
                string day,
                string first,
                string second)
            {
                // Search ALL college labs.
                // A lab cannot be used by another class
                // at the same time.

                return allLabs
                    .Where(r =>
                        !string.IsNullOrWhiteSpace(r.RoomNumber))

                    .Where(r =>
                        IsSlotFree(
                            teacher,
                            day,
                            first,
                            r.RoomNumber!) &&

                        IsSlotFree(
                            teacher,
                            day,
                            second,
                            r.RoomNumber!))

                    .Select(r => r.RoomNumber)
                    .FirstOrDefault();
            }

            // =====================================================
            // 16. REMOVE EXACT DUPLICATE ASSIGNMENTS
            // =====================================================

            assignments = assignments
                .Where(a =>
                    !string.IsNullOrWhiteSpace(a.Subject) &&
                    !string.IsNullOrWhiteSpace(a.ClassName))

                .GroupBy(a => new
                {
                    TeacherId = a.TeacherId,

                    ClassName = a.ClassName!.Trim(),

                    Semester = a.Semester,

                    Subject = a.Subject!.Trim(),

                    SubjectType = (a.SubjectType ?? "Theory").Trim()
                })

                .Select(g => g.First())

                .ToList();

            // =====================================================
            // 17. SUBJECT TYPE GROUPS
            // =====================================================

            var practicals = assignments
                .Where(a =>
                    string.Equals(
                        a.SubjectType,
                        "Practical",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            var projects = assignments
                .Where(a =>
                    string.Equals(
                        a.SubjectType,
                        "Project",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            var theoryAndPracticals = assignments
                .Where(a =>
                    string.Equals(
                        a.SubjectType,
                        "Theory + Practical",
                        StringComparison.OrdinalIgnoreCase))
                .ToList();

            var theories = assignments
                .Where(a =>
                    string.Equals(
                        a.SubjectType,
                        "Theory",
                        StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(a.SubjectType))
                .ToList();

            // =====================================================
            // 18. ADD TWO-HOUR ENTRY
            // =====================================================

            void AddTwoHourEntry(
                TeacherAssignment assignment,
                string day,
                string first,
                string second,
                string room,
                string subjectType)
            {
                var teacher = teachers
                    .FirstOrDefault(t =>
                        t.Id == assignment.TeacherId);

                if (teacher == null ||
                    string.IsNullOrWhiteSpace(teacher.TeacherName) ||
                    string.IsNullOrWhiteSpace(assignment.Subject))
                {
                    return;
                }

                string teacherName = teacher.TeacherName;
                string subject = assignment.Subject.Trim();

                generated.Add(new Timetable
                {
                    ClassName = className,
                    Department = department,
                    Semester = semester,
                    Day = day,
                    TimeSlot = first,
                    Subject = subject,
                    SubjectType = subjectType,
                    Teacher = teacherName,
                    Classroom = room,
                    AcademicYear = academicYear
                });

                generated.Add(new Timetable
                {
                    ClassName = className,
                    Department = department,
                    Semester = semester,
                    Day = day,
                    TimeSlot = second,
                    Subject = subject,
                    SubjectType = subjectType,
                    Teacher = teacherName,
                    Classroom = room,
                    AcademicYear = academicYear
                });
            }

            // =====================================================
            // 19. SCHEDULE PRACTICAL / PROJECT
            // =====================================================

            void ScheduleTwoHour(
                TeacherAssignment assignment,
                bool useLab,
                string subjectType)
            {
                var teacher = teachers
                    .FirstOrDefault(t => t.Id == assignment.TeacherId);

                if (teacher == null ||
                    string.IsNullOrWhiteSpace(teacher.TeacherName) ||
                    string.IsNullOrWhiteSpace(assignment.Subject))
                {
                    return;
                }

                string teacherName = teacher.TeacherName.Trim();
                string subject = assignment.Subject.Trim();

                var candidates = new List<(
                    string Day,
                    string First,
                    string Second,
                    string Room,
                    int Score
                )>();

                foreach (var day in days)
                {
                    // Practical/Project should happen only ONCE in the week
                    if (SubjectAlreadyOnDay(subject, day))
                    {
                        continue;
                    }

                    foreach (var block in twoHourBlocks)
                    {
                        string? room;

                        // -----------------------------
                        // PRACTICAL = LAB
                        // -----------------------------
                        if (useLab)
                        {
                            room = FindLab(
                                teacherName,
                                day,
                                block.First,
                                block.Second);
                        }

                        // -----------------------------
                        // PROJECT = NORMAL CLASSROOM
                        // -----------------------------
                        else
                        {
                            room = FindNormalRoom(
                                teacherName,
                                day,
                                block.First);

                            if (!string.IsNullOrWhiteSpace(room))
                            {
                                if (!IsSlotFree(
                                    teacherName,
                                    day,
                                    block.Second,
                                    room))
                                {
                                    room = null;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(room))
                        {
                            continue;
                        }

                        int score =
                            (ClassDailyLoad(day) * 100) +
                            (TeacherDailyLoad(
                                teacherName,
                                day) * 20);

                        candidates.Add(
                            (
                                day,
                                block.First,
                                block.Second,
                                room,
                                score
                            ));
                    }
                }

                var selected = candidates
                    .OrderBy(x => x.Score)
                    .ThenBy(x => Array.IndexOf(days, x.Day))
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(selected.Room))
                {
                    failedSubjects.Add(
                        $"{subject} ({subjectType})");

                    return;
                }

                // ONE practical/project session = TWO HOURS
                AddTwoHourEntry(
                    assignment,
                    selected.Day,
                    selected.First,
                    selected.Second,
                    selected.Room,
                    subjectType);
            }


            // =====================================================
            // 20. PRACTICALS FIRST
            // =====================================================

            foreach (var assignment in practicals)
            {
                // IMPORTANT:
                // Every practical subject gets ONLY ONE
                // practical session per week.
                //
                // The session occupies TWO continuous hours.

                ScheduleTwoHour(
                    assignment,
                    true,
                    "Practical");
            }


            // =====================================================
            // 21. PROJECTS SECOND
            // =====================================================

            foreach (var assignment in projects)
            {
                // IMPORTANT:
                // Every project gets ONLY ONE
                // project session per week.
                //
                // The session occupies TWO continuous hours.

                ScheduleTwoHour(
                    assignment,
                    false,
                    "Project");
            }


            // =====================================================
            // 22. THEORY + PRACTICAL
            //
            // 2 theory lectures = 2 separate one-hour periods
            // 1 practical = 1 two-hour continuous block
            // =====================================================

            foreach (var assignment in theoryAndPracticals)
            {
                var teacher = teachers
                    .FirstOrDefault(t => t.Id == assignment.TeacherId);

                if (teacher == null ||
                    string.IsNullOrWhiteSpace(teacher.TeacherName) ||
                    string.IsNullOrWhiteSpace(assignment.Subject))
                {
                    continue;
                }

                string teacherName = teacher.TeacherName.Trim();
                string subject = assignment.Subject.Trim();

                // =================================================
                // THEORY PART
                // ALWAYS 2 LECTURES PER WEEK
                // EACH LECTURE = 1 HOUR
                // =================================================

                int requiredTheory = 2;

                int theoryCreated = 0;

                for (int lecture = 0;
                     lecture < requiredTheory;
                     lecture++)
                {
                    var candidates = new List<(
                        string Day,
                        string Time,
                        string Room,
                        int Score
                    )>();

                    foreach (var day in days)
                    {
                        // Do not put the same subject twice
                        // on the same day.
                        if (SubjectAlreadyOnDay(subject, day))
                        {
                            continue;
                        }

                        foreach (var time in periods)
                        {
                            string? room = FindNormalRoom(
                                teacherName,
                                day,
                                time);

                            if (string.IsNullOrWhiteSpace(room))
                            {
                                continue;
                            }

                            int score =
                                (ClassDailyLoad(day) * 100) +
                                (TeacherDailyLoad(
                                    teacherName,
                                    day) * 20);

                            candidates.Add(
                                (
                                    day,
                                    time,
                                    room,
                                    score
                                ));
                        }
                    }

                    var selected = candidates
                        .OrderBy(x => x.Score)
                        .ThenBy(x => Array.IndexOf(days, x.Day))
                        .ThenBy(x => Array.IndexOf(periods, x.Time))
                        .FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(selected.Room))
                    {
                        break;
                    }

                    generated.Add(new Timetable
                    {
                        ClassName = className,
                        Department = department,
                        Semester = semester,
                        Day = selected.Day,
                        TimeSlot = selected.Time,
                        Subject = subject,
                        SubjectType = "Theory + Practical",
                        Teacher = teacherName,
                        Classroom = selected.Room,
                        AcademicYear = academicYear
                    });

                    theoryCreated++;
                }

                if (theoryCreated < requiredTheory)
                {
                    failedSubjects.Add(
                        $"{subject} (Theory + Practical) - " +
                        $"{requiredTheory - theoryCreated} theory lecture(s) not scheduled");
                }


                // =================================================
                // PRACTICAL PART
                // ONLY ONE PRACTICAL PER WEEK
                // 2 CONTINUOUS HOURS
                // =================================================

                ScheduleTwoHour(
                    assignment,
                    true,
                    "Theory + Practical");
            }


            // =====================================================
            // 23. THEORY SUBJECTS
            //
            // Every theory subject:
            // 2 lectures per week
            // Each lecture = 1 hour
            // =====================================================

            foreach (var assignment in theories)
            {
                var teacher = teachers
                    .FirstOrDefault(t => t.Id == assignment.TeacherId);

                if (teacher == null ||
                    string.IsNullOrWhiteSpace(teacher.TeacherName) ||
                    string.IsNullOrWhiteSpace(assignment.Subject))
                {
                    continue;
                }

                string teacherName = teacher.TeacherName.Trim();
                string subject = assignment.Subject.Trim();

                // IMPORTANT:
                // Every theory subject gets exactly
                // TWO lectures per week.
                int required = 2;

                int created = 0;

                for (int lecture = 0;
                     lecture < required;
                     lecture++)
                {
                    var candidates = new List<(
                        string Day,
                        string Time,
                        string Room,
                        int Score
                    )>();

                    foreach (var day in days)
                    {
                        // Do not put both theory lectures
                        // on the same day.
                        if (SubjectAlreadyOnDay(subject, day))
                        {
                            continue;
                        }

                        foreach (var time in periods)
                        {
                            string? room = FindNormalRoom(
                                teacherName,
                                day,
                                time);

                            if (string.IsNullOrWhiteSpace(room))
                            {
                                continue;
                            }

                            int score =
                                (ClassDailyLoad(day) * 100) +
                                (TeacherDailyLoad(
                                    teacherName,
                                    day) * 20);

                            candidates.Add(
                                (
                                    day,
                                    time,
                                    room,
                                    score
                                ));
                        }
                    }

                    var selected = candidates
                        .OrderBy(x => x.Score)
                        .ThenBy(x => Array.IndexOf(days, x.Day))
                        .ThenBy(x => Array.IndexOf(periods, x.Time))
                        .FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(selected.Room))
                    {
                        break;
                    }

                    generated.Add(new Timetable
                    {
                        ClassName = className,
                        Department = department,
                        Semester = semester,
                        Day = selected.Day,
                        TimeSlot = selected.Time,
                        Subject = subject,
                        SubjectType = "Theory",
                        Teacher = teacherName,
                        Classroom = selected.Room,
                        AcademicYear = academicYear
                    });

                    created++;
                }

                if (created < required)
                {
                    failedSubjects.Add(
                        $"{subject} (Theory) - " +
                        $"{required - created} lecture(s) not scheduled");
                }
            }

            // =====================================================
            // 24. SAVE
            // =====================================================

            // =====================================================
            // 24. SAVE
            // =====================================================

            if (generated.Any())
            {
                _context.Timetables.AddRange(generated);
                _context.SaveChanges();

                // 📜 AUTOMATIC TIMETABLE HISTORY
                var history = new TimetableHistory
                {
                    ClassName = className,
                    Department = department,
                    Action = "Timetable Generated",
                    CreatedDate = DateTime.Now
                };

                _context.TimetableHistories.Add(history);
                _context.SaveChanges();

                // 🔔 AUTOMATIC NOTIFICATION
                var notification = new Notification
                {
                    Title = "Timetable Generated",
                    Message = $"Timetable for {className} has been generated successfully for academic year {academicYear}.",
                    CreatedDate = DateTime.Now,
                    Status = "New"
                };

                _context.Notifications.Add(notification);
                _context.SaveChanges();
            }

            // =====================================================
            // 25. RESULT
            // =====================================================

            if (!generated.Any())
            {
                TempData["Error"] =
                    $"Timetable could not be generated for {className}. " +
                    "Please check teaching details, classrooms and labs.";

                return RedirectToAction(nameof(AutoGenerate));
            }

            TempData["Success"] =
                $"Timetable generated successfully for {className}. " +
                $"{generated.Count} timetable periods created.";

            if (failedSubjects.Any())
            {
                TempData["Error"] =
                    "Some teaching requirements could not be completely scheduled: " +
                    string.Join(", ", failedSubjects);
            }

            return RedirectToAction(
                nameof(ViewTimetable),
                new { className = className });
        }

        // =========================
        // CLEAR TIMETABLE
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearTimetable()
        {
            var timetables = _context.Timetables.ToList();

            if (timetables.Any())
            {
                _context.Timetables.RemoveRange(timetables);
                _context.SaveChanges();

                TempData["Success"] =
                    "All timetable entries cleared successfully.";
            }
            else
            {
                TempData["Success"] =
                    "There are no timetable entries to clear.";
            }

            return RedirectToAction(nameof(AutoGenerate));
        }
        // =========================
        // LOAD EDIT DROPDOWNS
        // =========================
        private void LoadEditDropdowns(Timetable timetable)
        {
            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ToList();

            ViewBag.Departments = _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToList();

            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            ViewBag.Classrooms = _context.Classrooms
                .OrderBy(c => c.RoomNumber)
                .ToList();

            ViewBag.Subjects = _context.Subjects
                .Where(s =>
                    s.Department == timetable.Department &&
                    s.Semester == timetable.Semester)
                .OrderBy(s => s.SubjectName)
                .ToList();
        }
        private int GetTimeSlotOrder(string? timeSlot)
        {
            return timeSlot switch
            {
                "9:00 AM - 10:00 AM" => 1,
                "10:00 AM - 11:00 AM" => 2,
                "11:30 AM - 12:30 PM" => 3,
                "12:30 PM - 1:30 PM" => 4,
                _ => 99
            };
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var history = _context.TimetableHistories
                .FirstOrDefault(h => h.Id == id);

            if (history == null)
            {
                return NotFound();
            }

            _context.TimetableHistories.Remove(history);
            _context.SaveChanges();

            TempData["Success"] = "Timetable history deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }

}