using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers

{
    [ApiController]
    [Route("/Home")]
    public class HomeController : Controller{
        // Action method for the Home Page
        public ActionResult Index()
        {
            return Content("1337 Team Home Page!!!"); 
        }
    }
}


