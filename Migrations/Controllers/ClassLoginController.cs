
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutomaticCollegeTimetableGenerator.Models;
using AutomaticCollegeTimetableGenerator.Data;

public class ClassLoginController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClassLoginController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: STUDENTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Students.ToListAsync());
    }

    // GET: STUDENTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: STUDENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: STUDENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    string username,
    string password,
    string className,
    string department)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(className))
        {
            ModelState.AddModelError(
                "",
                "Please fill all required fields.");

            return View(new Student
            {
                Username = username,
                Password = password,
                ClassName = className,
                Department = department
            });
        }

        var existingAccount = await _context.Students
            .FirstOrDefaultAsync(s => s.Username == username);

        if (existingAccount != null)
        {
            ModelState.AddModelError(
                "",
                "This username already exists.");

            return View(new Student
            {
                Username = username,
                Password = password,
                ClassName = className,
                Department = department
            });
        }

        // ==========================================
        // GET CLASS INFORMATION
        // ==========================================

        var classData = await _context.Classes
            .FirstOrDefaultAsync(c => c.ClassName == className);

        if (classData == null)
        {
            ModelState.AddModelError(
                "",
                "Selected class was not found in Manage Classes.");

            return View(new Student
            {
                Username = username,
                Password = password,
                ClassName = className,
                Department = department
            });
        }

        // ==========================================
        // CREATE CLASS LOGIN
        // ==========================================

        var student = new Student
        {
            StudentName = classData.ClassName,
            Username = username,
            Password = password,
            ClassName = classData.ClassName,
            Department = classData.Department,
            Semester = classData.Semester
        };

        _context.Students.Add(student);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: STUDENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Username,Password,ClassName,Department,Semester")] Student student)
    {
        if (id != student.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: STUDENTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.Id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: STUDENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int? id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
    // ================= STUDENT LOGIN =================

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(StudentLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(s =>
                s.Username == model.Username &&
                s.Password == model.Password);

        if (student == null)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        // Store the logged-in class information in Session
        HttpContext.Session.SetString("StudentUsername", student.Username ?? "");
        HttpContext.Session.SetString("StudentClassName", student.ClassName ?? "");
        HttpContext.Session.SetString("StudentDepartment", student.Department ?? "");
        HttpContext.Session.SetString("StudentSemester", student.Semester.ToString());

        return RedirectToAction("SelectSemester", "ClassLogin");
    }
    // ================= CLASS TIMETABLE =================

    // ================= CLASS TIMETABLE =================

    public async Task<IActionResult> Timetable()
    {
        var className = HttpContext.Session.GetString("StudentClassName");
        var semesterString = HttpContext.Session.GetString("StudentSelectedSemester");

        if (string.IsNullOrEmpty(className))
        {
            return RedirectToAction("Login");
        }

        if (string.IsNullOrEmpty(semesterString))
        {
            return RedirectToAction("SelectSemester");
        }

        int semester = int.Parse(semesterString);

        var timetable = await _context.Timetables
            .Where(t =>
                t.ClassName == className &&
                t.Semester == semester)
            .ToListAsync();

        return View(timetable);
    }
    // ================= SELECT SEMESTER =================

    [HttpGet]
    public async Task<IActionResult> SelectSemester()
    {
        var className = HttpContext.Session.GetString("StudentClassName");

        if (string.IsNullOrEmpty(className))
        {
            return RedirectToAction(nameof(Login));
        }

        var semesters = await _context.Classes
            .Where(c => c.ClassName == className)
            .Select(c => c.Semester)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        ViewBag.Semesters = semesters;

        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SelectSemester(int semester)
    {
        var className = HttpContext.Session.GetString("StudentClassName");

        if (string.IsNullOrEmpty(className))
        {
            return RedirectToAction(nameof(Login));
        }

        var validSemester = _context.Classes
            .Any(c => c.ClassName == className &&
                     c.Semester == semester);

        if (!validSemester)
        {
            TempData["ErrorMessage"] = "Invalid semester selected.";
            return RedirectToAction(nameof(SelectSemester));
        }

        HttpContext.Session.SetString(
            "StudentSelectedSemester",
            semester.ToString());

        return RedirectToAction(nameof(Timetable));
    }
}
