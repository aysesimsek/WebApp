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
using mailinblue;
using Newtonsoft.Json.Linq;
using System.Web.Hosting;

namespace Deneme.Controllers
{
    public class CurrentReconciliationController : MyBaseController
    {
        #region Show Cari Mutabakat Excel
        [HttpGet]
        [Authorize]
        public ActionResult Show_CRView(int id)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var v = dc.CariMutabakatExcel.Where(a => a.CariMutabakatId == id).ToList();
                var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_CR(int id)
        {
            ViewBag.CariMutabakatId = id;
            return View();

        }
        #endregion

        #region Detail Cari Mutabakat 
        [HttpGet]
        [Authorize]
        public ActionResult Detail_CRView(int id)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {

                #region Mesaj Durumu

                var cariMutabakatDetay = dc.CariMutabakatDetay.Where(a => a.CariMutabakatId == id).ToList();
                API sendinBlue = new mailinblue.API("MxcTA0LGPkWmHI2g");

                for (int i = 0; i < cariMutabakatDetay.Count; i++)
                {
                    if (cariMutabakatDetay[i].MessageID != " ")
                    {
                        Dictionary<string, Object> data = new Dictionary<string, Object>();
                        data.Add("message_id", cariMutabakatDetay[i].MessageID);
                        Object getReport = sendinBlue.get_report(data);
                        JToken token = JObject.Parse(getReport.ToString());
                        string Event = token.SelectToken("data[0].event").ToString();
                        if (Event == "requests")
                        {
                            cariMutabakatDetay[i].MailDurumu = Deneme.Resource.Sent;
                        }
                        else if (Event == "delivery")
                        {
                            cariMutabakatDetay[i].MailDurumu = Deneme.Resource.Delivered;
                        }
                        else if (Event == "views")
                        {
                            cariMutabakatDetay[i].MailDurumu = Deneme.Resource.Viewed;
                        }

                        else if (Event == "invalid")
                        {
                            cariMutabakatDetay[i].MailDurumu = Deneme.Resource.InvalidMail;
                        }
                        else if (Event == "spam")
                        {
                            cariMutabakatDetay[i].MailDurumu = Deneme.Resource.Spam;
                        }
                        else if (Event == "clicks")
                        {
                            cariMutabakatDetay[i].MailDurumu = Deneme.Resource.Clicked;
                        }

                        if (cariMutabakatDetay[i].OnayDurumu == "Approved" || cariMutabakatDetay[i].OnayDurumu == "Onaylandı")
                        {
                            cariMutabakatDetay[i].OnayDurumu = Deneme.Resource.Approved;
                        }

                        else if (cariMutabakatDetay[i].OnayDurumu == "Not Approved" || cariMutabakatDetay[i].OnayDurumu == "Onaylanmadı")
                        {
                            cariMutabakatDetay[i].OnayDurumu = Deneme.Resource.NotApproved;
                        }
                        else if (cariMutabakatDetay[i].OnayDurumu == "Pending" || cariMutabakatDetay[i].OnayDurumu == "Beklemede")
                        {
                            cariMutabakatDetay[i].OnayDurumu = Deneme.Resource.Pending;
                        }
                        dc.SaveChanges();
                    }


                }
                #endregion

                var v = dc.CariMutabakatDetay.Where(a => a.CariMutabakatId == id).ToList();
                var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Detail_CR(int id)
        {
            ViewBag.CariMutabakatId = id;
            return View();

        }
        #endregion


        #region Add Cari Mutabakat Excel
        [HttpGet]
        [Authorize]
        public ActionResult Add_CR()
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
                var CariMutabakatŞablonu = dc.Şablon_CariMutabakat.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();

                List<int> Yıl = new List<int> { 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018 };

                ViewBag.Yıl = Yıl;
                List<SelectListItem> ŞablonId = (from item in CariMutabakatŞablonu
                                                 select new SelectListItem
                                                 {
                                                     Text = item.ŞablonAdı,
                                                     Value = item.ŞablonId.ToString()
                                                 }).ToList();


                ViewBag.ŞablonId = ŞablonId;

                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add_CR(HttpPostedFileBase excelfile, CariMutabakkat cariMutabakat)
        {
            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Please select a excel file <br>";
                return View("Index");
            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {


                    using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
                    {
                        #region Getting session informations
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
                        var şablon = dc.Şablon_CariMutabakat.Where(a => a.ŞablonId == cariMutabakat.ŞablonId).FirstOrDefault();
                        #endregion

                        #region Save bill details
                        cariMutabakat.KullanıcıId = UserID;
                        cariMutabakat.CompanyId = SelectedCompanyID;
                        cariMutabakat.ExcelAdı = excelfile.FileName;
                        cariMutabakat.ŞablonAdı = şablon.ŞablonAdı;
                        cariMutabakat.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                        dc.CariMutabakkat.Add(cariMutabakat);
                        dc.SaveChanges();
                        #endregion

                        #region Getting excel column names
                        int be = (şablon.BelgeNumarası)[0] - 65;
                        int bt = (şablon.BelgeTarihi)[0] - 65;
                        int ca = (şablon.CariAdı)[0] - 65;
                        int ck = (şablon.CariKodu)[0] - 65;
                        int t = (şablon.KdvHariçTutar)[0] - 65;
                        int kdv = (şablon.KdvTutarı)[0] - 65;
                        int vd = (şablon.VergiDairesi)[0] - 65;
                        int vn = (şablon.VergiNo)[0] - 65;
                        #endregion

                        try
                        {

                            string path = Server.MapPath("~/Content/" + excelfile.FileName);

                            #region Checking file is exist
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            #endregion

                            excelfile.SaveAs(path);

                            #region File Operations
                            FileStream fs = new FileStream(path, FileMode.Open);
                            IExcelDataReader excelReader;
                            if (excelfile.FileName.EndsWith("xls"))
                            {
                                excelReader = ExcelReaderFactory.CreateBinaryReader(fs);
                            }
                            else
                            {
                                excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                            }
                            #endregion

                            try
                            {

                                #region Transfer excel to dataset
                                DataSet excelDS = excelReader.AsDataSet();
                                DataTable excel = excelDS.Tables[0];
                                DataTable copyExcel = excel.Clone();
                                copyExcel.Columns[t].DataType = typeof(Double);

                                DataColumn dcolColumn = new DataColumn("CariMutabakatId", typeof(Int32));
                                dcolColumn.DefaultValue = cariMutabakat.CariMutabakatId;
                                copyExcel.Columns.Add(dcolColumn);

                                int başlangıçSatırı = 1;
                                foreach (DataRow row in excel.Rows)
                                {
                                    try
                                    {
                                        if (başlangıçSatırı >= şablon.BaşlangıçSatırı)
                                        {
                                            copyExcel.ImportRow(row);
                                        }
                                        başlangıçSatırı++;
                                    }
                                    catch (Exception)
                                    {
                                        CariMutabakkat cMutabakat = dc.CariMutabakkat.Where(x => x.CariMutabakatId == cariMutabakat.CariMutabakatId).FirstOrDefault<CariMutabakkat>();
                                        dc.CariMutabakkat.Remove(cMutabakat);
                                        //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                        dc.SaveChanges();
                                        return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                                    }
                                 
                                }
                                excelReader.Close();
                                #endregion

                                #region Deleting unnecessary columns

                                //for (int i = 0; i < excel.Columns.Count; i++)
                                //{

                                //    if (i != be && i != bt && i != ca && i != ck && i != kdv && i != t && i != vd && i != vn)
                                //    {
                                //        copyExcel.Columns.Remove("Column" + i);

                                //    }


                                //}
                                #endregion

                                try
                                {
                                    #region Adding bulk data in database
                                    using (var scope = new TransactionScope())
                                    {
                                        string connectionString =
                                        "Data Source=94.73.148.5;" +
                                        "Initial Catalog=YirmibesYazilimMutabakat;" +
                                        "User id=user5EE1B9;" +
                                        "Password=MEhmet5346;";

                                        var sqlConnection = new SqlConnection(connectionString);
                                        var sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                                        {
                                            BatchSize = 4000,
                                            DestinationTableName = "CariMutabakatExcel",
                                            BulkCopyTimeout = 6000
                                        };
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ck, "CariKodu");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ca, "CariAdı");                                     
                                        sqlBulkCopy.ColumnMappings.Add("Column" + vd, "VergiDairesi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + vn, "VergiNo");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + bt, "BelgeTarihi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + be, "BelgeNumarası");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + t, "KdvHariçTutar");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + kdv, "KdvTutarı");
                                        sqlBulkCopy.ColumnMappings.Add("CariMutabakatId", "CariMutabakatId");

                                        var dataTable = copyExcel;
                                        sqlConnection.Open();

                                        sqlBulkCopy.WriteToServer(dataTable);

                                        scope.Complete();
                                        sqlBulkCopy.Close();
                                        sqlConnection.Close();
                                        sqlConnection.Dispose();
                                    }
                                    Add_CRDetail(cariMutabakat);
                                    #endregion
                                }
                                catch (Exception e)
                                {
                                    CariMutabakkat cMutabakat = dc.CariMutabakkat.Where(x => x.CariMutabakatId == cariMutabakat.CariMutabakatId).FirstOrDefault<CariMutabakkat>();
                                    dc.CariMutabakkat.Remove(cMutabakat);
                                    //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                    dc.SaveChanges();
                                    return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                                }
                            }
                            catch (Exception)
                            {
                                CariMutabakkat cMutabakat = dc.CariMutabakkat.Where(x => x.CariMutabakatId == cariMutabakat.CariMutabakatId).FirstOrDefault<CariMutabakkat>();
                                dc.CariMutabakkat.Remove(cMutabakat);
                                //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                dc.SaveChanges();
                                return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                            }
                        }
                        catch (Exception)
                        {
                            CariMutabakkat cMutabakat = dc.CariMutabakkat.Where(x => x.CariMutabakatId == cariMutabakat.CariMutabakatId).FirstOrDefault<CariMutabakkat>();
                            dc.CariMutabakkat.Remove(cMutabakat);
                            //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                            dc.SaveChanges();
                            return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = Deneme.Resource.SaveProblem }, JsonRequestBehavior.AllowGet);

                }

            }


        }
        #endregion


        #region CariMutabakatDetayları
        public void Add_CRDetail(CariMutabakkat mutabakat)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                #region Getting session informations
                var cariMutabakat = dc.CariMutabakkat.Where(a => a.CariMutabakatId == mutabakat.CariMutabakatId).FirstOrDefault();

                var totals = dc.CariMutabakatExcel.Where(a => a.CariMutabakatId == mutabakat.CariMutabakatId).GroupBy(x => new { x.VergiNo, x.VergiDairesi, x.CariKodu, x.CariAdı })
                                                                .Select(x => new
                                                                {
                                                                    TotalKdvHariç = x.Sum(g => g.KdvHariçTutar),
                                                                    TotalKdv = x.Sum(g => g.KdvTutarı),
                                                                    VN = x.Key.VergiNo,
                                                                    VD = x.Key.VergiDairesi,
                                                                    CK = x.Key.CariKodu,
                                                                    CA = x.Key.CariAdı,
                                                                    C = x.Count()
                                                                }).ToList();
          
                #endregion



                #region Save mutabakat 

                cariMutabakat.CariSayısı = totals.Count.ToString();
                cariMutabakat.BelgeSayısı = dc.CariMutabakatExcel.Where(a => a.CariMutabakatId == mutabakat.CariMutabakatId).ToList().Count.ToString(); 
                dc.SaveChanges();
                #endregion

                #region Save mutabakat detayları

                CariMutabakatDetay mutabakatDetay = new CariMutabakatDetay();
                foreach (var item in totals)
                {
                    mutabakatDetay.CariAdı = item.CA;
                    mutabakatDetay.CariKodu = item.CK;
                    mutabakatDetay.CariMutabakatId = mutabakat.CariMutabakatId;
                    mutabakatDetay.VergiNo = item.VN;
                    mutabakatDetay.KdvHariçTutar = item.TotalKdvHariç;
                    mutabakatDetay.KdvTutarı = item.TotalKdv;
                    mutabakatDetay.BelgeSayısı = item.C.ToString();
                    mutabakatDetay.OnayDurumu = " ";
                    mutabakatDetay.MessageID = " ";
                    mutabakatDetay.MailDetay = dc.CariBilgileriExcel.Where(a => a.VergiNo == item.VN).FirstOrDefault().Email1.ToString();
                    dc.CariMutabakatDetay.Add(mutabakatDetay);
                    dc.SaveChanges();
                }

                #endregion

            }

        }

        #endregion

        #region Delete Cari Mutabakat ve Cari Mutabakat Detayları
        [HttpPost]
        [Authorize]
        public ActionResult Delete_CR(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                try
                {
                    CariMutabakkat cariMutabakkat = db.CariMutabakkat.Where(x => x.CariMutabakatId == id).FirstOrDefault<CariMutabakkat>();
                    // CariMutabakatExcel cariMutabakatExcel = db.CariMutabakatExcel.Where(x => x.CariMutabakatId == id).FirstOrDefault<CariMutabakatExcel>();
                    db.CariMutabakkat.Remove(cariMutabakkat);
                    db.CariMutabakatExcel.RemoveRange(db.CariMutabakatExcel.Where(x => x.CariMutabakatId == id));
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                catch(Exception)
                {

                    return Json(new { success = false, message = Deneme.Resource.IOExeption }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region View Cari Mutabakat Bilgileri
        [Authorize]
        public ActionResult CR_View()
        {
            return View();
        }

        [Authorize]
        public ActionResult CR()
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
                var CurrentReconciliation = dc.CariMutabakkat.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = CurrentReconciliation }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region SendMail

        [HttpGet]
        [Authorize]
        public ActionResult SendMail_CR(int id)
        {
            var cm = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[12];
            if (cm == "1")
            {
                //string[] Keys = User.Identity.Name.Split(',');
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
                    var cariMutabakatBilgileri = dc.CariMutabakkat.Where(a => a.CariMutabakatId == id).FirstOrDefault();
                    var v = dc.CariMutabakatDetay.Where(a => a.CariMutabakatId == id).ToList();
                    var cariMutabakatDetay = dc.CariMutabakatDetay.Where(a => a.CariMutabakatId == id).FirstOrDefault();
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
                        var verifyUrl = "/CurrentEvidenceFile/VerifyMutabakat/" + v[i].MutabakatKabulLink;
                        var trueLink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                        v[i].MutabakatRetLink = Guid.NewGuid();
                        verifyUrl = "/CurrentEvidenceFile/VerifyMutabakat/" + v[i].MutabakatRetLink;
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
                        body = body.Replace("@ViewBag.FaturaDetayları", cariMutabakatBilgileri.Yıl + " " + cariMutabakatBilgileri.Ay);
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
                }
                return Json(new { success = true, message = Deneme.Resource.MailSent }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Yetkiniz Bulunmamaktadır" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        //[Authorize]
        //public void VerifyMutabakat(string id)
        //{
        //    using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
        //    {
        //        dc.Configuration.ValidateOnSaveEnabled = false;

        //        var t = dc.CariMutabakatDetay.Where(a => a.MutabakatKabulLink == new Guid(id)).FirstOrDefault();
        //        var f = dc.CariMutabakatDetay.Where(a => a.MutabakatRetLink == new Guid(id)).FirstOrDefault();

        //        if (t != null)
        //        {
        //            t.OnayDurumu = "Approved";
        //            dc.SaveChanges();

        //        }
        //        else if (f != null)
        //        {
        //            f.OnayDurumu = "NotApproved";
        //            dc.SaveChanges();

        //        }

        //    }

        //}

        #endregion

        #region Download File

        [HttpGet]
        [Authorize]
        public FileResult Download(int id) //FileResult
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var file = dc.CariMutabakatDetay.Where(a => a.CariMutabakatDetayId == id).FirstOrDefault();
                string path = Server.MapPath("~/EvidenceFiles/" + file.FileName + "");
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string fileName = file.FileName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
        }

        #endregion
    }
}