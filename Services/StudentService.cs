using Data;

namespace Services
{
    public class StudentService
    {
        private SchoolContext _schoolContext;

        public StudentService(SchoolContext schoolContext)
        {
            _schoolContext = schoolContext;
        }
    }


}
