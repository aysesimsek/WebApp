using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Deneme.Models;
using System.Data;
using ExcelDataReader;
using System.Transactions;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;
using mailinblue;
using System.Net;

namespace Deneme.Controllers
{
    public class ReconciliationController : MyBaseController
    {
        #region View BaBs Mutabakat
        [Authorize]
        public ActionResult BaBsMY_View()
        {
            return View();
        }

        [Authorize]
        public ActionResult BaBsMY()
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
                var BaBsMutabakatYükleme = dc.BaBsMutabakat.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = BaBsMutabakatYükleme }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Add BaBs Mutabakat 
        [HttpPost]
        [Authorize]
        public ActionResult Add_BaBsMY(BaBsMutabakat mutabakat)
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
                #region Getting session informations
                var FaturaBilgileri = dc.FaturaBilgileri.Where(a => a.FaturaId == mutabakat.FaturaBilgileriId).FirstOrDefault();

                var totals = dc.FaturaBilgileriExcel.Where(a => a.FaturaBilgileriId == mutabakat.FaturaBilgileriId).GroupBy(x => new { x.VergiNo, x.VergiDairesi, x.CariKodu, x.CariAdı, x.TcKimlikNo })
                                                                .Select(x => new
                                                                {
                                                                    Total = x.Sum(g => g.KdvHariçTutar),
                                                                    VN = x.Key.VergiNo,
                                                                    VD = x.Key.VergiDairesi,
                                                                    CK = x.Key.CariKodu,
                                                                    CA = x.Key.CariAdı,
                                                                    TC = x.Key.TcKimlikNo,
                                                                    C = x.Count()
                                                                }).ToList();
                totals.RemoveAll(i => i.Total < 5);
                #endregion



                #region Save mutabakat 
                mutabakat.CariSayısı = totals.Count.ToString();
                mutabakat.BelgeSayısı = dc.FaturaBilgileriExcel.Where(a => a.FaturaBilgileriId == mutabakat.FaturaBilgileriId).ToList().Count.ToString();
                mutabakat.FaturaBilgileri = FaturaBilgileri.Yıl + " " + FaturaBilgileri.Ay + " " + FaturaBilgileri.FaturaTipi;
                mutabakat.KullanıcıId = UserID;
                mutabakat.CompanyId = SelectedCompanyID;
                mutabakat.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                mutabakat.Tip = FaturaBilgileri.FaturaTipi;
                dc.BaBsMutabakat.Add(mutabakat);
                dc.SaveChanges();

                #endregion

                #region Save mutabakat detayları

                BaBsMutabakatDetay mutabakatDetay = new BaBsMutabakatDetay();
                foreach (var item in totals)
                {
                    mutabakatDetay.CariAdı = item.CA;
                    mutabakatDetay.CariKodu = item.CK;
                    mutabakatDetay.MutabakatId = mutabakat.MutabakatId;
                    mutabakatDetay.VergiNo = item.VN;
                    mutabakatDetay.KdvHariçTutar = item.Total;
                    mutabakatDetay.BelgeSayısı = item.C.ToString();
                    mutabakatDetay.OnayDurumu = " ";
                    mutabakatDetay.MessageID = " ";
                    mutabakatDetay.MailDetay = dc.CariBilgileriExcel.Where(a => a.VergiNo == item.VN).FirstOrDefault().Email1.ToString();
                    dc.BaBsMutabakatDetay.Add(mutabakatDetay);
                    dc.SaveChanges();
                }

                #endregion

            }

            return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [Authorize]
        public ActionResult Add_BaBsMY()
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
                var FaturaBilgileri = dc.FaturaBilgileri.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                List<SelectListItem> FaturaBilgileriId;
                if (Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[0] == "1" && Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[5] == "0")
                {
                    FaturaBilgileriId = (from item in FaturaBilgileri.Where(x => x.FaturaTipi == "Alış")
                                         select new SelectListItem
                                         {
                                             Text = item.Yıl + " " + item.Ay + " " + item.FaturaTipi,
                                             Value = item.FaturaId.ToString()
                                         }).ToList();
                }
                else if (Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[0] == "0" && Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[5] == "1")
                {
                    FaturaBilgileriId = (from item in FaturaBilgileri.Where(x => x.FaturaTipi == "Satış")
                                         select new SelectListItem
                                         {
                                             Text = item.Yıl + " " + item.Ay + " " + item.FaturaTipi,
                                             Value = item.FaturaId.ToString()
                                         }).ToList();
                }
                else
                {
                    FaturaBilgileriId = (from item in FaturaBilgileri
                                         select new SelectListItem
                                         {
                                             Text = item.Yıl + " " + item.Ay + " " + item.FaturaTipi,
                                             Value = item.FaturaId.ToString()
                                         }).ToList();
                }

                ViewBag.FaturaBilgileriId = FaturaBilgileriId;

                return View();
            }
        }
        #endregion

        #region Show Mutabakat Detayları
        [HttpGet]
        [Authorize]
        public ActionResult Show_BaBsMYView(int id)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {


                #region Mesaj Durumu

                var mutabakatDetay = dc.BaBsMutabakatDetay.Where(a => a.MutabakatId == id).ToList();
                API sendinBlue = new mailinblue.API("MxcTA0LGPkWmHI2g");

                for (int i = 0; i < mutabakatDetay.Count; i++)
                {
                    if (mutabakatDetay[i].MessageID != " ")
                    {
                        Dictionary<string, Object> data = new Dictionary<string, Object>();
                        data.Add("message_id", mutabakatDetay[i].MessageID);
                        Object getReport = sendinBlue.get_report(data);
                        JToken token = JObject.Parse(getReport.ToString());
                        string Event = token.SelectToken("data[0].event").ToString();
                        if (Event == "requests")
                        {
                            mutabakatDetay[i].MailDurumu = Deneme.Resource.Sent;
                        }
                        else if (Event == "delivery")
                        {
                            mutabakatDetay[i].MailDurumu = Deneme.Resource.Delivered;
                        }
                        else if (Event == "views")
                        {
                            mutabakatDetay[i].MailDurumu = Deneme.Resource.Viewed;
                        }

                        else if (Event == "invalid")
                        {
                            mutabakatDetay[i].MailDurumu = Deneme.Resource.InvalidMail;
                        }
                        else if (Event == "spam")
                        {
                            mutabakatDetay[i].MailDurumu = Deneme.Resource.Spam;
                        }
                        else if (Event == "clicks")
                        {
                            mutabakatDetay[i].MailDurumu = Deneme.Resource.Clicked;
                        }

                        if (mutabakatDetay[i].OnayDurumu == "Approved" || mutabakatDetay[i].OnayDurumu == "Onaylandı")
                        {
                            mutabakatDetay[i].OnayDurumu = Deneme.Resource.Approved;
                        }

                        else if (mutabakatDetay[i].OnayDurumu == "Not Approved" || mutabakatDetay[i].OnayDurumu == "Onaylanmadı")
                        {
                            mutabakatDetay[i].OnayDurumu = Deneme.Resource.NotApproved;
                        }
                        else if (mutabakatDetay[i].OnayDurumu == "Pending" || mutabakatDetay[i].OnayDurumu == "Beklemede")
                        {
                            mutabakatDetay[i].OnayDurumu = Deneme.Resource.Pending;
                        }
                        dc.SaveChanges();
                    }


                }
                #endregion

                var v = dc.BaBsMutabakatDetay.Where(a => a.MutabakatId == id).ToList();
                var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }


        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_BaBsMY(int id)
        {

            ViewBag.MutabakatId = id;
            return View();

        }
        #endregion

        #region Delete Mutabakat ve Mutabakat Detayları
        [HttpPost]
        public ActionResult Delete_BaBsMY(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                var ba = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[3];
                var bs = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[8];

                BaBsMutabakat mutabakatBilgileri = db.BaBsMutabakat.Where(x => x.MutabakatId == id).FirstOrDefault<BaBsMutabakat>();
                var FaturaTipi = mutabakatBilgileri.FaturaBilgileri.Split(' ')[2];

                if (ba == "1" && FaturaTipi == "Alış" || bs == "1" && FaturaTipi == "Satış")
                {
                    BaBsMutabakatDetay mutabakatDetay = db.BaBsMutabakatDetay.Where(x => x.MutabakatId == id).FirstOrDefault<BaBsMutabakatDetay>();
                    db.BaBsMutabakat.Remove(mutabakatBilgileri);
                    db.BaBsMutabakatDetay.RemoveRange(db.BaBsMutabakatDetay.Where(x => x.MutabakatId == id));
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Yetkiniz Bulunmamaktadır" }, JsonRequestBehavior.AllowGet);

                }
            }
        }
        #endregion

        #region SendMail

        [HttpGet]
        [Authorize]
        public ActionResult SendMail_BaBsMY(int id)
        {

            //string[] Keys = User.Identity.Name.Split(',');

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
                var ba = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[2];
                var bs = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[7];
                int SelectedCompanyID = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["UserOp"]["CompanyId"]));
                var faturaDetayları = dc.BaBsMutabakat.Where(a => a.MutabakatId == id).FirstOrDefault();
                var FaturaTipi = faturaDetayları.FaturaBilgileri.Split(' ')[2];

                if (ba == "1" && FaturaTipi == "Alış" || bs == "1" && FaturaTipi == "Satış")
                {
                    var v = dc.BaBsMutabakatDetay.Where(a => a.MutabakatId == id).ToList();
                    var mutabakatDetay = dc.BaBsMutabakatDetay.Where(a => a.MutabakatId == id).FirstOrDefault();
                    var CompanyInfo = dc.Company.Where(x => x.CompanyId == SelectedCompanyID).FirstOrDefault();

                    API sendinBlue = new mailinblue.API("MxcTA0LGPkWmHI2g");
                    Dictionary<string, Object> data = new Dictionary<string, Object>();
                    Dictionary<string, string> to = new Dictionary<string, string>();
                    List<string> from_name = new List<string>();

                    string body = "";
                    //string subject = "";
                    for (int i = 0; i < v.Count; i++)
                    {
                        data.Clear();
                        to.Clear();
                        from_name.Clear();

                        v[i].MutabakatKabulLink = Guid.NewGuid();
                        var verifyUrl = "/EvidenceFile/VerifyMutabakat/" + v[i].MutabakatKabulLink;
                        var trueLink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                        v[i].MutabakatRetLink = Guid.NewGuid();
                        verifyUrl = "/EvidenceFile/VerifyMutabakat/" + v[i].MutabakatRetLink;
                        var falseLink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                        to.Add(v[i].MailDetay, "Mutabakat İşlemi");
                        from_name.Add("bilgi@mutabakatci.com");
                        from_name.Add("Yirmibeş Yazılım ve Danışmanlık");

                        body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Mutabakat" + ".cshtml");
                        //subject = "Mutabakat Onayı";

                        string date = (DateTime.Now.Date).ToString("yyyy-MM-dd");
                        string GönderimTarihi = Convert.ToString(DateTime.Now);

                        body = body.Replace("@ViewBag.MutabikLink", trueLink);
                        body = body.Replace("@ViewBag.MutabikDegilLink", falseLink);

                        #region Mail Template Bilgileri
                        body = body.Replace("@ViewBag.FaturaDetayları", faturaDetayları.FaturaBilgileri);
                        body = body.Replace("@ViewBag.GönderimTarihi", GönderimTarihi);
                        body = body.Replace("@ViewBag.KullanıcıTaxOffice", CompanyInfo.TaxOffice);
                        body = body.Replace("@ViewBag.KullanıcıCompanyTitle", CompanyInfo.CompanyName);
                        body = body.Replace("@ViewBag.KullanıcıPhoneNumber", CompanyInfo.PhoneNumber);
                        body = body.Replace("@ViewBag.KullanıcıTaxNumber", CompanyInfo.TaxNo_TcNo);


                        body = body.Replace("@ViewBag.GönderilenCariAdı", v[i].CariAdı);
                        body = body.Replace("@ViewBag.GönderilenVergiNo", v[i].VergiNo);
                        body = body.Replace("@ViewBag.GönderilenBelgeSayısı", v[i].BelgeSayısı);
                        body = body.Replace("@ViewBag.GönderilenToplamTutar", v[i].KdvHariçTutar.ToString());
                        body = body.Replace("@ViewBag.GönderilenCariKodu", v[i].CariKodu);

                        body = body.ToString();
                        #endregion

                        data.Add("to", to);
                        data.Add("from", from_name);
                        data.Add("subject", "deneme");
                        data.Add("html", body);

                        Object sendEmail = sendinBlue.send_email(data);
                        JToken token = JObject.Parse(sendEmail.ToString());
                        string messageID = token.SelectToken("data.message-id").ToString();
                        v[i].MessageID = messageID;
                        v[i].OnayDurumu = "Pending";
                        dc.SaveChanges();

                    }
                    return Json(new { success = true, message = Deneme.Resource.MailSent }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Yetkiniz Bulunmamaktadır" }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        #endregion

        #region Download File

        [HttpGet]
        [Authorize]
        public FileResult Download(int id) //FileResult
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var file = dc.BaBsMutabakatDetay.Where(a => a.MutabakatDetayId == id).FirstOrDefault();
                if (file.FileName != null)
                {
                    string path = Server.MapPath("~/EvidenceFiles/" + file.FileName + "");
                    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                    string fileName = file.FileName;
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                    return null;
            }
        }

        #endregion
    }
}
