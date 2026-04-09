using System.ComponentModel.DataAnnotations;

namespace Web.Models.Enrollment
{
    public class EnrollmentViewModel
    {
        public string StudentFullName { get; set; }
        public string? CourseTitle { get; set; }

        [DisplayFormat(NullDisplayText = "No grade")]
        public string? Grade { get; set; }
    }
}
