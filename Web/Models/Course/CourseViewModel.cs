using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web.Models.Enrollment;

namespace Web.Models.Course
{
    public class CourseViewModel
    {
        [DisplayName("Number")]
        [Range(1000, 5000)]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(1, 5)]
        public int Credits { get; set; }

        public string? Department { get; set; }

        public int DepartmentID { get; set; }
        public IEnumerable<EnrollmentViewModel>? EnrollmentViewModels { get; set; }

        public IEnumerable<CourseAssignmentVM>? CourseAssignmentsVM { get; set; }
    }
}
