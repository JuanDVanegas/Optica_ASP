using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public async Task<ActionResult> UpdateData()
        {
            var userId = User.Identity.GetUserId();
            var model = new UpdateViewModel();
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await UserManager.FindByIdAsync(userId);
                string roleName = UserManager.GetRoles(userId).First();
                model = new UpdateViewModel(user, roleName);
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
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user.Email != model.Email)
            {
                user.EmailConfirmed = false;
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", callbackUrl);
            }

            user.UserData.First().Nombre = model.Nombre;
            user.UserData.First().Apellido = model.Apellido;
            user.UserData.First().TipoDocumento = model.TipoDocumento;
            user.UserData.First().Documento = model.Documento;
            user.UserData.First().FechaNacimiento = model.FechaNacimiento;

            await UserManager.UpdateAsync(user);
            return RedirectToAction("UpdateData");
        }
    }
}