using System.ComponentModel.DataAnnotations;

namespace Web.Models.Instructor
{
    public class InstructorEditVM
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstMidName; }
        }

        public List<CourseCheckboxViewModel> Courses { get; set; }
        public string? OfficeLocation { get; set; }
    }

    public class CourseCheckboxViewModel
    {
        public int CourseID { get; set; }
        public string Title { get; set; }
        public bool IsAssigned { get; set; }

    }
}
