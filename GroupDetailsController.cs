using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace hack_together_groups_manager
{
    public class GroupDetailsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string Id)
        {
            var user = 4;
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
    }
}
