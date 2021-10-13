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
using System.Threading;

namespace Deneme.Controllers
{
    public class InvoiceController : MyBaseController
    {
        #region Show Fatura Detayları
        [HttpGet]
        [Authorize]
        public ActionResult Show_FAEYView(int id)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var v = dc.FaturaBilgileriExcel.Where(a => a.FaturaBilgileriId == id).ToList();
                var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_FAEY(int id)
        {
            ViewBag.FaturaId = id;
            return View();

        }
        #endregion

        #region Add Fatura Detayları
        [HttpGet]
        [Authorize]
        public ActionResult Add_FAEY()
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

                List<int> Yıl = new List<int> { 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018 };

                ViewBag.Yıl = Yıl;
                List<SelectListItem> ŞablonId = (from item in FaturaAktarmaŞablonu
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
        public ActionResult Add_FAEY(HttpPostedFileBase excelfile, FaturaBilgileri fatura)
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
                        var şablon = dc.Şablon_FaturaAktarım.Where(a => a.ŞablonId == fatura.ŞablonId).FirstOrDefault();
                        #endregion

                        #region Save bill details
                        fatura.KullanıcıId = UserID;
                        fatura.CompanyId = SelectedCompanyID;
                        fatura.ExcelAdı = excelfile.FileName;
                        fatura.ŞablonAdı = şablon.ŞablonAdı;
                        fatura.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                        dc.FaturaBilgileri.Add(fatura);
                        dc.SaveChanges();
                        #endregion

                        #region Getting excel column names
                        int be = (şablon.BelgeNumarası)[0] - 65;
                        int bt = (şablon.BelgeTarihi)[0] - 65;
                        int ca = (şablon.CariAdı)[0] - 65;
                        int ck = (şablon.CariKodu)[0] - 65;
                        int dt = (şablon.DövizTipi)[0] - 65;
                        int t = (şablon.KdvHariçTutar)[0] - 65;
                        int tc = (şablon.TcKimlikNo)[0] - 65;
                        int vd = (şablon.VergiDairesi)[0] - 65;
                        int vn = (şablon.VergiNo)[0] - 65;
                        #endregion

                        try // IOExeption
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
                 
                                DataColumn dcolColumn = new DataColumn("FaturaBilgileriId", typeof(Int32));
                                dcolColumn.DefaultValue = fatura.FaturaId;
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
                                    catch (System.ArgumentException)
                                    {
                                        FaturaBilgileri faturaBilgileri = dc.FaturaBilgileri.Where(x => x.FaturaId == fatura.FaturaId).FirstOrDefault<FaturaBilgileri>();
                                        dc.FaturaBilgileri.Remove(faturaBilgileri);
                                        dc.FaturaBilgileriExcel.RemoveRange(dc.FaturaBilgileriExcel.Where(x => x.FaturaBilgileriId == fatura.FaturaId));
                                        dc.SaveChanges();
                                        return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);
                                    }

                                }
                                excelReader.Close();
                                #endregion

                                #region Deleting unnecessary columns

                                //for (int i = 0; i < excel.Columns.Count; i++)
                                //{

                                //    if (i != be && i != bt && i != ca && i != ck && i != dt && i != t && i != tc && i != vd && i != vn)
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
                                            DestinationTableName = "FaturaBilgileriExcel",
                                            BulkCopyTimeout = 6000
                                        };
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ck, "CariKodu");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ca, "CariAdı");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + dt, "DövizTipi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + vd, "VergiDairesi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + tc, "TcKimlikNo");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + t, "KdvHariçTutar");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + vn,"VergiNo");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + bt,"BelgeTarihi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + be, "BelgeNumarası");
                                        sqlBulkCopy.ColumnMappings.Add("FaturaBilgileriId", "FaturaBilgileriId");

                                        var dataTable = copyExcel;
                                        sqlConnection.Open();

                                        sqlBulkCopy.WriteToServer(dataTable);

                                        scope.Complete();
                                        sqlBulkCopy.Close();
                                        sqlConnection.Close();
                                        sqlConnection.Dispose();
                                    }
                                    #endregion
                                }
                                catch (Exception)
                                {

                                    FaturaBilgileri faturaBilgileri = dc.FaturaBilgileri.Where(x => x.FaturaId == fatura.FaturaId).FirstOrDefault<FaturaBilgileri>();
                                    dc.FaturaBilgileri.Remove(faturaBilgileri);
                                    dc.FaturaBilgileriExcel.RemoveRange(dc.FaturaBilgileriExcel.Where(x => x.FaturaBilgileriId == fatura.FaturaId));
                                    dc.SaveChanges();
                                    return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            catch (Exception)
                            {
                                FaturaBilgileri faturaBilgileri = dc.FaturaBilgileri.Where(x => x.FaturaId == fatura.FaturaId).FirstOrDefault<FaturaBilgileri>();
                                dc.FaturaBilgileri.Remove(faturaBilgileri);
                                dc.FaturaBilgileriExcel.RemoveRange(dc.FaturaBilgileriExcel.Where(x => x.FaturaBilgileriId == fatura.FaturaId));
                                dc.SaveChanges();
                                return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);
                            }

                        }
                        catch (Exception)
                        {
                            FaturaBilgileri faturaBilgileri = dc.FaturaBilgileri.Where(x => x.FaturaId == fatura.FaturaId).FirstOrDefault<FaturaBilgileri>();
                            dc.FaturaBilgileri.Remove(faturaBilgileri);
                            dc.FaturaBilgileriExcel.RemoveRange(dc.FaturaBilgileriExcel.Where(x => x.FaturaBilgileriId == fatura.FaturaId));
                            dc.SaveChanges();
                            return Json(new { success = false, message = Deneme.Resource.IOExeption }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = Deneme.Resource.WrongFileExtension }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region Delete Fatura ve Fatura Detayları
        [HttpPost]
        [Authorize]
        public ActionResult Delete_FAEY(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                try
                {
                    FaturaBilgileri faturaBilgileri = db.FaturaBilgileri.Where(x => x.FaturaId == id).FirstOrDefault<FaturaBilgileri>();
                    db.FaturaBilgileri.Remove(faturaBilgileri);
                    db.FaturaBilgileriExcel.RemoveRange(db.FaturaBilgileriExcel.Where(x => x.FaturaBilgileriId == id));
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = Deneme.Resource.IOExeption }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region View Fatura Bilgileri
        [Authorize]
        public ActionResult FAEY_View()
        {
            return View();
        }

        [Authorize]
        public ActionResult FAEY()
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
                var FaturaExcelYükleme = dc.FaturaBilgileri.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = FaturaExcelYükleme }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}