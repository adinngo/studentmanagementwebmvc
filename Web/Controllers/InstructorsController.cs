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
        public async Task<IActionResult> Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexVM();
            viewModel.InstructorsVM = await _context.Instructors.Select(p => new InstructorViewModel
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
            }).AsNoTracking().ToListAsync();

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

        //public async Task<IActionResult> Select(int? id, int? courseID)
        //{
        //    if (id == null) return NotFound();

        //    var viewModel = new InstructorIndexData();
        //    var instructor = _context.Instructors.Select(p => new InstructorViewModel
        //    {
        //        ID = p.ID,
        //        LastName = p.LastName,
        //        FirstMidName = p.FirstMidName,
        //        HireDate = p.HireDate,
        //        Office = p.OfficeAssignment.Location,
        //        Courses = p.CourseAssignments.Select(c => new CourseAssignmentDTO
        //        {
        //            CourseID = c.CourseID,
        //            CourseTitle = c.Course.Title
        //        })
        //    }).Single(i => i.ID == id);

        //    if (instructor == null)
        //    {
        //        return NotFound();
        //    }
        //    viewModel.courses = instructor.Courses.Select(c => new CourseViewModel
        //    {

        //    })

        //    return View();
        //}
    }
}
