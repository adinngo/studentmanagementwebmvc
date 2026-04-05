using Microsoft.AspNetCore.Mvc;
using Services;


namespace Web.Controllers
{
    public class StudentsController : Controller
    {
        private StudentService _service;

        public StudentsController(StudentService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
