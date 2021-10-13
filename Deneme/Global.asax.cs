using Deneme.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Deneme
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();
            HttpException httpException = exception as HttpException;
            RouteData route = new RouteData();
            route.Values.Add("Controller", "Error");
            if(httpException!=null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        route.Values.Add("action", "Http404");
                        break;
                    case 500:
                        route.Values.Add("action", "Http500");
                        break;
                }
                Server.ClearError();
                Response.TrySkipIisCustomErrors = true;

            }
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), route));

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
