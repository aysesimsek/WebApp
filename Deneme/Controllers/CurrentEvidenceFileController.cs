using Deneme.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Deneme.Controllers
{
    public class CurrentEvidenceFileController : MyBaseController
    {
        [HttpGet]
        public ActionResult VerifyMutabakat(string id)
        {
            using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                var t = dc.CariMutabakatDetay.Where(a => a.MutabakatKabulLink == new Guid(id)).FirstOrDefault();
                var f = dc.CariMutabakatDetay.Where(a => a.MutabakatRetLink == new Guid(id)).FirstOrDefault();

                if (t != null)
                {
                    t.OnayDurumu = "Approved";
                    dc.SaveChanges();
                    return RedirectToAction("AfterMailView");
                }
                else if (f != null)
                {
                    return RedirectToAction("AddEvidenceFile", "CurrentEvidenceFile", new { id = f.CariMutabakatDetayId });
                }
            }
            return null;
        }

        [HttpGet]
        public ActionResult AddEvidenceFile(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddEvidenceFile(HttpPostedFileBase file, int id)
        {

            if (file != null)
            {
                FileInfo dosyaBilgisi = new FileInfo(file.FileName);

                string FileName = dosyaBilgisi.Name.Substring(0, dosyaBilgisi.Name.Length - dosyaBilgisi.Extension.Length);
                FileName += "-" + Guid.NewGuid().ToString().Replace("-", "") + dosyaBilgisi.Extension;
                string path = Server.MapPath("~/EvidenceFiles/" + FileName);
                file.SaveAs(path);

                //string kayitYeri = "EvidenceFiles"+ "/" + FileName;
                using (YirmibesYazilimMutabakatEntities1 dc = new YirmibesYazilimMutabakatEntities1())
                {

                    var f = dc.CariMutabakatDetay.Where(a => a.CariMutabakatDetayId == id).FirstOrDefault();
                    f.OnayDurumu = "Not Approved";
                    f.FileName = FileName;



                    dc.SaveChanges();
                }
            }

            return RedirectToAction("AfterMailView");
        }
        [HttpGet]
        public ActionResult AfterMailView()
        {

            return View();
        }

    }
}