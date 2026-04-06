namespace Web.Models
{
    public class StudentDetailsViewModel
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public IEnumerable<EnrollmentDTO> Enrollments { get; set; }
    }
    public class EnrollmentDTO
    {
        public string? CourseTitle { get; set; }
        public string? Grade { get; set; }
    }
}
