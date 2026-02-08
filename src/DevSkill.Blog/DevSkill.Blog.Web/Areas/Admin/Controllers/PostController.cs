using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Blog.Web.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
