using Web.Models.Course;
using Web.Models.Enrollment;

namespace Web.Models.Instructor
{
    public class InstructorIndexVM
    {

        public IEnumerable<InstructorViewModel> InstructorsVM { get; set; }
        public IEnumerable<CourseViewModel> CoursesVM { get; set; }
        public IEnumerable<EnrollmentViewModel> EnrollmentsVM { get; set; }
    }
}
