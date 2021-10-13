using Deneme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Deneme.Controllers
{
    public class CompanyOperationsController : MyBaseController
    {

        [Authorize]
        public ActionResult COP_View()
        {

            return View();
        }

        [Authorize]
        public ActionResult COP()
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                //string[] Keys = User.Identity.Name.Split(',');
                    int UserID = Convert.ToInt32(User.Identity.Name);
                    var company = dc.Company.Where(a => a.KullanıcıID == UserID).ToList();
                    return Json(new { data = company }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_COP(int id = 0)
        {
            if (id == 0)
                return View(new Company());
            else
            {
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.Company.Where(x => x.CompanyId == id).FirstOrDefault<Company>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_COP(Company company)
        {
            //string[] Keys = User.Identity.Name.Split(',');
            int UserID = Convert.ToInt32(User.Identity.Name);
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                company.KullanıcıID = UserID;
                if (company.CompanyId == 0)
                {
                    db.Company.Add(company);
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(company).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.UpdatedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete_COP(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                Company company = db.Company.Where(x => x.CompanyId == id).FirstOrDefault<Company>();
                db.Company.Remove(company);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Select_COP(int id)
        {
            string isAdmin = Server.HtmlEncode(Request.Cookies["UserOp"]["isAdmin"]);
            
            string Yetki = Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]);
            if (isAdmin == "0")
            {
                string SubUserId = Server.HtmlEncode(Request.Cookies["UserOp"]["SubId"]);
                Response.Cookies["UserOp"]["SubId"] = SubUserId;
            }
            Response.Cookies["UserOp"]["CompanyId"] = id.ToString();

            Response.Cookies["UserOp"]["Yetki"] = Yetki;
            Response.Cookies["UserOp"]["isAdmin"] = isAdmin;
            Response.Cookies["UserOp"].Expires = DateTime.Now.AddMinutes(20);
            return Json(new { success = true, message = Deneme.Resource.CompanySelectedSuccesfully }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult SubCOP_View()
        {

            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult SubCOP()
        {

            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                int SubId = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["UserOp"]["SubId"]));
                var yetki = db.Yetki.Where(x => x.UserID == SubId).ToList();
                List<Company> company = new List<Company>();
                foreach (var item in yetki)
                {
                    company.Add(db.Company.Where(x => x.CompanyId == item.CompanyId).FirstOrDefault());
                }
                return Json(new { data = company }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}