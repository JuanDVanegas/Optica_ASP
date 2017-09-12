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
using PagedList;
using Microsoft.Owin.Security;

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
            if (User.IsInRole("Medico"))
            {
                var entidad = user.UserData.Medico.Entidad.Nombre;
                ViewBag.Entidad = entidad;
            }
            return View();
        }

        public async Task<ActionResult> UpdateData(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Su contraseña se ha cambiado."
                : message == ManageMessageId.Error ? "Se ha producido un error."
                : "";
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            string roleName = UserManager.GetRoles(userId).First();
            var model = new UpdateViewModel(user, roleName);

            var dType = user.UserData.TipoDocumento;
            ViewBag.DTypes = dType;

            ViewBag.UserName = user.UserData.Nombre;
            if (User.IsInRole("Medico"))
            {
                var entidad = user.UserData.Medico.Entidad.Nombre;
                ViewBag.Entidad = entidad;
            }

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
                user.EmailConfirmed = false;
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", callbackUrl);
            }

            user.UserData.Nombre = model.Nombre;
            user.UserData.Apellido = model.Apellido;
            user.UserName = model.Nombre + " " + model.Apellido;

            await UserManager.UpdateAsync(user);
            return RedirectToAction("UpdateData");
        }

        public ActionResult Historial(string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            var user = UserManager.FindById(User.Identity.GetUserId());
            var historial = db.Historial.ToList();
            if (User.IsInRole("Paciente"))
            {
                historial = user.UserData.Paciente.Historial;
                if (!string.IsNullOrEmpty(searchString))
                {
                    historial = historial.Where(h => h.Medico.UserData.Nombre.Contains(searchString)
                                                   || h.Medico.Entidad.Nombre.Contains(searchString)).ToList();
                }
            }
            if (User.IsInRole("Medico"))
            {
                historial = user.UserData.Medico.Historial;
                if (!string.IsNullOrEmpty(searchString))
                {
                    historial = historial.Where(h => h.Paciente.UserData.Nombre.Contains(searchString)
                                                     || h.Paciente.UserData.Documento.Contains(searchString)).ToList();
                }
                var entidad = user.UserData.Medico.Entidad.Nombre;
                ViewBag.Entidad = entidad;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(historial.ToPagedList(pageNumber, pageSize));
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
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (User.IsInRole("Medico"))
            {
                var entidad = user.UserData.Medico.Entidad.Nombre;
                ViewBag.Entidad = entidad;
            }
            return View(historial);
        }

        [Authorize(Roles = "Medico")]
        public ActionResult CreateHistorial()
        {
            List<SelectListItem> dType = new List<SelectListItem>();
            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });
            ViewBag.DTypes = dType;
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (User.IsInRole("Medico"))
            {
                var entidad = user.UserData.Medico.Entidad.Nombre;
                ViewBag.Entidad = entidad;
            }
            return View();
        }

        [Authorize(Roles = "Medico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateHistorial(Historial model)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (User.IsInRole("Medico"))
            {
                var entidad = user.UserData.Medico.Entidad.Nombre;
                ViewBag.Entidad = entidad;
            }
            List<SelectListItem> dType = new List<SelectListItem>();
            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });
            ViewBag.DTypes = dType;
            var medico = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var paciente = db.Paciente.FirstOrDefault(x =>
                x.UserData.Documento == model.Paciente.UserData.Documento &&
                x.UserData.TipoDocumento == model.Paciente.UserData.TipoDocumento);
            if (paciente == null)
            {
                ModelState.AddModelError("", "No se encontro el paciente");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                if (medico.Id == paciente.UserData.User.Id)
                {
                    ModelState.AddModelError("", "No se encontro el paciente");
                    return View(model);
                }
                var registro = new Registro
                {
                    Descripcion = model.Registro.Descripcion,
                    Resultado = model.Registro.Resultado,
                    Tratamiento = model.Registro.Tratamiento
                };
                var historial = new Historial
                {
                    Fecha = DateTime.Now,
                    MedicoId = medico.UserData.Medico.MedicoId,
                    PacienteId = paciente.PacienteId,
                    Registro = registro
                };
                db.Historial.Add(historial);
                await db.SaveChangesAsync();
                return RedirectToAction("Historial");
            }
            return View(model);
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("UpdateData", "ManageAccount", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usan para protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}