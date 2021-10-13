using System;
using System.Linq;
using System.Web.Mvc;
using Deneme.Models;
using System.Data;


namespace Deneme.Controllers
{
    public class InvoiceTemplateController : MyBaseController
    {
        #region F.A.S
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_FAS(int id = 0)
        {
            if (id == 0)
                return View(new Şablon_FaturaAktarım());
            else
            {
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.Şablon_FaturaAktarım.Where(x => x.ŞablonId == id).FirstOrDefault<Şablon_FaturaAktarım>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_FAS(Şablon_FaturaAktarım fatura)
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
                    db.Şablon_FaturaAktarım.Add(fatura);
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
        public ActionResult Delete_FAS(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                Şablon_FaturaAktarım fatura = db.Şablon_FaturaAktarım.Where(x => x.ŞablonId == id).FirstOrDefault<Şablon_FaturaAktarım>();
                db.Şablon_FaturaAktarım.Remove(fatura);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult FAS_View()
        {

            return View();
        }

        [Authorize]
        public ActionResult FAS()
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
                var FaturaAktarmaŞablonu = dc.Şablon_FaturaAktarım.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = FaturaAktarmaŞablonu }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}