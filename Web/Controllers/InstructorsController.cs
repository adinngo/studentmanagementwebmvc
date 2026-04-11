using Data;
using Entities;
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var instructor = await _context.Instructors.
                Include(o => o.OfficeAssignment).
                Include(c => c.CourseAssignments).ThenInclude(c => c.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(ist => ist.ID == id);

            if (instructor == null) return NotFound();

            var allCourses = await _context.Courses.ToListAsync();

            var viewModel = new InstructorEditVM
            {
                ID = instructor.ID,
                LastName = instructor.LastName,
                FirstMidName = instructor.FirstMidName,
                HireDate = instructor.HireDate,
                OfficeLocation = instructor.OfficeAssignment.Location,
                Courses = allCourses.Select(c => new CourseCheckboxViewModel
                {
                    CourseID = c.CourseID,
                    Title = c.Title,
                    IsAssigned = instructor.CourseAssignments.Any(ca => ca.CourseID == c.CourseID)
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstructorEditVM viewModel)
        {
            if (id != viewModel.ID) return NotFound();
            if (ModelState.IsValid)
            {
                var instructorToUpdate = await _context.Instructors
               .Include(o => o.OfficeAssignment)
               .Include(c => c.CourseAssignments)
               .ThenInclude(c => c.Course)
               .FirstOrDefaultAsync(ist => ist.ID == id);

                if (instructorToUpdate == null) return NotFound();

                instructorToUpdate.LastName = viewModel.LastName;
                instructorToUpdate.FirstMidName = viewModel.FirstMidName;
                instructorToUpdate.HireDate = viewModel.HireDate;

                UpdateOfficeAssignment(instructorToUpdate, viewModel.OfficeLocation);
                UpdateCourseAssignments(instructorToUpdate, viewModel.Courses);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            await PopulateAssignedCourseData(viewModel);
            return View(viewModel);
        }

        private async Task PopulateAssignedCourseData(InstructorEditVM viewModel)
        {
            var allCourses = await _context.Courses.ToListAsync();
            var selectedIds = viewModel.Courses.Where(c => c.IsAssigned).Select(c => c.CourseID).ToHashSet();

            viewModel.Courses = allCourses.Select(c => new CourseCheckboxViewModel
            {
                CourseID = c.CourseID,
                Title = c.Title,
                IsAssigned = selectedIds.Contains(c.CourseID)
            }).ToList();
        }

        private void UpdateOfficeAssignment(Instructor instructorToUpdate, string? officeLocation)
        {
            if (string.IsNullOrWhiteSpace(officeLocation))
            {
                instructorToUpdate.OfficeAssignment = null!;
            }
            else
            {
                if (instructorToUpdate.OfficeAssignment == null)
                {
                    instructorToUpdate.OfficeAssignment = new OfficeAssignment
                    {
                        Location = officeLocation
                    };
                }
                else
                {
                    instructorToUpdate.OfficeAssignment.Location = officeLocation;
                }
            }
        }

        private void UpdateCourseAssignments(Instructor instructorToUpdate, List<CourseCheckboxViewModel> courseCheckboxes)
        {
            var currentAssignedIds = instructorToUpdate.CourseAssignments.Select(c => c.CourseID).ToHashSet();
            var selectedIds = courseCheckboxes.Where(c => c.IsAssigned).Select(c => c.CourseID).ToHashSet();

            //thêm những cái mới vào 
            foreach (var course in selectedIds.Except(currentAssignedIds))
            {
                instructorToUpdate.CourseAssignments.Add(new CourseAssignment
                {
                    CourseID = course,
                    InstructorID = instructorToUpdate.ID
                });
            }

            var toRemove = instructorToUpdate.CourseAssignments
                                 .Where(ca => !selectedIds.Contains(ca.CourseID))
                                 .ToList();

            foreach (var ca in toRemove)
            {
                instructorToUpdate.CourseAssignments.Remove(ca);
            }
        }

        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null) return NotFound();
            var viewModel = await _context.Instructors
                .Select(ist => new InstructorViewModel
                {
                    ID = ist.ID,
                    LastName = ist.LastName,
                    FirstMidName = ist.FirstMidName,
                    HireDate = ist.HireDate,
                    Office = ist.OfficeAssignment.Location,
                }).AsNoTracking().FirstOrDefaultAsync(ist => ist.ID == id);
            if (viewModel == null) return NotFound();
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["Error"] = "";
            }
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existingInstructor = await _context.Instructors.FindAsync(id);
            if (existingInstructor == null) return NotFound();
            var departments = await _context.Departments.Where(d => d.InstructorID == id).ToListAsync();
            foreach (var dep in departments)
            {
                dep.InstructorID = null;
            }
            try
            {
                _context.Instructors.Remove(existingInstructor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Delete", new { id, saveChangesError = true });
            }

        }

    }
}
