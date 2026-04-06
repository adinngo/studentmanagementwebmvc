using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class StudentViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "LastName is Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "LastName is FirstMidName")]
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

    }
}
