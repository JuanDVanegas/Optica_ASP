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
using System.Net;

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
            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });
            ViewBag.DTypes = dType;

            ViewBag.UserName = user.UserData.Nombre;
            ViewBag.Entidad = "ABC Opticas";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public ActionResult Historial()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            if (User.IsInRole("Paciente"))
            {
                return View(db.Historial.Where(x => x.Paciente.Id == user.Id).ToList());
            }
            else if (User.IsInRole("Medico"))
            {
                return View(db.Historial.Where(x => x.Medico.Id == user.Id).ToList());
            }
            return View(db.Historial.ToList());
        }
        public async Task<ActionResult> HistorialDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Historial historial = await db.Historial.FindAsync(id);
            if (historial == null)
            {
                return RedirectToAction("Historial");
            }
            return View(historial);
        }
        public ActionResult CreateHistorial()
        {
            List<SelectListItem> dType = new List<SelectListItem>();
            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });
            ViewBag.DTypes = dType;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateHistorial(Historial model)
        {
            var medico = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var paciente = db.Users.Where(x => 
                x.UserData.Documento == model.Paciente.UserData.Documento &&
                x.UserData.TipoDocumento == model.Paciente.UserData.TipoDocumento);
            if (paciente.First() == null)
            {
                ModelState.AddModelError("", "El paciente no existe");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var historial = new Historial
                {
                    Fecha = model.Fecha,
                    Medico = medico,
                    Paciente = paciente.First(),
                    Registro = model.Registro
                };
                db.Historial.Add(historial);
                await db.SaveChangesAsync();
                return RedirectToAction("Historial");
            }
            return View(model);
        }
    }
}