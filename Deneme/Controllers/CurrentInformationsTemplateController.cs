using System;
using System.Linq;
using System.Web.Mvc;
using Deneme.Models;
using System.Data;

namespace Deneme.Controllers
{
    public class CurrentInformationsTemplateController : MyBaseController
    {
        #region C.B.S
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_CBS(int id = 0)
        {
            if (id == 0)
                return View(new Şablon_CariBilgileri());
            else
            {
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.Şablon_CariBilgileri.Where(x => x.ŞablonId == id).FirstOrDefault<Şablon_CariBilgileri>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_CBS(Şablon_CariBilgileri fatura)
        {
            //string[] Keys = User.Identity.Name.Split(',');
            int UserID = Convert.ToInt32(User.Identity.Name);
            int SelectedCompanyID = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["UserOp"]["CompanyId"]));
            //if (Session["SelectedCompanyID"] != null)
            //{
            //    SelectedCompanyID = Convert.ToInt32(Session["SelectedCompanyID"]);
            //}
            //else
            //{
            //    SelectedCompanyID = Convert.ToInt32(Keys[1]);
            //}
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                fatura.KullanıcıId = UserID;
                fatura.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                fatura.CompanyId = SelectedCompanyID;
                if (fatura.ŞablonId == 0)
                {
                    db.Şablon_CariBilgileri.Add(fatura);
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.Entry(fatura).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.UpdatedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
            }


        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete_CBS(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                Şablon_CariBilgileri fatura = db.Şablon_CariBilgileri.Where(x => x.ŞablonId == id).FirstOrDefault<Şablon_CariBilgileri>();
                db.Şablon_CariBilgileri.Remove(fatura);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult CBS_View()
        {

            return View();
        }

        [Authorize]
        public ActionResult CBS()
        {
            //string[] Keys = User.Identity.Name.Split(',');
            int UserID = Convert.ToInt32(User.Identity.Name);
            int SelectedCompanyID = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["UserOp"]["CompanyId"]));
            //if (Session["SelectedCompanyID"] != null)
            //{
            //    SelectedCompanyID = Convert.ToInt32(Session["SelectedCompanyID"]);
            //}
            //else
            //{
            //    SelectedCompanyID = Convert.ToInt32(Keys[1]);
            //}
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var CariBilgileriŞablonu = dc.Şablon_CariBilgileri.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID ).ToList();
                return Json(new { data = CariBilgileriŞablonu }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}