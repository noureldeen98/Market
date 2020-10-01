using Emarket.Extensions;
using Emarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emarket.Controllers
{
    public class HomeController : Controller
    {
        private EmarketDBEntities db = new EmarketDBEntities();
        public HomeController()
        {
            ViewBag.ListCart = db.Carts.ToList();
        }
        public ActionResult Index()
        {
            //this.AddNotification("THis is the sucess notification", NotificationType.SUCCESS);
            //this.AddNotification("THis is the info notification", NotificationType.INFO);
            //this.AddNotification("THis is the error notification", NotificationType.ERROR);
            //this.AddNotification("THis is the warning notification", NotificationType.WARNING);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}