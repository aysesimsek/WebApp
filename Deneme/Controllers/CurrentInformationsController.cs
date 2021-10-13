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

namespace Deneme.Controllers
{
    public class CurrentInformationsController : MyBaseController
    {
        #region View Cari Bilgileri 
        [Authorize]
        public ActionResult CEY_View()
        {
            return View();
        }

        [Authorize]
        public ActionResult CEY()
        {

            //string[] Keys = User.Identity.Name.Split(',');
            int UserID = Convert.ToInt32(User.Identity.Name);
            int SelectedCompanyID = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["UserOp"]["CompanyId"]));
            //if (Session["SelectedCompanyID"]!=null)
            //{
            //     SelectedCompanyID = Convert.ToInt32(Session["SelectedCompanyID"]);
            //}
            //else
            //{
            //     SelectedCompanyID = Convert.ToInt32(Keys[1]);
            //}
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var CariExcelYükleme = dc.CariBilgileri.Where(a => a.KullanıcıId == UserID && a.CompanyId == SelectedCompanyID).ToList();
                return Json(new { data = CariExcelYükleme }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Add Cari Detayları 
        [Authorize]
        [HttpPost]
        public ActionResult Add_CEY(HttpPostedFileBase excelfile, CariBilgileri cari)
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
                        var şablon = dc.Şablon_CariBilgileri.Where(c => c.ŞablonId == cari.ŞablonId).FirstOrDefault();
                        #endregion

                        #region Save bill details
                        cari.KullanıcıId = UserID;
                        cari.CompanyId = SelectedCompanyID;
                        cari.ExcelAdı = excelfile.FileName;
                        cari.ŞablonAdı = şablon.ŞablonAdı;
                        cari.OluşturmaTarihi = Convert.ToString(DateTime.Now);
                        dc.CariBilgileri.Add(cari);
                        dc.SaveChanges();
                        #endregion

                        #region Getting excel column names
                        int ct = (şablon.CariTipi)[0] - 65;
                        int ci = (şablon.Cariİl)[0] - 65;
                        int ca = (şablon.CariAdı)[0] - 65;
                        int ck = (şablon.CariKodu)[0] - 65;
                        int dt = (şablon.DövizTipi)[0] - 65;
                        int t = (şablon.Telefon)[0] - 65;
                        int tc = (şablon.TcKimlikNo)[0] - 65;
                        int vd = (şablon.VergiDairesi)[0] - 65;
                        int vn = (şablon.VergiNo)[0] - 65;
                        int a = (şablon.Adres)[0] - 65;
                        int e1 = (şablon.Email1)[0] - 65;
                        int e2 = (şablon.Email2)[0] - 65;
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


                                DataColumn dcolColumn = new DataColumn("CariBilgileriId", typeof(Int32));
                                dcolColumn.DefaultValue = cari.CariBilgileriId;
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
                                    catch (System.Exception)
                                    {
                                        CariBilgileri cariBilgileri = dc.CariBilgileri.Where(x => x.CariBilgileriId == cari.CariBilgileriId).FirstOrDefault<CariBilgileri>();
                                        dc.CariBilgileri.Remove(cariBilgileri);
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

                                //    if (i != a && i != e1 && i != e2 && i != ct && i != ci && i != ca && i != ck && i != dt && i != t && i != tc && i != vd && i != vn)
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
                                            DestinationTableName = "CariBilgileriExcel",
                                            BulkCopyTimeout = 6000
                                        };
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ck, "CariKodu");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ca, "CariAdı");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + dt, "DövizTipi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + vd, "VergiDairesi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + tc, "TcKimlikNo");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + t, "Telefon");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + vn, "VergiNo");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ct, "CariTipi");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + ci, "Cariİl");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + e1, "Email1");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + e2, "Email2");
                                        sqlBulkCopy.ColumnMappings.Add("Column" + a, "Adres");
                                        sqlBulkCopy.ColumnMappings.Add("CariBilgileriId", "CariBilgileriId");

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
                                catch (Exception e)
                                {
                                    CariBilgileri cariBilgileri = dc.CariBilgileri.Where(x => x.CariBilgileriId == cari.CariBilgileriId).FirstOrDefault<CariBilgileri>();
                                    dc.CariBilgileri.Remove(cariBilgileri);
                                    //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                    dc.SaveChanges();
                                    return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            catch(Exception)
                            {
                                CariBilgileri cariBilgileri = dc.CariBilgileri.Where(x => x.CariBilgileriId == cari.CariBilgileriId).FirstOrDefault<CariBilgileri>();
                                dc.CariBilgileri.Remove(cariBilgileri);
                                //dc.CariBilgileriExcel.RemoveRange(dc.CariBilgileriExcel.Where(x => x.CariBilgileriId == cari.CariBilgileriId));
                                dc.SaveChanges();
                                return Json(new { success = false, message = Deneme.Resource.SqlExeption }, JsonRequestBehavior.AllowGet);

                            }
                        }
                        catch(Exception)
                        {
                            CariBilgileri cariBilgileri = dc.CariBilgileri.Where(x => x.CariBilgileriId == cari.CariBilgileriId).FirstOrDefault<CariBilgileri>();
                            dc.CariBilgileri.Remove(cariBilgileri);
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
        [Authorize]
        [HttpGet]
        public ActionResult Add_CEY()
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                //string[] Keys = User.Identity.Name.Split(',');
                int UserID = Convert.ToInt32(User.Identity.Name);
                var CariBilgileriŞablonu = dc.Şablon_CariBilgileri.Where(a => a.KullanıcıId == UserID).ToList();


                List<SelectListItem> ŞablonId = (from item in CariBilgileriŞablonu
                                                 select new SelectListItem
                                                 {
                                                     Text = item.ŞablonAdı,
                                                     Value = item.ŞablonId.ToString()
                                                 }).ToList();


                ViewBag.ŞablonId = ŞablonId;

                return View();
            }
        }
        #endregion


        #region Show Cari Detayları
        [HttpGet]
        [Authorize]
        public ActionResult Show_CEYView(int id)
        {

            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                var v = dc.CariBilgileriExcel.Where(a => a.CariBilgileriId == id).ToList();
                var jsonResult = Json(new { data = v }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult Show_CEY(int id)
        {
            ViewBag.CariId = id;
            return View();

        }
        #endregion

        #region Delete Cari ve Cari Detayları
        [Authorize]
        [HttpPost]
        public ActionResult Delete_CEY(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                try
                {
                    CariBilgileri cari = db.CariBilgileri.Where(x => x.CariBilgileriId == id).FirstOrDefault<CariBilgileri>();
                    CariBilgileriExcel cariExcel = db.CariBilgileriExcel.Where(x => x.CariBilgileriId == id).FirstOrDefault<CariBilgileriExcel>();
                    db.CariBilgileri.Remove(cari);
                    db.CariBilgileriExcel.RemoveRange(db.CariBilgileriExcel.Where(x => x.CariBilgileriId == id));
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
                }
                catch(Exception)
                {
                    return Json(new { success = false, message = Deneme.Resource.IOExeption }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Delete_CEYExcel(int id)
        {
            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
              
                CariBilgileriExcel cariExcel = db.CariBilgileriExcel.Where(x => x.CariId == id).FirstOrDefault<CariBilgileriExcel>();
                db.CariBilgileriExcel.Remove(cariExcel);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.DeletedSuccessfully }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion


        #region Add CEYDetail
        [HttpGet]
        [Authorize]
        public ActionResult Add_CEYDetail(int id)
        {
            CariBilgileriExcel c = new CariBilgileriExcel();
            c.CariBilgileriId = id;
            return View(c);

        }

        [HttpPost]
        [Authorize]
        public ActionResult Add_CEYDetail(CariBilgileriExcel cariBilgileriExcel)
        {

            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
                db.CariBilgileriExcel.Add(cariBilgileriExcel);
                db.SaveChanges();
                return Json(new { success = true, message = Deneme.Resource.SavedSuccessfully }, JsonRequestBehavior.AllowGet);

            }

        }
        #endregion

        #region Edit CEY
        [HttpGet]
        [Authorize]
        public ActionResult Edit_CEY(int id)
        {
          
                using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
                {
                    return View(db.CariBilgileriExcel.Where(x => x.CariId == id).FirstOrDefault<CariBilgileriExcel>());
                }
            

        }

        [HttpPost]
        [Authorize]
        public ActionResult Edit_CEY(CariBilgileriExcel cariBilgileriExcel)
        {

            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {
        
     
                    db.Entry(cariBilgileriExcel).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = Deneme.Resource.UpdatedSuccessfully }, JsonRequestBehavior.AllowGet);
                
            }

        }

        #endregion

    }
}