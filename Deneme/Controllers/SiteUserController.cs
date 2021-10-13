using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Deneme.Models;
using mailinblue;

namespace Deneme.Controllers
{
    public class SiteUserController : MyBaseController
    {
        #region Registration
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsValid,ActivationLink")] Registration reg)
        {
            Company company = new Company();
            Yetki yetki = new Yetki();
            SiteUser user = new SiteUser();


            bool Status = false;
            string message = "";

            // Model Validation 
            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = IsEmailExist(reg.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    message = @Deneme.Resource.EmailExist;
                    ViewBag.Message = message;
                    ViewBag.Status = Status;
                    Status = false;
                    return View(reg);
                }
                #endregion



                user.Username = reg.Username;
                user.Email = reg.Email;

                #region Generate Activation Code 
                user.ActivationLink = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(reg.Password);
                reg.ConfirmPassword = Crypto.Hash(reg.ConfirmPassword);
                #endregion

                user.IsValid = false;

                #region Save to Database
                using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
                {

                    dc.SiteUser.Add(user);
                    dc.SaveChanges();

                    #region Company

                    company.KullanıcıID = user.ID;
                    company.TaxNo_TcNo = reg.TaxNumber;
                    company.CompanyName = reg.CompanyTitle;
                    company.Address = reg.Address;
                    company.City = reg.City;
                    company.Country = reg.Country;
                    company.FaxNumber = reg.FaxNumber;
                    company.PhoneNumber = reg.PhoneNumber;
                    company.State = reg.State;
                    company.TaxOffice = reg.TaxOffice;
                    company.IsDefault = true;

                    dc.Company.Add(company);
                    dc.SaveChanges();
                    #endregion

                    //Send Email to User
                    SendinBlue(user.ID, user.ActivationLink.ToString());
                    message = @Deneme.Resource.RegDone + user.Email + @Deneme.Resource.HasBeenSent;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = @Deneme.Resource.InvalidRequest;
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var sub = dc.SubUser.Where(a => a.Email == emailID).FirstOrDefault();
                var v = dc.SiteUser.Where(a => a.Email == emailID).FirstOrDefault();


                return v != null || sub != null;
            }
        }
        #endregion

        #region Send Mail
        [NonAction]
        public void SendinBlue(int regID, string activationlink, string emailFor = "VerifyAccount")
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                string body = null;
                var regInfo = dc.SiteUser.Where(x => x.ID == regID).FirstOrDefault();
                var VerifyUrl = "/SiteUser/" + emailFor + "/" + activationlink;
                var Link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, VerifyUrl);
                // var url = activationlink + "Register/Confirm?regId=" + regID;


                string subject = "";

                if (emailFor == "VerifyAccount")
                {
                    body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Activation" + ".cshtml");
                    subject = "Hesap Doğrulaması";

                    body = body.Replace("@ViewBag.ConfirmationLink", Link);
                    body = body.ToString();

                }
                else if (emailFor == "ResetPassword")
                {
                    body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Password" + ".cshtml");
                    subject = "Şifre Sıfırlama";
                    body = body.Replace("@ViewBag.ResetLink", Link);
                    body = body.Replace("@ViewBag.UserMail", regInfo.Email);
                    body = body.ToString();
                }



                API sendinBlue = new mailinblue.API("MxcTA0LGPkWmHI2g");
                Dictionary<string, Object> data = new Dictionary<string, Object>();
                Dictionary<string, string> to = new Dictionary<string, string>();


                to.Add(regInfo.Email, regInfo.Email);
                List<string> from_name = new List<string>();
                from_name.Add("bilgi@mutabakatci.com");
                from_name.Add("Yirmibeş Yazılım ve Danışmanlık");

                data.Add("to", to);
                data.Add("from", from_name);
                data.Add("subject", subject);
                data.Add("html", body);

                Object sendEmail = sendinBlue.send_email(data);
            }




        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                var v = dc.SiteUser.Where(a => a.ActivationLink == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsValid = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = @Deneme.Resource.InvalidRequest;
                }
            }
            ViewBag.Status = Status;
            return RedirectToAction("Login", "SiteUser");
        }
        #endregion

        #region Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "", string message = "")
        {
            message = " ";
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var v = dc.SiteUser.Where(a => a.Email == login.Email).FirstOrDefault();
                var sub = dc.SubUser.Where(a => a.Email == login.Email).FirstOrDefault();


                if (v != null)
                {
                    var company = dc.Company.Where(a => a.KullanıcıID == v.ID && a.IsDefault == true).FirstOrDefault();

                    if (v.IsValid == false)
                    {
                        ViewBag.Message = @Deneme.Resource.VerifyEmail;
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {

                        string Keys = v.ID.ToString(); /*+ "," + company.CompanyId.ToString() + "," + "1"*/
                        //Session["Yetki"] = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(Keys, false, Session.Timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        HttpContext.Response.AppendCookie(cookie);


                        HttpCookie Scookie = new HttpCookie("UserOp");
                        Scookie.Values["Yetki"] = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
                        Scookie.Values["CompanyId"] = company.CompanyId.ToString();
                        Scookie.Values["isAdmin"] = "1";
                        Scookie.Expires = DateTime.Now.AddMinutes(30);
                        Scookie.HttpOnly = true;
                        Response.Cookies.Add(Scookie);
                        HttpContext.Response.AppendCookie(Scookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = @Deneme.Resource.InvalidCredential;
                    }
                }
                else if (sub != null)
                {
                    //var company = dc.Company.Where(a => a.KullanıcıID == sub.Yetkili && a.IsDefault == true).FirstOrDefault();
                    var company = dc.Yetki.Where(a => a.UserID == sub.ID && a.isDefault == true).FirstOrDefault();
                    v = dc.SiteUser.Where(a => a.ID == sub.Yetkili).FirstOrDefault();
                    var yetki = dc.Yetki.Where(a => a.UserID == sub.ID && a.CompanyId == company.CompanyId).FirstOrDefault();
                    if (string.Compare(Crypto.Hash(login.Password), sub.Password) == 0)
                    {

                        string Keys = v.ID.ToString(); /*+ "," + company.CompanyId.ToString() + "," + "0" + "," + sub.ID.ToString()*/
                        //Session["Yetki"] = yetki.Yetkiler;

                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(Keys, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        HttpContext.Response.Cookies.Add(cookie);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        HttpCookie Scookie = new HttpCookie("UserOp");
                        Scookie.Values["Yetki"] = yetki.Yetkiler;
                        Scookie.Values["CompanyId"] = company.CompanyId.ToString();
                        Scookie.Values["isAdmin"] = "0";
                        Scookie.Values["SubId"] = sub.ID.ToString();
                        Scookie.Expires = DateTime.Now.AddMinutes(30);
                        Scookie.HttpOnly = true;
                        Response.Cookies.Add(Scookie);
                        HttpContext.Response.AppendCookie(Scookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = @Deneme.Resource.InvalidCredential;
                    }
                }
                else
                {
                    message = @Deneme.Resource.InvalidCredential;
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Logout()
        {

            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "SiteUser");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {
            string message = "";
            bool status = false;

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var account = dc.SiteUser.Where(a => a.Email == Email).FirstOrDefault();
                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendinBlue(account.ID, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    status = true;
                    message = @Deneme.Resource.ResetPasswordLink;
                }
                else
                {
                    message = @Deneme.Resource.AccountNotFound;
                    status = false;
                }
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var user = dc.SiteUser.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {

            var message = "";
            if (ModelState.IsValid)
            {
                using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
                {
                    var user = dc.SiteUser.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = Deneme.Resource.UpdatedPassword;
                    }
                }
            }
            else
            {
                message = @Deneme.Resource.InvalidRequest;
                ViewBag.Message = message;
                return View();
            }
            ViewBag.Message = message;
            return RedirectToAction("Login", "SiteUser");
        }

        #endregion

        #region Lang
        public ActionResult ChangeLanguage(string lang)
        {
            new SiteLanguages().SetLanguage(lang);

            return Redirect(Request.UrlReferrer.ToString());
        }
        #endregion

    }
}