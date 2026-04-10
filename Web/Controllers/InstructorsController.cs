using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.Course;
using Web.Models.Enrollment;
using Web.Models.Instructor;

namespace Web.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int? courseID, int? pageNumber, string sortOrder, string currentFilter, string searchString)
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

            var viewModel = new InstructorIndexVM();
            var instructors = _context.Instructors.Select(p => new InstructorViewModel
            {
                ID = p.ID,
                LastName = p.LastName,
                FirstMidName = p.FirstMidName,
                HireDate = p.HireDate,
                Office = p.OfficeAssignment.Location,
                Courses = p.CourseAssignments.Select(c => new CourseViewModel
                {
                    CourseID = c.CourseID,
                    Title = c.Course.Title

                })
            });

            if (!string.IsNullOrEmpty(searchString))
            {
                instructors = instructors.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
            }

            instructors = sortOrder switch
            {
                "name_desc" => instructors.OrderByDescending(s => s.LastName),
                "date_desc" => instructors.OrderByDescending(s => s.HireDate),
                "date" => instructors.OrderBy(s => s.HireDate),
                _ => instructors.OrderBy(s => s.LastName),
            };

            int pageSize = 2;
            ViewBag.CurrentPage = pageNumber;
            viewModel.InstructorsVM = await PaginatedList<InstructorViewModel>.CreateAsync(instructors.AsNoTracking(), pageNumber ?? 1, pageSize);

            if (id != null)
            {
                ViewData["InstructorID"] = id.Value;

                viewModel.CoursesVM = await _context.CourseAssignments
                    .Where(c => c.InstructorID == id.Value).Select(c => new CourseViewModel
                    {
                        CourseID = c.CourseID,
                        Title = c.Course.Title,
                        Department = c.Course.Department.Name
                    }).AsNoTracking().ToListAsync();
            }

            if (courseID != null)
            {
                ViewData["CourseID"] = courseID.Value;

                viewModel.EnrollmentsVM = await _context.Enrollments.
                    Where(e => e.CourseID == courseID.Value).Select(e => new EnrollmentViewModel
                    {
                        StudentFullName = e.Student.FullName,
                        Grade = e.Grade.ToString()
                    }).AsNoTracking().ToListAsync();

            }


            return View(viewModel);
        }


    }
}
