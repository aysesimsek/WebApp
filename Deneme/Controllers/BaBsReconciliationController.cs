using Deneme.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Deneme.Controllers
{
    public class BaBsReconciliationController : MyBaseController
    {

        #region Show BaBs Excel
        [HttpGet]
        [Authorize]
        public ActionResult Show_BaBsExcelView(int id)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var v = dc.BaBsMutabakatExcel.Where(a => a.MutabakatBilgileriId == id).ToList();
                var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_BaBsExcel(int id)
        {
            ViewBag.FaturaId = id;
            return View();

        }
        #endregion

        #region Add BaBsExcel
        [HttpGet]
        [Authorize]
        public ActionResult Add_BaBsExcel()
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {

                //string Keys = User.Identity.Name.Split(',');
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
                var BaBsŞablon = dc.Şablon_Ba_BsMutabakat.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();

                List<int> Yıl = new List<int> { 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018 };

                ViewBag.Yıl = Yıl;
                List<SelectListItem> ŞablonId = (from item in BaBsŞablon
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
        public ActionResult Add_BaBsExcel(HttpPostedFileBase excelfile, BaBsMutabakatBilgileri fatura)
        {
            var bal = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[0];
            var bsat = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[5];
            var FaturaTipi = fatura.FaturaTipi;

            if (bal == "1" && FaturaTipi == "Alış" || bsat == "1" && FaturaTipi == "Satış")
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
                            var şablon = dc.Şablon_Ba_BsMutabakat.Where(a => a.ŞablonId == fatura.ŞablonId).FirstOrDefault();
                            #endregion

                            #region Save bill details
                            fatura.KullanıcıId = UserID;
                            fatura.CompanyId = SelectedCompanyID;
                            fatura.ExcelAdı = excelfile.FileName;
                            fatura.ŞablonAdı = şablon.ŞablonAdı;
                            fatura.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                            dc.BaBsMutabakatBilgileri.Add(fatura);
                            dc.SaveChanges();
                            #endregion

                            #region Getting excel column names

                            int bs = (şablon.BelgeSayısı)[0] - 65;
                            int ca = (şablon.CariAdı)[0] - 65;
                            int ck = (şablon.CariKodu)[0] - 65;
                            int t = (şablon.KdvHariçTutar)[0] - 65;
                            int tc = (şablon.TcKimlikNo)[0] - 65;
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

                                    DataColumn dcolColumn = new DataColumn("MutabakatBilgileriId", typeof(Int32));
                                    dcolColumn.DefaultValue = fatura.MutabakatBilgileriId;
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
                                        catch(Exception)
                                        {
                                            BaBsMutabakatBilgileri baBsMutabakatBilgileri = dc.BaBsMutabakatBilgileri.Where(x => x.MutabakatBilgileriId == fatura.MutabakatBilgileriId).FirstOrDefault<BaBsMutabakatBilgileri>();
                                            dc.BaBsMutabakatBilgileri.Remove(baBsMutabakatBilgileri);
                                            //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                            dc.SaveChanges();
                                            return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                                        }
                                    }
                                    excelReader.Close();
                                    #endregion

                                    #region Deleting unnecessary columns

                                    for (int i = 0; i < excel.Columns.Count; i++)
                                    {

                                        if (i != bs && i != ca && i != ck && i != t && i != tc && i != vn)
                                        {
                                            copyExcel.Columns.Remove("Column" + i);

                                        }


                                    }
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
                                                DestinationTableName = "BaBsMutabakatExcel",
                                                BulkCopyTimeout = 6000
                                            };
                                            sqlBulkCopy.ColumnMappings.Add("Column" + ck, "CariKodu");
                                            sqlBulkCopy.ColumnMappings.Add("Column" + ca, "CariAdı");
                                            sqlBulkCopy.ColumnMappings.Add("Column" + tc, "TcKimlikNo");
                                            sqlBulkCopy.ColumnMappings.Add("Column" + vn, "VergiNo");                                     
                                            sqlBulkCopy.ColumnMappings.Add("Column" + bs, "BelgeSayısı");
                                            sqlBulkCopy.ColumnMappings.Add("Column" + t, "KdvHariçTutar");
                                            sqlBulkCopy.ColumnMappings.Add("MutabakatBilgileriId", "MutabakatBilgileriId");

                                            var dataTable = copyExcel;
                                            sqlConnection.Open();

                                            sqlBulkCopy.WriteToServer(dataTable);

                                            scope.Complete();
                                            sqlBulkCopy.Close();
                                            sqlConnection.Close();
                                            sqlConnection.Dispose();
                                        }

                                        #endregion
                                        try
                                        {
                                            Add_BaBsExcelReconciliation(fatura.MutabakatBilgileriId);
                                        }
                                        catch
                                        {
                                            BaBsMutabakat baBsMutabakat = dc.BaBsMutabakat.Where(x => x.FaturaBilgileriId == fatura.MutabakatBilgileriId).FirstOrDefault<BaBsMutabakat>();
                                            dc.BaBsMutabakat.Remove(baBsMutabakat);
                                            //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                            dc.SaveChanges();
                                            return Json(new { success = false, message =Deneme.Resource.MissingCurrentInformations}, JsonRequestBehavior.AllowGet);

                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        BaBsMutabakatBilgileri baBsMutabakatBilgileri = dc.BaBsMutabakatBilgileri.Where(x => x.MutabakatBilgileriId == fatura.MutabakatBilgileriId).FirstOrDefault<BaBsMutabakatBilgileri>();
                                        dc.BaBsMutabakatBilgileri.Remove(baBsMutabakatBilgileri);
                                        //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                        dc.SaveChanges();
                                        return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                                    }
                                }
                                catch (Exception)
                                {
                                    BaBsMutabakatBilgileri baBsMutabakatBilgileri = dc.BaBsMutabakatBilgileri.Where(x => x.MutabakatBilgileriId == fatura.MutabakatBilgileriId).FirstOrDefault<BaBsMutabakatBilgileri>();
                                    dc.BaBsMutabakatBilgileri.Remove(baBsMutabakatBilgileri);
                                    //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                    dc.SaveChanges();
                                    return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                                }
                            }
                            catch (Exception)
                            {
                                BaBsMutabakatBilgileri baBsMutabakatBilgileri = dc.BaBsMutabakatBilgileri.Where(x => x.MutabakatBilgileriId == fatura.MutabakatBilgileriId).FirstOrDefault<BaBsMutabakatBilgileri>();
                                dc.BaBsMutabakatBilgileri.Remove(baBsMutabakatBilgileri);
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
            else
            {
                return Json(new { success = false, message = "Böyle bir yetkiniz bulunmamaktadır" }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        #region Delete BaBsExcel
        [HttpPost]
        [Authorize]
        public ActionResult Delete_BaBsExcel(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {

                var ba = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[3];
                var bs = @Server.HtmlEncode(Request.Cookies["UserOp"]["Yetki"]).ToString().Split(',')[8];

                BaBsMutabakatBilgileri babsMutabakatBilgileri = db.BaBsMutabakatBilgileri.Where(x => x.MutabakatBilgileriId == id).FirstOrDefault<BaBsMutabakatBilgileri>();
                var FaturaTipi = babsMutabakatBilgileri.FaturaTipi;
                if (ba == "1" && FaturaTipi == "Alış" || bs == "1" && FaturaTipi == "Satış")
                {
                    try
                    {
                        db.BaBsMutabakatBilgileri.Remove(babsMutabakatBilgileri);
                        db.BaBsMutabakatExcel.RemoveRange(db.BaBsMutabakatExcel.Where(x => x.MutabakatBilgileriId == id));
                        db.SaveChanges();
                        return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        return Json(new { success = false, message = Deneme.Resource.IOExeption }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new { success = false, message = "Yetkiniz Bulunmamaktadır"}, JsonRequestBehavior.AllowGet);
                }

            }
        }
        #endregion

        #region View BaBs Bilgileri
        [Authorize]
        public ActionResult BaBsExcel_View()
        {
            return View();
        }

        [Authorize]
        public ActionResult BaBsExcel()
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
                var BaBsExcelYükleme = dc.BaBsMutabakatBilgileri.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = BaBsExcelYükleme }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Add BaBs Excel Mutabakat 
        [HttpPost]
        [Authorize]
        public ActionResult Add_BaBsExcelReconciliation(/*BaBsMutabakat mutabakat*/ int id)
        {
            BaBsMutabakat mutabakat = new BaBsMutabakat();
            mutabakat.FaturaBilgileriId = id;
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
                var BaBs = dc.BaBsMutabakatBilgileri.Where(a => a.MutabakatBilgileriId == mutabakat.FaturaBilgileriId).FirstOrDefault();

                var totals = dc.BaBsMutabakatExcel.Where(a => a.MutabakatBilgileriId == mutabakat.FaturaBilgileriId).GroupBy(x => new { x.VergiNo, x.CariKodu, x.CariAdı, x.TcKimlikNo })
                                                                .Select(x => new
                                                                {
                                                                    Total = x.Sum(g => g.KdvHariçTutar),
                                                                    VN = x.Key.VergiNo,
                                                                    CK = x.Key.CariKodu,
                                                                    CA = x.Key.CariAdı,
                                                                    TC = x.Key.TcKimlikNo,
                                                                    C = x.Count()
                                                                }).ToList();

                #endregion
                #region Save mutabakat 
                mutabakat.CariSayısı = totals.Count.ToString();
                mutabakat.BelgeSayısı = dc.BaBsMutabakatExcel.Where(a => a.MutabakatBilgileriId == mutabakat.FaturaBilgileriId).ToList().Count.ToString();
                mutabakat.FaturaBilgileri = BaBs.Yıl + " " + BaBs.Ay + " " + BaBs.FaturaTipi;
                mutabakat.KullanıcıId = UserID;
                mutabakat.CompanyId = SelectedCompanyID;
                mutabakat.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                mutabakat.Tip = BaBs.FaturaTipi;
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
        #endregion


    }
}