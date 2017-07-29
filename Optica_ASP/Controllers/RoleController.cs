using Microsoft.AspNet.Identity.Owin;
using Optica_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Optica_ASP.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        public RoleController() { }
        public RoleController(ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager; 
        }
        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }
            private set { _roleManager = value; }
        }
        // GET: Role
        public ActionResult Index()
        {
            List<RoleViewModel> List = new List<RoleViewModel>();
            return View();
        }
    }
}