using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Web.Models.Enrollment;

namespace Web.Models.Course
{
    public class CourseViewModel
    {
        [DisplayName("Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        public string Department { get; set; }

        public IEnumerable<EnrollmentViewModel> EnrollmentViewModels { get; set; }
    }
}
