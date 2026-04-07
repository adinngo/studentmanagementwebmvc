using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models;


namespace Web.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in _context.Students select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
            }

            students = sortOrder switch
            {
                "name_desc" => students.OrderByDescending(s => s.LastName),
                "date_desc" => students.OrderByDescending(s => s.EnrollmentDate),
                "date" => students.OrderBy(s => s.EnrollmentDate),
                _ => students.OrderBy(s => s.LastName),
            };

            var models = students.Select(s => new StudentViewModel
            {
                ID = s.ID,
                LastName = s.LastName,
                FirstMidName = s.FirstMidName,
                EnrollmentDate = s.EnrollmentDate
            });
            int pageSize = 3;
            return View(await PaginatedList<StudentViewModel>.CreateAsync(models.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.Include(e => e.Enrollments).ThenInclude(c => c.Course).AsNoTracking().FirstOrDefaultAsync(s => s.ID == id);

            if (student == null)
            {
                return NotFound();
            }
            var model = new StudentDetailsViewModel
            {
                ID = student.ID,
                LastName = student.LastName,
                FirstMidName = student.FirstMidName,
                EnrollmentDate = student.EnrollmentDate,
                Enrollments = student.Enrollments.Select(e => new EnrollmentDTO
                {
                    CourseTitle = e.Course.Title,
                    Grade = e.Grade.ToString()
                })
            };
            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = new Student
                    {
                        LastName = model.LastName,
                        FirstMidName = model.FirstMidName,
                        EnrollmentDate = model.EnrollmentDate
                    };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var s = await _context.Students.FindAsync(id);
            if (s == null)
            {
                return NotFound();
            }
            var model = new StudentViewModel
            {
                ID = s.ID,
                LastName = s.LastName,
                FirstMidName = s.FirstMidName,
                EnrollmentDate = s.EnrollmentDate
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentViewModel model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FindAsync(id);

            if (studentToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    studentToUpdate.LastName = model.LastName;
                    studentToUpdate.FirstMidName = model.FirstMidName;
                    studentToUpdate.EnrollmentDate = model.EnrollmentDate;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var s = await _context.Students.FindAsync(id);
            if (s == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
            "Delete failed. Try again, and if the problem persists " +
            "see your system administrator.";
            }

            var model = new StudentViewModel
            {
                ID = s.ID,
                LastName = s.LastName,
                FirstMidName = s.FirstMidName,
                EnrollmentDate = s.EnrollmentDate
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentToDelete = await _context.Students.FindAsync(id);

            if (studentToDelete == null) return NotFound();
            try
            {
                _context.Students.Remove(studentToDelete);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
            }
        }
    }
}
