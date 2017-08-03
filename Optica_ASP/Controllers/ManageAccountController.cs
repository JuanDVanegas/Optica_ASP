using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Optica_ASP.Models;

namespace Optica_ASP.Controllers
{
    [Authorize]
    public class ManageAccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        public ManageAccountController() { }
        public ManageAccountController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }
        public ApplicationRoleManager RoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }
            private set { _roleManager = value; }
        }
        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        public ActionResult Index()
        {
            ViewBag.Id = User.Identity.GetUserId();
            return View();
        }

        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<ActionResult> CreateRegistry(RegisterViewModel model)
        //{
        //    var role = new ApplicationRole { Name = model.Name };
        //    await RoleManager.CreateAsync(role);
        //    return RedirectToAction("Index");
        //}

        public async Task<ActionResult> UpdateData(string id)
        {
            var model = new UpdateViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                var user = await UserManager.FindByIdAsync(id);
                var role = await RoleManager.Roles;
                model = new UpdateViewModel(user);
            }

            List<SelectListItem> dType = new List<SelectListItem>();
            dType.Add(new SelectListItem { Value = model.TipoDocumento, Text = model.TipoDocumento });
            dType.Add(new SelectListItem { Value = "Cedula de Ciudadania", Text = "Cedula de Ciudadania" });
            dType.Add(new SelectListItem { Value = "Tarjeta de Identidad", Text = "Tarjeta de Identidad" });
            ViewBag.DTypes = dType;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateData(UpdateViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            user.UserData.Add(new UserData
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                TipoDocumento = model.TipoDocumento,
                Documento = model.Documento,
                FechaNacimiento = model.FechaNacimiento
            });
            await UserManager.UpdateAsync(user);
            return RedirectToAction("UpdateData");
        }

        //public async Task<ActionResult> Details(string id)
        //{
        //    var role = await RoleManager.FindByIdAsync(id);
        //    return View(new RoleViewModel(role));
        //}

        //public async Task<ActionResult> Delete(string id)
        //{
        //    var role = await RoleManager.FindByIdAsync(id);
        //    return View(new RoleViewModel(role));
        //}

        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    var role = await RoleManager.FindByIdAsync(id);
        //    await RoleManager.DeleteAsync(role);
        //    return RedirectToAction("Index");
        //}

    }
}