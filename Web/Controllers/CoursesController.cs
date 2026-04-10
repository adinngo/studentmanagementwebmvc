using Data;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models.Course;
using Web.Models.Enrollment;

namespace Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        private void PopulateDepartmentDropDownList(object? selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   select d;
            ViewBag.DepartmentID = new SelectList(departmentsQuery.AsNoTracking(), "DepartmentID", "Name", selectedDepartment);
        }
        public async Task<IActionResult> Index()
        {
            var model = _context.Courses.Select(c => new CourseViewModel
            {
                CourseID = c.CourseID,
                Title = c.Title,
                Credits = c.Credits,
                Department = c.Department.Name
            });

            return View(await model.AsNoTracking().ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.Courses.Select(c => new CourseViewModel
            {
                CourseID = c.CourseID,
                Title = c.Title,
                Credits = c.Credits,
                Department = c.Department.Name,
                EnrollmentViewModels = c.Enrollments.Select(e => new EnrollmentViewModel
                {
                    StudentFullName = e.Student.FullName
                }),
                CourseAssignmentsVM = c.CourseAssignments.Select(c => new CourseAssignmentVM
                {
                    InstructorFullName = c.Instructor.FullName
                })
            }).AsNoTracking().FirstOrDefaultAsync(c => c.CourseID == id);

            if (model == null) return NotFound();

            return View(model);
        }

        public IActionResult Create()
        {
            PopulateDepartmentDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = new Course
                    {
                        CourseID = model.CourseID,
                        Title = model.Title,
                        Credits = model.Credits,
                        DepartmentID = model.DepartmentID
                    };
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }

            }

            PopulateDepartmentDropDownList(model.DepartmentID);
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var model = new CourseViewModel
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentID = course.DepartmentID
            };

            PopulateDepartmentDropDownList(model.DepartmentID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseViewModel model)
        {
            if (id != model.CourseID)
            {
                return NotFound();
            }
            var courseToUpdate = await _context.Courses.FindAsync(id);

            if (courseToUpdate == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    courseToUpdate.CourseID = model.CourseID;
                    courseToUpdate.Title = model.Title;
                    courseToUpdate.Credits = model.Credits;
                    courseToUpdate.DepartmentID = model.DepartmentID;

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            PopulateDepartmentDropDownList(model.DepartmentID);
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null) return NotFound();

            var model = await _context.Courses.Select(c => new CourseViewModel
            {
                CourseID = c.CourseID,
                Title = c.Title,
                Credits = c.Credits,
                Department = c.Department.Name
            }).AsNoTracking().FirstOrDefaultAsync(c => c.CourseID == id);

            if (model == null) return NotFound();

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "You can't delete this course because It already have enrollment students";
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courseToDelete = await _context.Courses.FindAsync(id);

            if (courseToDelete == null) return NotFound();
            try
            {
                _context.Courses.Remove(courseToDelete);
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
