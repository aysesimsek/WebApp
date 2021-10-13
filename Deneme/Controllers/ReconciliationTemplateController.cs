using System;
using System.Linq;
using System.Web.Mvc;
using Deneme.Models;
using System.Data;


namespace Deneme.Controllers
{
    public class ReconciliationTemplateController : MyBaseController
    {
        #region BaBs.M.S
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_BBMS(int id = 0)
        {
            if (id == 0)
                return View(new Şablon_Ba_BsMutabakat());
            else
            {
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.Şablon_Ba_BsMutabakat.Where(x => x.ŞablonId == id).FirstOrDefault<Şablon_Ba_BsMutabakat>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_BBMS(Şablon_Ba_BsMutabakat fatura)
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
                    db.Şablon_Ba_BsMutabakat.Add(fatura);
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
        public ActionResult Delete_BBMS(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                Şablon_Ba_BsMutabakat fatura = db.Şablon_Ba_BsMutabakat.Where(x => x.ŞablonId == id).FirstOrDefault<Şablon_Ba_BsMutabakat>();
                db.Şablon_Ba_BsMutabakat.Remove(fatura);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully}, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult BBMS_View()
        {

            return View();
        }

        [Authorize]
        public ActionResult BBMS()
        {
            
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
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
                var BaBsMutabakatŞablonu = dc.Şablon_Ba_BsMutabakat.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = BaBsMutabakatŞablonu }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}