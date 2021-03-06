﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Optica_ASP.Models;
using System.Data.Entity;

namespace Optica_ASP.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Instancias
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController() { }
        public AccountController(
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
            private set{ _userManager = value; }
        }
        #endregion
        //
        // GET: /Account/Login
        [AunteticateAuthorize]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await  UserManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ModelState.AddModelError("", "No se ha podido encontrar tu cuenta");
                return View(model);
            }
            if (user.Enabled == false)
            {
                ModelState.AddModelError("", "Tu cuenta se encuentra deshabilitada, contacta un administrador.");
                return View(model);
            }
            // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
            // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "ManageAccount");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                default:
                    ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                    return View(model);
            }
        }

        [AunteticateAuthorize]
        [AllowAnonymous]
        public ActionResult RoleRegister()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var role in db.Roles.Where(r => !r.Name.Contains("Admin")))
                list.Add(new SelectListItem { Value = role.Name, Text = role.Name });

            ViewBag.DTypes = list;
            return View();
        }

        [AunteticateAuthorize]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RoleRegister(RoleRegisterViewModel model)
        {
            if (model.UserRole == "Medico")
            {
                return RedirectToAction("RegisterMedico");
            }
            if (model.UserRole == "Paciente")
            {
                return RedirectToAction("Register");
            }
            return View(model);
        }
        // GET: /Account/Register
        [AunteticateAuthorize]
        [AllowAnonymous]
        public ActionResult Register()
        {
            List<SelectListItem> dType = new List<SelectListItem>();

            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });

            ViewBag.DTypes = dType;

            return View();
        }

        [AunteticateAuthorize]
        [AllowAnonymous]
        public ActionResult RegisterMedico()
        {
            List<SelectListItem> dType = new List<SelectListItem>();

            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });

            ViewBag.DTypes = dType;

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            #region documentoyRoles
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles)
                if (role.Name != "Admin")
                {
                    list.Add(new SelectListItem { Value = role.Name, Text = role.Name });
                }
            ViewBag.Roles = list;

            List<SelectListItem> dType = new List<SelectListItem>();

            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });

            ViewBag.DTypes = dType; 
            #endregion

            string roleName = "Paciente";
            var identityUser = await UserManager.FindByEmailAsync(model.Email);
            var userData = await db.UserData.FirstOrDefaultAsync(x => x.Documento == model.Documento && x.TipoDocumento == model.TipoDocumento);
            if(identityUser != null || userData != null)
            {
                ModelState.AddModelError("", "No es posible registrar este usuario.");
                return View(model);
            }
            var user = new ApplicationUser {
                UserName = model.Email,
                Email = model.Email,
                Enabled = true
            };
            user.UserData = new UserData
            {
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                TipoDocumento = model.TipoDocumento,
                Documento = model.Documento,
                FechaNacimiento = model.FechaNacimiento,
                User = user,
                Paciente = new Paciente()
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                result = await UserManager.AddToRoleAsync(user.Id, roleName);
                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", callbackUrl);

                return RedirectToAction("ConfirmEmailSend", "Account");
            }
            AddErrors(result);
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterMedico(RegisterMedicoViewModel model)
        {
            #region documentypes
            List<SelectListItem> dType = new List<SelectListItem>();

            foreach (var documentType in db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });

            ViewBag.DTypes = dType;
            #endregion
            string roleName = "Medico";
            var identityUser = await UserManager.FindByEmailAsync(model.Email);
            var userData = await db.UserData.FirstOrDefaultAsync(x => x.Documento == model.Documento && x.TipoDocumento == model.TipoDocumento);
            if (identityUser != null || userData != null)
            {
                ModelState.AddModelError("", "No es posible registrar este usuario.");
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.Nombre,
                Email = model.Email,
                Enabled = true
            };
            Entity entidad = await db.Entity.FirstOrDefaultAsync(x => x.Nombre == model.NombreEntidad && x.Codigo == model.CodigoEntidad);

            if (entidad != null)
            {
                db.Entity.Attach(entidad);
                user.UserData = new UserData
                {
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    TipoDocumento = model.TipoDocumento,
                    Documento = model.Documento,
                    FechaNacimiento = model.FechaNacimiento,
                    User = user,
                    Medico = new Medico { EntidaId = entidad.EntityId }
                };
            }
            else
            {
                ModelState.AddModelError("", "Las Credenciales de la entidad Son Incorrectas");
                return View(model);
            }
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                result = await UserManager.AddToRoleAsync(user.Id, roleName);
                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", callbackUrl);

                return RedirectToAction("ConfirmEmailSend", "Account");
            }
            AddErrors(result);
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmailSend()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", callbackUrl);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}