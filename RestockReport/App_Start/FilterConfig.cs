using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RestockReport
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public class AuthorizeActionFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                Controller controller = filterContext.Controller as Controller;

                if (controller != null)
                {
                    if (session["Menu"] == null || session["UserName"]==null || session["Title"] == null || session["UserID"] == null 
                        || session["UserPrimaryID"] == null || session["UserName"] == null
                        )
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                                             RouteValueDictionary(new { controller = "Account", action = "Index" }));
                    }
                }

                base.OnActionExecuting(filterContext);
            }
        }


        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
         filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                                 RouteValueDictionary(new { controller = "Account", action = "Index" }));
                }
            }
        }

    }
}
