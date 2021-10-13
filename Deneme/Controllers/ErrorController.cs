using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deneme.Controllers
{
    public class ErrorController : MyBaseController
    {
        // GET: Error
     
        public ActionResult Http400()
        {
            return View();
        }
        public ActionResult Http403()
        {
            return View();
        }
        public ActionResult Http404()
        {
            return View();
        }
        public ActionResult Http500()
        {
            return View();
        }
        public ActionResult Http503()
        {
            return View();
        }
    }
}