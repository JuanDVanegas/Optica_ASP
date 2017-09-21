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
        private ApplicationDbContext _db = new ApplicationDbContext();
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
            ViewBag.Nombre = user.UserData.Nombre + " " + user.UserData.Apellido;
            ViewBag.Entidad = user.UserData.Medico == null ? "": user.UserData.Medico.Entidad.Nombre;
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
            ViewBag.EmailConfirmed = user.EmailConfirmed;
            ViewBag.Nombre = user.UserData.Nombre + " " + user.UserData.Apellido;
            ViewBag.Entidad = user.UserData.Medico == null ? "" : user.UserData.Medico.Entidad.Nombre;

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
            user.UserName = model.Email;

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
            var historial = _db.Historial.ToList();
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
            }
            ViewBag.Nombre = user.UserData.Nombre + " " + user.UserData.Apellido;
            ViewBag.Entidad = user.UserData.Medico == null ? "" : user.UserData.Medico.Entidad.Nombre;
            var pageSize = 5;
            var pageNumber = (page ?? 1);
            return View(historial.ToPagedList(pageNumber, pageSize));
        }
        public async Task<ActionResult> HistorialDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Historial historial = await _db.Historial.FindAsync(id);
            if (historial == null)
            {
                return RedirectToAction("Historial");
            }
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = user.UserData.Nombre + " " + user.UserData.Apellido;
            ViewBag.Entidad = user.UserData.Medico.Entidad.Nombre;
            return View(historial);
        }

        [Authorize(Roles = "Medico")]
        public ActionResult CreateHistorial()
        {
            List<SelectListItem> dType = new List<SelectListItem>();
            foreach (var documentType in _db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });
            ViewBag.DTypes = dType;
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = user.UserData.Nombre + " " + user.UserData.Apellido;
            ViewBag.Entidad = user.UserData.Medico == null ? "" : user.UserData.Medico.Entidad.Nombre;
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
            foreach (var documentType in _db.DocumentType)
                dType.Add(new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre });
            ViewBag.DTypes = dType;
            var medico = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var paciente = _db.Paciente.FirstOrDefault(x =>
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
                _db.Historial.Add(historial);
                await _db.SaveChangesAsync();
                return RedirectToAction("Historial");
            }
            return View(model);
        }
        public ActionResult ChangePassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = user.UserData.Nombre + " " + user.UserData.Apellido;
            ViewBag.Entidad = user.UserData.Medico == null ? "" : user.UserData.Medico.Entidad.Nombre;
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

        [Authorize(Roles = "Admin")]
        public ActionResult AdminUsers(string search, int? page)
        {
            if (search != null)
            {
                page = 1;
            }
            var users = _db.Users.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                users = _db.Users.Where(x => x.Email.Contains(search)  || x.UserData.Documento.Contains(search)).ToList();
            }
            var myUser = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = myUser.UserData.Nombre + " " + myUser.UserData.Apellido;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminUpdateUser(string id, string message)
        {
            ViewBag.Message = message;
            var myUser = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = myUser.UserData.Nombre + " " + myUser.UserData.Apellido;
            var user = await UserManager.FindByIdAsync(id);
            var roleName = UserManager.GetRoles(id).First();
            var entitys = _db.Entity.Select(entity => new SelectListItem { Value = entity.EntityId, Text = entity.Nombre }).ToList();
            var dType = _db.DocumentType.Select(documentType => new SelectListItem {Value = documentType.Nombre, Text = documentType.Nombre}).ToList();
            ViewBag.DTypes = dType;
            ViewBag.Role = roleName;
            var model = new UpdateViewModel(user, roleName);
            ViewBag.Entity = entitys;
            ViewBag.Enabled = user.Enabled;
            TempData["Id"] = id;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminUpdateUser(UpdateViewModel model)
        {
            string id = TempData["id"].ToString();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByIdAsync(id);

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
            user.UserData.TipoDocumento = model.TipoDocumento;
            user.UserData.Documento = model.Documento;
            user.UserData.FechaNacimiento = model.FechaNacimiento;
            user.UserName = model.Email;

            await UserManager.UpdateAsync(user);
            return RedirectToAction("AdminUsers");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AdminEnabledUser(bool enabled)
        {
            string id = TempData["id"].ToString();
            var user = await UserManager.FindByIdAsync(id);
            user.Enabled = enabled;
            var myUser = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = myUser.UserData.Nombre + " " + myUser.UserData.Apellido;
            await UserManager.UpdateAsync(user);
            return RedirectToAction("AdminUsers");
        }
        
        [Authorize(Roles = "Admin")]
        public ActionResult AdminDeleteUser(string email)
        {
            var user = UserManager.FindByEmail(email);
            var historial =
                _db.Historial.FirstOrDefault(x => x.MedicoId == user.UserData.Medico.MedicoId || 
                                            x.PacienteId == user.UserData.Paciente.PacienteId);
            if (historial != null)
            {
                return RedirectToAction("AdminUpdateUser", "ManageAccount", new {id = user.Id, message = "No es posible eliminar el usuario."});
            }
            return RedirectToAction("AdminUsers");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminAddUser()
        {
            var myUser = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = myUser.UserData.Nombre + " " + myUser.UserData.Apellido;
            var roles = _db.Roles.Select(role => new SelectListItem { Value = role.Name, Text = role.Name }).ToList();
            var dType = _db.DocumentType.Select(documentType => new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre }).ToList();
            var entitys = _db.Entity.Select(entity => new SelectListItem { Value = entity.EntityId, Text = entity.Nombre }).ToList();
            ViewBag.Roles = roles;
            ViewBag.DType = dType;
            ViewBag.Entitys = entitys;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AdminAddUser(AdminAddUserViewModel model)
        {
            var roles = _db.Roles.Select(role => new SelectListItem { Value = role.Name, Text = role.Name }).ToList();
            var dType = _db.DocumentType.Select(documentType => new SelectListItem { Value = documentType.Nombre, Text = documentType.Nombre }).ToList();
            var entitys = _db.Entity.Select(entity => new SelectListItem { Value = entity.EntityId, Text = entity.Nombre }).ToList();
            ViewBag.Roles = roles;
            ViewBag.DType = dType;
            ViewBag.Entitys = entitys;

            var identityUser = await UserManager.FindByEmailAsync(model.Email);
            var userData = await _db.UserData.FirstOrDefaultAsync(x => x.Documento == model.Documento && x.TipoDocumento == model.TipoDocumento);
            if (identityUser != null || userData != null)
            {
                ModelState.AddModelError("", "No es posible registrar este usuario.");
                return View(model);
            }
            var user = new ApplicationUser
            {
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
            };
            if (model.RoleName=="Paciente")
            {
                user.UserData.Paciente = new Paciente();
            }
            if (model.RoleName == "Medico")
            {
                var entity = _db.Entity.FirstOrDefault(x => x.Nombre == model.NombreEntidad);
                if (entity != null)
                {
                    user.UserData.Medico = new Medico { EntidaId = entity.EntityId };
                }
                else
                {
                    ModelState.AddModelError("","Problema al validar la entidad");
                    return View(model);
                }
            }
            var result = await UserManager.CreateAsync(user, model.Documento);
            if (result.Succeeded)
            {
                result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
                // Para obtener más información sobre cómo habilitar la confirmación de cuenta y el restablecimiento de contraseña, visite http://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", callbackUrl);

                return RedirectToAction("AdminUsers", "ManageAccount");
            }
            AddErrors(result);
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminChangePassword(string email)
        {
            var myUser = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.Nombre = myUser.UserData.Nombre + " " + myUser.UserData.Apellido;
            TempData["email"] = email;
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminChangePassword(AdminChangePasswordModel model)
        {
            string email = TempData["email"].ToString();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(email);
            var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id,token,model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("AdminUpdateUser", "ManageAccount", new { id = user.Id});
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