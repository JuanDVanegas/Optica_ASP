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
        #region Instancias
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();
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
        #endregion

        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Entidad = "ABC Opticas";
            return View();
        }

        public async Task<ActionResult> UpdateData()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            string roleName = UserManager.GetRoles(userId).First();
            var model = new UpdateViewModel(user, roleName);

            List<SelectListItem> dType = new List<SelectListItem>();
            dType.Add(new SelectListItem { Value = model.TipoDocumento, Text = model.TipoDocumento });
            dType.Add(new SelectListItem { Value = "Cedula de Ciudadania", Text = "Cedula de Ciudadania" });
            dType.Add(new SelectListItem { Value = "Tarjeta de Identidad", Text = "Tarjeta de Identidad" });
            ViewBag.DTypes = dType;

            ViewBag.UserName = user.UserData.First().Nombre;
            ViewBag.Entidad = "ABC Opticas";

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateData(UpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", callbackUrl);
            }

            user.UserData.Nombre = model.Nombre;
            user.UserData.Apellido = model.Apellido;
            user.UserData.TipoDocumento = model.TipoDocumento;
            user.UserData.Documento = model.Documento;
            user.UserData.FechaNacimiento = model.FechaNacimiento;
            user.UserName = model.Nombre;

            await UserManager.UpdateAsync(user);
            return RedirectToAction("UpdateData");
        }

        //public async Task<ActionResult> Historial(HistorialViewModel model)
        //{
        //    var Historial = from historial in db.Historial
        //                    where historial == model.NombreEntidad && entity.Codigo == model.CodigoEntidad
        //                    select historial;
        //    if (Historial.First().Id != null)
        //    { }
        //    return View(model);
        //}
    }
}