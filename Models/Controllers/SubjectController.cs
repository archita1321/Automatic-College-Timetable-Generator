using AutomaticCollegeTimetableGenerator.Data;
using AutomaticCollegeTimetableGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutomaticCollegeTimetableGenerator.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // MANAGE SUBJECTS
        // =========================================================

        public IActionResult Index()
        {
            var subjects = _context.Subjects
                .OrderBy(s => s.Teacher)
                .ThenBy(s => s.Department)
                .ThenBy(s => s.ClassName)
                .ThenBy(s => s.Semester)
                .ThenBy(s => s.SubjectName)
                .ThenBy(s => s.SubjectType)
                .ToList();

            return View(subjects);
        }


        // =========================================================
        // ADD SUBJECT - GET
        // =========================================================

        [HttpGet]
        public IActionResult Create(int? teacherId)
        {
            LoadDropdowns();

            var model = new BulkSubjectViewModel();

            if (teacherId.HasValue)
            {
                var teacher = _context.Teachers
                    .FirstOrDefault(t => t.Id == teacherId.Value);

                if (teacher != null)
                {
                    model.TeacherId = teacher.Id;
                }
            }

            return View(model);
        }


        // =========================================================
        // ADD SUBJECT - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BulkSubjectViewModel model)
        {
            LoadDropdowns();

            var teacher = _context.Teachers
                .FirstOrDefault(t => t.Id == model.TeacherId);

            if (teacher == null)
            {
                ModelState.AddModelError(
                    "TeacherId",
                    "Please select a valid teacher.");
            }

            if (model.Subjects == null || !model.Subjects.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Please add at least one subject.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int addedCount = 0;
            int duplicateCount = 0;

            foreach (var row in model.Subjects)
            {
                if (string.IsNullOrWhiteSpace(row.SubjectName) ||
                    string.IsNullOrWhiteSpace(row.ClassName) ||
                    string.IsNullOrWhiteSpace(row.Department))
                {
                    continue;
                }

                string subjectName = row.SubjectName.Trim();
                string className = row.ClassName.Trim();
                string department = row.Department.Trim();
                string subjectType = string.IsNullOrWhiteSpace(row.SubjectType)
                    ? "Theory"
                    : row.SubjectType.Trim();

                // =====================================================
                // AUTOMATIC LECTURE + DURATION
                // =====================================================

                int lectureCount;
                string duration;

                if (subjectType.Equals(
                    "Theory",
                    StringComparison.OrdinalIgnoreCase))
                {
                    lectureCount = 2;
                    duration = "1 Hour";
                }
                else
                {
                    // Practical / Project
                    lectureCount = 1;
                    duration = "2 Hours";
                }


                // =====================================================
                // CHECK DUPLICATE SUBJECT
                // =====================================================

                bool alreadyExists = _context.Subjects.Any(s =>
                    s.Teacher == teacher!.TeacherName &&
                    s.SubjectName == subjectName &&
                    s.ClassName == className &&
                    s.Department == department &&
                    s.Semester == row.Semester &&
                    s.SubjectType == subjectType);

                if (alreadyExists)
                {
                    duplicateCount++;
                    continue;
                }


                // =====================================================
                // CREATE SUBJECT
                // =====================================================

                var subject = new Subject
                {
                    SubjectName = subjectName,
                    ClassName = className,
                    Department = department,
                    Semester = row.Semester,
                    Teacher = teacher!.TeacherName ?? "",
                    LectureCount = lectureCount,
                    Duration = duration,
                    SubjectType = subjectType
                };

                _context.Subjects.Add(subject);


                // =====================================================
                // CREATE MATCHING TEACHER ASSIGNMENT
                // =====================================================

                bool assignmentExists =
                    _context.TeacherAssignments.Any(a =>
                        a.TeacherId == teacher.Id &&
                        a.ClassName == className &&
                        a.Semester == row.Semester &&
                        a.Subject == subjectName &&
                        a.SubjectType == subjectType);

                if (!assignmentExists)
                {
                    _context.TeacherAssignments.Add(
                        new TeacherAssignment
                        {
                            TeacherId = teacher.Id,
                            ClassName = className,
                            Semester = row.Semester,
                            Subject = subjectName,
                            LectureCount = lectureCount,
                            Duration = duration,
                            SubjectType = subjectType
                        });
                }

                addedCount++;
            }

            _context.SaveChanges();


            // =====================================================
            // MESSAGE
            // =====================================================

            if (addedCount == 0)
            {
                TempData["ErrorMessage"] =
                    "No new subjects were added. The subjects may already exist.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    $"{addedCount} subject(s) added successfully.";

                if (duplicateCount > 0)
                {
                    TempData["SuccessMessage"] +=
                        $" {duplicateCount} duplicate subject(s) skipped.";
                }
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // EDIT SUBJECT - GET
        // =========================================================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var subject = _context.Subjects
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }


        // =========================================================
        // EDIT SUBJECT - POST
        // SUBJECT + MATCHING TEACHER ASSIGNMENT UPDATED TOGETHER
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Subject subject)
        {
            var existingSubject = _context.Subjects
                .FirstOrDefault(s => s.Id == id);

            if (existingSubject == null)
            {
                return NotFound();
            }


            // =====================================================
            // SAVE OLD VALUES
            // =====================================================

            string oldSubjectName =
                existingSubject.SubjectName ?? "";

            string oldSubjectType =
                existingSubject.SubjectType ?? "Theory";

            string oldTeacherName =
                existingSubject.Teacher ?? "";

            string oldClassName =
                existingSubject.ClassName ?? "";

            string oldDepartment =
                existingSubject.Department ?? "";

            int oldSemester =
                existingSubject.Semester;


            // =====================================================
            // VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(subject.SubjectName))
            {
                ModelState.AddModelError(
                    "SubjectName",
                    "Subject Name is required.");
            }

            if (string.IsNullOrWhiteSpace(subject.SubjectType))
            {
                ModelState.AddModelError(
                    "SubjectType",
                    "Subject Type is required.");
            }

            if (!ModelState.IsValid)
            {
                subject.Id = existingSubject.Id;
                subject.Teacher = oldTeacherName;
                subject.ClassName = oldClassName;
                subject.Department = oldDepartment;
                subject.Semester = oldSemester;
                subject.LectureCount = existingSubject.LectureCount;
                subject.Duration = existingSubject.Duration;

                return View(subject);
            }


            string newSubjectName =
                subject.SubjectName.Trim();

            string newSubjectType =
                subject.SubjectType.Trim();


            // =====================================================
            // AUTOMATIC LECTURE + DURATION
            // =====================================================

            int lectureCount;
            string duration;

            if (newSubjectType.Equals(
                "Theory",
                StringComparison.OrdinalIgnoreCase))
            {
                lectureCount = 2;
                duration = "1 Hour";
            }
            else
            {
                // Practical / Project
                lectureCount = 1;
                duration = "2 Hours";
            }


            // =====================================================
            // CHECK DUPLICATE
            // =====================================================

            bool duplicate = _context.Subjects.Any(s =>
                s.Id != id &&
                s.Teacher == oldTeacherName &&
                s.ClassName == oldClassName &&
                s.Department == oldDepartment &&
                s.Semester == oldSemester &&
                s.SubjectName == newSubjectName &&
                s.SubjectType == newSubjectType
            );

            if (duplicate)
            {
                ModelState.AddModelError(
                    "SubjectName",
                    "This subject already exists for this teacher.");

                subject.Id = existingSubject.Id;
                subject.Teacher = oldTeacherName;
                subject.ClassName = oldClassName;
                subject.Department = oldDepartment;
                subject.Semester = oldSemester;

                return View(subject);
            }


            // =====================================================
            // UPDATE SUBJECT
            // =====================================================

            existingSubject.SubjectName =
                newSubjectName;

            existingSubject.SubjectType =
                newSubjectType;

            existingSubject.LectureCount =
                lectureCount;

            existingSubject.Duration =
                duration;


            // =====================================================
            // FIND TEACHER
            // =====================================================

            var teacher = _context.Teachers
                .FirstOrDefault(t =>
                    t.TeacherName == oldTeacherName);


            if (teacher != null)
            {
                // =================================================
                // FIND OLD ASSIGNMENT
                // =================================================

                var assignment =
                    _context.TeacherAssignments
                    .FirstOrDefault(a =>
                        a.TeacherId == teacher.Id &&
                        a.ClassName == oldClassName &&
                        a.Semester == oldSemester &&
                        a.Subject == oldSubjectName &&
                        a.SubjectType == oldSubjectType);


                if (assignment != null)
                {
                    // =============================================
                    // UPDATE SAME ASSIGNMENT
                    // =============================================

                    assignment.Subject =
                        newSubjectName;

                    assignment.SubjectType =
                        newSubjectType;

                    assignment.LectureCount =
                        lectureCount;

                    assignment.Duration =
                        duration;
                }
                else
                {
                    // =============================================
                    // SAFETY:
                    // If assignment doesn't exist, create it.
                    // =============================================

                    _context.TeacherAssignments.Add(
                        new TeacherAssignment
                        {
                            TeacherId = teacher.Id,
                            ClassName = oldClassName,
                            Semester = oldSemester,
                            Subject = newSubjectName,
                            LectureCount = lectureCount,
                            Duration = duration,
                            SubjectType = newSubjectType
                        });
                }
            }


            // =====================================================
            // SAVE EVERYTHING
            // =====================================================

            _context.SaveChanges();

            TempData["SuccessMessage"] =
                $"Subject '{oldSubjectName}' updated to '{newSubjectName}' successfully.";

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // DELETE SINGLE SUBJECT
        // DELETE SUBJECT + MATCHING TEACHER ASSIGNMENT
        // =========================================================
        // =========================================================
        // DELETE SUBJECT - GET (CONFIRMATION)
        // =========================================================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var subject = _context.Subjects
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }


        // =========================================================
        // DELETE SUBJECT - POST (ACTUAL DELETE)
        // DELETE SUBJECT + MATCHING TEACHER ASSIGNMENT
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var subject = _context.Subjects
                .FirstOrDefault(s => s.Id == id);

            if (subject == null)
            {
                return NotFound();
            }

            var teacher = _context.Teachers
                .FirstOrDefault(t => t.TeacherName == subject.Teacher);

            if (teacher != null)
            {
                var assignments = _context.TeacherAssignments
                    .Where(a =>
                        a.TeacherId == teacher.Id &&
                        a.ClassName == subject.ClassName &&
                        a.Semester == subject.Semester &&
                        a.Subject == subject.SubjectName &&
                        a.SubjectType == subject.SubjectType)
                    .ToList();

                if (assignments.Any())
                {
                    _context.TeacherAssignments.RemoveRange(assignments);
                }
            }

            string deletedSubjectName = subject.SubjectName;

            _context.Subjects.Remove(subject);

            _context.SaveChanges();

            TempData["SuccessMessage"] =
                $"Subject '{deletedSubjectName}' deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // DELETE ALL SUBJECTS
        // SUBJECTS + ALL TEACHER ASSIGNMENTS
        // =========================================================

        // =========================================================
        // DELETE ALL SUBJECTS
        // DELETE ONLY MATCHING TEACHER ASSIGNMENTS
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAll()
        {
            var subjects = _context.Subjects.ToList();

            if (!subjects.Any())
            {
                TempData["ErrorMessage"] =
                    "There are no subjects to delete.";

                return RedirectToAction(nameof(Index));
            }

            int deletedSubjects = subjects.Count;
            int deletedAssignments = 0;

            // =====================================================
            // FIND MATCHING TEACHER ASSIGNMENTS
            // =====================================================

            foreach (var subject in subjects)
            {
                var teacher = _context.Teachers
                    .FirstOrDefault(t =>
                        t.TeacherName == subject.Teacher);

                if (teacher == null)
                {
                    continue;
                }

                var assignments = _context.TeacherAssignments
                    .Where(a =>
                        a.TeacherId == teacher.Id &&
                        a.ClassName == subject.ClassName &&
                        a.Semester == subject.Semester &&
                        a.Subject == subject.SubjectName &&
                        a.SubjectType == subject.SubjectType)
                    .ToList();

                if (assignments.Any())
                {
                    deletedAssignments += assignments.Count;

                    _context.TeacherAssignments
                        .RemoveRange(assignments);
                }
            }

            // =====================================================
            // DELETE ALL SUBJECTS
            // =====================================================

            _context.Subjects.RemoveRange(subjects);

            _context.SaveChanges();

            // =====================================================
            // SUCCESS MESSAGE
            // =====================================================

            TempData["SuccessMessage"] =
                $"{deletedSubjects} subject(s) deleted successfully. " +
                $"{deletedAssignments} matching teacher assignment(s) also removed.";

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // LOAD DROPDOWNS
        // =========================================================

        private void LoadDropdowns()
        {
            ViewBag.Teachers = _context.Teachers
                .OrderBy(t => t.TeacherName)
                .ToList();

            ViewBag.Departments = _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToList();

            ViewBag.Classes = _context.Classes
                .OrderBy(c => c.ClassName)
                .ThenBy(c => c.Semester)
                .ToList();
        }
    }
}