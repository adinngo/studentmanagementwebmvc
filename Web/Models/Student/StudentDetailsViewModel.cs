using Web.Models.Enrollment;

namespace Web.Models.Student
{
    public class StudentDetailsViewModel
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public IEnumerable<EnrollmentViewModel> Enrollments { get; set; }
    }

}
