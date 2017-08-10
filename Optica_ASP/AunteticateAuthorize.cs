using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Optica_ASP
{
    public class AunteticateAuthorize : ActionFilterAttribute
    {
        IAuthenticationManager Authentication
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Authentication.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "ManageAccount", action = "Index" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}