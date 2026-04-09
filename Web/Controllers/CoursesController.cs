using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.Course;

namespace Web.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
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
    }
}
