using System;
using System.Linq;
using System.Web.Mvc;
using Deneme.Models;
using System.Data;
using System.Collections.Generic;

namespace Deneme.Controllers
{

    public class HomeController : MyBaseController
    {

        [Authorize]
        public ActionResult Index()
        {


            #region BA Mutabakat
            //int counterBaDeliveredMails = 0;
            //int counterBaViewedMails = 0;
            //int counterBaClickedMails = 0;
            //int counterBaSpamMails = 0;
            //int counterBaInvalidMails = 0;

            //int counterBaApproved = 0;
            //int counterBaNotApproved = 0;
            //int counterBaPending = 0;
            //int totalBaReconciliation = 0;
            List<int> BamailStatusValues = new List<int>();
            #endregion

            #region Last BA Mutabakat

            int counterLastBaApproved = 0;
            int counterLastBaNotApproved = 0;
            int counterLastBaPending = 0;
            int totalLastBaReconciliation = 0;
            

            #endregion
            #region Last BS Mutabakat

            int counterLastBsApproved = 0;
            int counterLastBsNotApproved = 0;
            int counterLastBsPending = 0;
            int totalLastBsReconciliation = 0;

            #endregion
            #region Last CARİ Mutabakat

            int counterLastCariApproved = 0;
            int counterLastCariNotApproved = 0;
            int counterLastCariPending = 0;
            int totalLastCariReconciliation = 0;

            #endregion


            #region BS Mutabakat
            //int counterBsDeliveredMails = 0;
            //int counterBsViewedMails = 0;
            //int counterBsClickedMails = 0;
            //int counterBsSpamMails = 0;
            //int counterBsInvalidMails = 0;

            //int counterBsApproved = 0;
            //int counterBsNotApproved = 0;
            //int counterBsPending = 0;
            //int totalBsReconciliation = 0;

            List<int> BsmailStatusValues = new List<int>();
            #endregion

            #region Cari Mutabakat
            //int counterCariDeliveredMails = 0;
            //int counterCariViewedMails = 0;
            //int counterCariClickedMails = 0;
            //int counterCariSpamMails = 0;
            //int counterCariInvalidMails = 0;

            //int counterCariApproved = 0;
            //int counterCariNotApproved = 0;
            //int counterCariPending = 0;
            //int totalCariReconciliation = 0;

            List<int> CarimailStatusValues = new List<int>();
            #endregion


            List<string> mailStatus = new List<string>();

            using (YirmibesYazilimMutabakatEntities1 db = new YirmibesYazilimMutabakatEntities1())
            {

                int id = Convert.ToInt32(User.Identity.Name);
                int SelectedCompanyID = Convert.ToInt32(Server.HtmlEncode(Request.Cookies["UserOp"]["CompanyId"]));

                #region Last Ba
                var LastBat = db.BaBsMutabakat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID && a.Tip == "Alış").ToList();

                

                if (LastBat.Count != 0)
                {

                    var LastBa = db.BaBsMutabakat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID && a.Tip == "Alış").Max(a => a.MutabakatId);
                    ViewBag.BaId = LastBa;
                    var LastBaBilgileri = db.BaBsMutabakat.Where(a => a.MutabakatId == LastBa).FirstOrDefault();
                    var LastBaDetay = db.BaBsMutabakatDetay.Where(a => a.MutabakatId == LastBa).ToList();
                    ViewBag.LastBaBilgileri = LastBaBilgileri.FaturaBilgileri.Split(' ')[0]+ " " + LastBaBilgileri.FaturaBilgileri.Split(' ')[1];

                    foreach (var itemStatus in LastBaDetay)
                    {

                        if (itemStatus.OnayDurumu == "Approved" || itemStatus.OnayDurumu == "Onaylandı")
                        {
                            counterLastBaApproved++;
                        }
                        else if (itemStatus.OnayDurumu == "Not Approved" || itemStatus.OnayDurumu == "Onaylanmadı")
                        {
                            counterLastBaNotApproved++;
                        }
                        else if (itemStatus.OnayDurumu == "Pending" || itemStatus.OnayDurumu == "Beklemede")
                        {
                            counterLastBaPending++;
                        }
                    }
                }
                #endregion
                #region Last Bs
                var LastBst = db.BaBsMutabakat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID && a.Tip == "Satış").ToList();

                if (LastBst.Count != 0)
                {
                    var LastBs = db.BaBsMutabakat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID && a.Tip == "Satış").Max(a => a.MutabakatId);
                    ViewBag.BsId = LastBs;
                    var LastBsBilgileri = db.BaBsMutabakat.Where(a => a.MutabakatId == LastBs).FirstOrDefault();
                    ViewBag.LastBsBilgileri = LastBsBilgileri.FaturaBilgileri.Split(' ')[0] + " " + LastBsBilgileri.FaturaBilgileri.Split(' ')[1];
                    var LastBsDetay = db.BaBsMutabakatDetay.Where(a => a.MutabakatId == LastBs).ToList();

                    foreach (var itemStatus in LastBsDetay)
                    {

                        if (itemStatus.OnayDurumu == "Approved" || itemStatus.OnayDurumu == "Onaylandı")
                        {
                            counterLastBsApproved++;
                        }
                        else if (itemStatus.OnayDurumu == "Not Approved" || itemStatus.OnayDurumu == "Onaylanmadı")
                        {
                            counterLastBsNotApproved++;
                        }
                        else if (itemStatus.OnayDurumu == "Pending" || itemStatus.OnayDurumu == "Beklemede")
                        {
                            counterLastBsPending++;
                        }
                    }
                }
                #endregion
                #region Last Cari
                var LastCarit = db.CariMutabakkat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID).ToList();
                if(LastCarit.Count != 0)
                {
                    var LastCari = db.CariMutabakkat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID).Max(a => a.CariMutabakatId);
                    ViewBag.CariId = LastCari;
                    var LastCariBilgileri = db.CariMutabakkat.Where(a => a.CariMutabakatId == LastCari).FirstOrDefault();
                    ViewBag.LastCariBilgileri = LastCariBilgileri.Yıl + " " + LastCariBilgileri.Ay;
                    var LastCariDetay = db.CariMutabakatDetay.Where(a => a.CariMutabakatDetayId == LastCari).ToList();


                    foreach (var itemStatus in LastCariDetay)
                    {

                        if (itemStatus.OnayDurumu == "Approved" || itemStatus.OnayDurumu == "Onaylandı")
                        {
                            counterLastCariApproved++;
                        }
                        else if (itemStatus.OnayDurumu == "Not Approved" || itemStatus.OnayDurumu == "Onaylanmadı")
                        {
                            counterLastCariNotApproved++;
                        }
                        else if (itemStatus.OnayDurumu == "Pending" || itemStatus.OnayDurumu == "Beklemede")
                        {
                            counterLastCariPending++;
                        }
                    }

                }
                #endregion


                #region Total Ba-Bs Mutabakat
                //var BaBsMutabakatYükleme = db.BaBsMutabakat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID).ToList();

                //foreach (var itemDetail in BaBsMutabakatYükleme)
                //{
                //        var ReconciliationDetail = db.BaBsMutabakatDetay.Where(a => a.MutabakatId == itemDetail.MutabakatId).ToList();
                //    foreach (var itemStatus in ReconciliationDetail)
                //    {

                //        if (itemStatus.MailDurumu == "Delivered" || itemStatus.MailDurumu == "İletildi")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaDeliveredMails++;
                //            }
                //            else
                //            {
                //                counterBsDeliveredMails++;
                //            }
                //        }
                //        else if (itemStatus.MailDurumu == "Viewed" || itemStatus.MailDurumu == "Görüntülendi")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaViewedMails++;
                //            }
                //            else
                //            {
                //                counterBsViewedMails++;
                //            }
                //        }
                //        else if (itemStatus.MailDurumu == "Clicked" || itemStatus.MailDurumu == "Tıklandı")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaClickedMails++;
                //            }
                //            else
                //            {
                //                counterBsClickedMails++;
                //            }
                //        }
                //        else if (itemStatus.MailDurumu == "Spam" || itemStatus.MailDurumu == "Spam")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaSpamMails++;
                //            }
                //            else
                //            {
                //                counterBsSpamMails++;
                //            }
                //        }
                //        else if (itemStatus.MailDurumu == "Invalid Mail" || itemStatus.MailDurumu == "Geçersiz Mail Adresi")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaInvalidMails++;

                //            }
                //            else
                //            {
                //                counterBsInvalidMails++;
                //            }
                //        }

                //        if (itemStatus.OnayDurumu == "Approved" || itemStatus.OnayDurumu == "Onaylandı")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaApproved++;

                //            }
                //            else
                //            {
                //                counterBsApproved++;
                //            }
                //        }
                //        else if (itemStatus.OnayDurumu == "Not Approved" || itemStatus.OnayDurumu == "Onaylanmadı")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaNotApproved++;
                //            }
                //            else
                //            {
                //                counterBsNotApproved++;
                //            }
                //        }
                //        else if (itemStatus.OnayDurumu == "Pending" || itemStatus.OnayDurumu == "Beklemede")
                //        {
                //            if (itemDetail.Tip == "Alış")
                //            {
                //                counterBaPending++;

                //            }
                //            else
                //            {
                //                counterBsPending++;
                //            }
                //        }
                //    }
                  
                //}
                //#endregion

                //#region Cari Mutabakat
                //var CariMutabakatYükleme = db.CariMutabakkat.Where(a => a.KullanıcıId == id && a.CompanyId == SelectedCompanyID).ToList();
                //foreach (var itemDetail in CariMutabakatYükleme)
                //{
                //    var CurrentReconciliationDetail = db.CariMutabakatDetay.Where(a => a.CariMutabakatId == itemDetail.CariMutabakatId).ToList();
                //    foreach (var itemStatus in CurrentReconciliationDetail)
                //    {

                //        if (itemStatus.MailDurumu == "Delivered" || itemStatus.MailDurumu == "İletildi")
                //        {
                          
                //                counterCariDeliveredMails++;
                           
                //        }
                //        else if (itemStatus.MailDurumu == "Viewed" || itemStatus.MailDurumu == "Görüntülendi")
                //        {
                           
                //                counterCariViewedMails++;

                //        }
                //        else if (itemStatus.MailDurumu == "Clicked" || itemStatus.MailDurumu == "Tıklandı")
                //        {
                           
                //                counterCariClickedMails++;
                           
                            
                           
                //        }
                //        else if (itemStatus.MailDurumu == "Spam" || itemStatus.MailDurumu == "Spam")
                //        {
                           
                //                counterCariSpamMails++;
                           
                            
                //        }
                //        else if (itemStatus.MailDurumu == "Invalid Mail" || itemStatus.MailDurumu == "Geçersiz Mail Adresi")
                //        {
                //              counterCariInvalidMails++;

                           
                //        }

                //        if (itemStatus.OnayDurumu == "Approved" || itemStatus.OnayDurumu == "Onaylandı")
                //        {
                           
                //                counterCariApproved++;

                          
                //        }
                //        else if (itemStatus.OnayDurumu == "Not Approved" || itemStatus.OnayDurumu == "Onaylanmadı")
                //        {
                            
                //                counterCariNotApproved++;
                         
                //        }
                //        else if (itemStatus.OnayDurumu == "Pending" || itemStatus.OnayDurumu == "Beklemede")
                //        {
                            
                //                counterCariPending++;

                          
                //        }
                //    }
                //}
                #endregion

            }

            #region Ba Onay Durumu
            //totalBaReconciliation = counterBaApproved + counterBaNotApproved + counterBaPending;
            //if (totalBaReconciliation == 0)
            //{ totalBaReconciliation++; }
            //counterBaViewedMails += counterBaClickedMails;
            //ViewBag.counterBaApproved = counterBaApproved;
            //ViewBag.counterBaNotApproved = counterBaNotApproved;
            //ViewBag.counterBaPending = counterBaPending;
            //ViewBag.totalBaReconciliation = totalBaReconciliation;
            #endregion

            #region Last Ba Onay Durumu
            totalLastBaReconciliation = counterLastBaApproved + counterLastBaNotApproved + counterLastBaPending;
            if (totalLastBaReconciliation == 0)
            { totalLastBaReconciliation++; }
       
            ViewBag.counterLastBaApproved = counterLastBaApproved;
            ViewBag.counterLastBaNotApproved = counterLastBaNotApproved;
            ViewBag.counterLastBaPending = counterLastBaPending;
            ViewBag.totalLastBaReconciliation = totalLastBaReconciliation;
            #endregion

            #region Bs Onay Durumu
            //totalBsReconciliation = counterBsApproved + counterBsNotApproved + counterBsPending;
            //if (totalBsReconciliation == 0)
            //{ totalBsReconciliation++; }
            //counterBsViewedMails += counterBsClickedMails;
            //ViewBag.counterBsApproved = counterBsApproved;
            //ViewBag.counterBsNotApproved = counterBsNotApproved;
            //ViewBag.counterBsPending = counterBsPending;
            //ViewBag.totalBsReconciliation = totalBsReconciliation;
            #endregion

            #region Last Bs Onay Durumu
            totalLastBsReconciliation = counterLastBsApproved + counterLastBsNotApproved + counterLastBsPending;
            if (totalLastBsReconciliation == 0)
            { totalLastBsReconciliation++; }

            ViewBag.counterLastBsApproved = counterLastBsApproved;
            ViewBag.counterLastBsNotApproved = counterLastBsNotApproved;
            ViewBag.counterLastBsPending = counterLastBsPending;
            ViewBag.totalLastBsReconciliation = totalLastBsReconciliation;
            #endregion

            #region Cari Onay Durumu
            //totalCariReconciliation = counterCariApproved + counterCariNotApproved + counterCariPending;
            //if (totalCariReconciliation == 0)
            //{ totalCariReconciliation++; }
            //counterCariViewedMails += counterCariClickedMails;
            //ViewBag.counterCariApproved = counterCariApproved;
            //ViewBag.counterCariNotApproved = counterCariNotApproved;
            //ViewBag.counterCariPending = counterCariPending;
            //ViewBag.totalCariReconciliation = totalCariReconciliation;
            #endregion

            #region Last Cari Onay Durumu
            totalLastCariReconciliation = counterLastCariApproved + counterLastCariNotApproved + counterLastCariPending;
            if (totalLastCariReconciliation == 0)
            { totalLastCariReconciliation++; }

            ViewBag.counterLastCariApproved = counterLastCariApproved;
            ViewBag.counterLastCariNotApproved = counterLastCariNotApproved;
            ViewBag.counterLastCariPending = counterLastCariPending;
            ViewBag.totalLastCariReconciliation = totalLastCariReconciliation;
            #endregion

            #region Ba Mail Durumu
            //mailStatus.Add(Deneme.Resource.Delivered);
            //mailStatus.Add(Deneme.Resource.Viewed);
            //mailStatus.Add(Deneme.Resource.Clicked);
            //mailStatus.Add(Deneme.Resource.Spam);
            //mailStatus.Add(Deneme.Resource.InvalidMailChart);

            //BamailStatusValues.Add(counterBaDeliveredMails);
            //BamailStatusValues.Add(counterBaViewedMails);
            //BamailStatusValues.Add(counterBaClickedMails);
            //BamailStatusValues.Add(counterBaSpamMails);
            //BamailStatusValues.Add(counterBaInvalidMails);

            //ViewBag.mailStatus = mailStatus;
            //ViewBag.BamailStatusValues = BamailStatusValues;
            #endregion

            return View();


        }
    }
}