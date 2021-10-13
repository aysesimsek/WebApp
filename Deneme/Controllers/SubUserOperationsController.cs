using Deneme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deneme.Controllers
{
    public class SubUserOperationsController : MyBaseController
    {
        #region Sub kullanıcı görüntüle
        [Authorize]
        public ActionResult SUOP_View()
        {

            return View();
        }

        [Authorize]
        public ActionResult SUOP()
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                //string[] Keys = User.Identity.Name.Split(',');
                int UserID = Convert.ToInt32(User.Identity.Name);
                var user = dc.SubUser.Where(a => a.Yetkili == UserID).ToList();
                return Json(new { data = user }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Sub ekle-düzenle
        [HttpGet]
        [Authorize]
        public ActionResult AddOrEdit_SUOP(int id = 0)
        {
            if (id == 0)
                return View(new SubUser());
            else
            {
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.SubUser.Where(x => x.ID == id).FirstOrDefault<SubUser>());
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddOrEdit_SUOP(SubUser sub)
        {
            //string[] Keys = User.Identity.Name.Split(',');
            int UserID = Convert.ToInt32(User.Identity.Name);
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                sub.Yetkili = UserID;
                sub.Password = Crypto.Hash(sub.Password);
                sub.ConfirmPassword = Crypto.Hash(sub.ConfirmPassword);
                if (sub.ID == 0)
                {
                    var isExist = IsEmailExist(sub.Email);
                    if (isExist)
                    {
                        ModelState.AddModelError("EmailExist", "Email already exist");
                        return Json(new { success = false, message = Deneme.Resource.EmailExist }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.SubUser.Add(sub);
                        db.SaveChanges();
                        return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    db.Entry(sub).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.UpdatedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {

                var u = dc.SiteUser.Where(a => a.Email == emailID).FirstOrDefault();
                var v = dc.SubUser.Where(a => a.Email == emailID).FirstOrDefault();
                return v != null || u != null;
            }
        }
        #endregion

        #region delete sub 
        [HttpPost]
        [Authorize]
        public ActionResult Delete_SUOP(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                SubUser sub = db.SubUser.Where(x => x.ID == id).FirstOrDefault<SubUser>();
                db.SubUser.Remove(sub);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Yetkilendir 
        [HttpGet]
        [Authorize]
        public ActionResult Authorization_SUOP()
        {
            //string[] Keys = User.Identity.Name.Split(',');
            int UserID = Convert.ToInt32(User.Identity.Name);
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                var companyInfo = db.Company.Where(a => a.KullanıcıID == UserID).ToList();
             

                List<SelectListItem> CompanyInformations = (from item in companyInfo
                                                          select new SelectListItem
                                                          {
                                                              Text = item.CompanyName,
                                                              Value = item.CompanyId.ToString()
                                                          }).ToList();

                ViewBag.CompanyId = CompanyInformations;
         
            }

            return View();

        }        

        [HttpPost]
        [Authorize]
        public ActionResult Authorization_SUOP(Yetki yetki, int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1()) {
                var company = db.Company.Where(x => x.CompanyId == yetki.CompanyId).FirstOrDefault();
                Yetki isExist = db.Yetki.Where(x => x.UserID == id && x.CompanyId == yetki.CompanyId).FirstOrDefault<Yetki>();
                if (isExist !=null)
                {
                    db.Yetki.Remove(isExist);
                    db.SaveChanges();

                }
                //var sub = db.SubUser.Where(x => x.ID == id).FirstOrDefault();
                var Default = db.Yetki.Where(x => x.UserID == id && x.isDefault == true).FirstOrDefault();
                if(Default == null)
                {
                    yetki.isDefault = true;
                }
                yetki.UserID = id;
                string YetkiListe =Convert.ToInt16(yetki.BaOluştur) + "," + Convert.ToInt16(yetki.BaDüzenle)+ "," + Convert.ToInt16(yetki.BaMailGönder) + "," + Convert.ToInt16(yetki.BaSil)+ "," + Convert.ToInt16(yetki.BaSmsGönder)+ "," + Convert.ToInt16(yetki.BsOluştur) + "," + Convert.ToInt16(yetki.BsDüzenle) + "," + Convert.ToInt16(yetki.BsMailGönder) + "," + Convert.ToInt16(yetki.BsSil) + "," + Convert.ToInt16(yetki.BsSmsGönder)+ "," + Convert.ToInt16(yetki.CariOluştur) + "," + Convert.ToInt16(yetki.CariDüzenle) + "," + Convert.ToInt16(yetki.CariMailGönder)+ "," + Convert.ToInt16(yetki.CariSil)+ "," + Convert.ToInt16(yetki.CariSmsGönder);
                  yetki.Yetkiler = YetkiListe;
                yetki.CompanyName = company.CompanyName;
                db.Yetki.Add(yetki);
                db.SaveChanges();
            }
            return Json(new { success = true, message = Deneme.Resource.UpdatedSuccessfully }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Yetkileri göster

        //[HttpGet]
        //[Authorize]
        //public ActionResult Show_AuthView(int id)
        //{

        //    using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
        //    {
        //        var v = dc.FaturaBilgileriExcel.Where(a => a.FaturaBilgileriId == id).ToList();
        //        var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
        //        jsonResult.MaxJsonLength = int.MaxValue;
        //        return jsonResult;
        //    }

        //}

        [HttpGet]
        [Authorize]
        public ActionResult Show_Auth(int id)
        {
            //ViewBag.AuthId = id;
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                List<Company> company = new List<Company>();
                var auths = dc.Yetki.Where(a => a.UserID == id).ToList();
                foreach (var item in auths)
                {
                     var c = dc.Company.Where(a => a.CompanyId == item.CompanyId).FirstOrDefault();
                     company.Add(c);
                }
                ViewBag.auths = auths;
                ViewBag.company = company;

                return View();
            }
        }
        #endregion
    }
}