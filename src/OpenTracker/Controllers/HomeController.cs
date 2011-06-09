using System.Web.Mvc;
using OpenTracker.Core.Account;

namespace OpenTracker.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        [AuthorizeUser]
        public ActionResult Index()
        {
            return View();
        }

    }
}
