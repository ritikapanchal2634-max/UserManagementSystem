using Microsoft.AspNetCore.Mvc;

namespace UserManagementSystem.Controllers
{
    public class Practicle2Controller : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int limit)
        {
            // Call method to get result
            var result = PrintValues(limit);
            ViewBag.Result = result;
            return View();
        }
        public static List<object> PrintValues(int limit)
        {
            if (limit <= 0)
            {
                return new List<object>();
            }
    
            var result = new List<object>();

            for (int i = 1; i <= limit; i++)
            {
                if (i % 3 == 0 && i % 5 == 0)
                {
                    result.Add("Height8");
                }
                else if (i % 3 == 0)
                    result.Add("Hei");
                else if (i % 5 == 0)
                    result.Add("ght");
                else
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}
