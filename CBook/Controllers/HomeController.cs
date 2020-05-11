using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CBook.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
          //ViewBag.EmailID = TempData["emailID"].ToString().Split('@').First();
          ViewBag.EmailID = TempData["emailID"];
          return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
