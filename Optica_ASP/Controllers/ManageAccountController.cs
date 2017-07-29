using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Optica_ASP.Controllers
{   
    [Authorize]
    public class ManageAccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}