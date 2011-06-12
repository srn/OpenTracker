using System.Web.Mvc;

namespace OpenTracker.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View("Http404");
        }

        public ActionResult General()
        {
            return View("Http404");
        }


        public ActionResult Http404()
        {
            return View();
        }

        public ActionResult Http403()
        {
            return View();
        }

    }
}
