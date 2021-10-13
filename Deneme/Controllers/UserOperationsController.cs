using Deneme.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deneme.Controllers
{
    public class UserOperationsController : MyBaseController
    {
     
        [Authorize]
        public ActionResult UOP_View()
        {
            return View();
        }

        [Authorize]
        public ActionResult UOP()
        {
           
                using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
                {
                    //string[] Keys = User.Identity.Name.Split(',');
                    int UserID = Convert.ToInt32(User.Identity.Name);
                    var user = dc.SiteUser.Where(a => a.ID == UserID).ToList();
                    return Json(new { data = user }, JsonRequestBehavior.AllowGet);
                }
        }
        [HttpGet]
        [Authorize]
        public ActionResult Edit_UOP(int id )
        {
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.SiteUser.Where(x => x.ID == id).FirstOrDefault<SiteUser>());
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit_UOP(SiteUser siteUser)
        {
            if (ModelState.IsValid)
            {
                var isExist = IsEmailExist(siteUser.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return Json(new { success = false, message = Deneme.Resource.EmailExist }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                    {
                        db.Entry(siteUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, message = Deneme.Resource.UpdatedSuccessfully }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = Deneme.Resource.Error }, JsonRequestBehavior.AllowGet);

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

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(int ID, ChangePasswordModel model)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                SiteUser siteUser = db.SiteUser.Where(x => x.ID == ID).FirstOrDefault<SiteUser>();
                string cryptoPassword = Crypto.Hash(model.OldPassword);
                if (siteUser.Password == cryptoPassword)
                {
                    siteUser.Password = Crypto.Hash(model.NewPassword);
                    siteUser.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.ChangedSuccesfully }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { success = false, message = Deneme.Resource.OldPasswordIncorrect }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}