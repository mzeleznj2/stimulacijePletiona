using Newtonsoft.Json;
using Stimulacije_2022.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Stimulacije_2022.Controllers
{
    public class HomeController : Controller
    {
        private dbNautilusEntities4 context = new dbNautilusEntities4();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Stimulacije()
        {
            if (Session["User"] != null)
            {                                          
                Stimulacije podaci = new Stimulacije();

                ViewBag.Stimulacije = podaci.vratiPodatke();

                return View("Stimulacije");                
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
       
        public ActionResult Login()
        {           
            return View();
        }


        public ActionResult Logiranje(string username, string password)
        {
            string user = "";
            string pass = "";

            user = username;
            pass = password;

            Response.Write("Imena " + user + " " + pass);

            if (user != "zaklina" || pass != "sedlarevicz")
            {
                @TempData["ErrorMessage"] = "Krivi username ili password!";
                return RedirectToAction("Login");               
            }

           
            Session["User"] = user;
            Session["Password"] = pass;

            return RedirectToAction("Index");
        }
               

        public ActionResult StimulacijeZaObradu()
        {
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            int mjesecPrije = oneMonthAgo.Month;
            int godinaPrije = oneMonthAgo.Year;

            if (Session["User"] != null)
            {
                return View(context.StimulacijaPL.Where(a => a.mjesec == mjesecPrije).ToList());
            }
            else 
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult PotvrdiStimulacije()
        {
            TempData["poruka"] = "";
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            int mjesecPrije = oneMonthAgo.Month;
            int godinaPrije = oneMonthAgo.Year;

            var traziStimulacije = context.StimulacijaPL.Where(a => (a.godina == godinaPrije) && (a.mjesec == mjesecPrije) 
                                                               && (a.status == "1")).ToList();

            var traziStimulacije2 = context.StimulacijaPL.Where(a => (a.godina == godinaPrije) && (a.mjesec == mjesecPrije)
                                                               && (a.status == "2")).ToList();

            var traziStimulacije3 = context.StimulacijaPL.Where(a => (a.godina == godinaPrije) && (a.mjesec == mjesecPrije)
                                                               && (a.status == "3")).ToList();

            if (traziStimulacije.Count > 0)
            {

                foreach (var stim in traziStimulacije)
                {                    
                    stim.status = "2";
                }                

                context.SaveChanges();


                TempData["poruka"] = "Potvrdili ste stimulacije za obradu.";
            }
            else if (traziStimulacije2.Count > 0)
            {
                TempData["poruka"] = "Stimulacije su već bile potvrđene.";
            }
            else if (traziStimulacije3.Count > 0)
            {
                TempData["poruka"] = "Stimulacije su već prebačene u plaću.";
            }
            else
            {
                TempData["poruka"] = "Stimulacije nisu još kopirane za obradu.";
            }
            
            return RedirectToAction("StimulacijeZaObradu");
        }
       
        public ActionResult Edit(int operator1, int godina, int mjesec)
        {
            if (Session["User"] != null)
            {
                var data = context.StimulacijaPL.Where(x => (x.@operator == operator1) && (x.godina == godina) && (x.mjesec == mjesec)).FirstOrDefault();

                return View(data);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult Edit(StimulacijaPL stimulacijeEdit)
        {
            int operator2, godina2, mjesec2;

            if (ModelState.IsValid) {
                using (var context = new dbNautilusEntities4())
                {
                    var data = context.StimulacijaPL.Where(x => x.idStimulacije == stimulacijeEdit.idStimulacije).FirstOrDefault();

                     operator2 = Convert.ToInt32(data.@operator);
                     godina2 = Convert.ToInt32(data.godina);
                     mjesec2 = Convert.ToInt32(data.mjesec);


                    if (data != null && (stimulacijeEdit.Efikasnost != 0 && stimulacijeEdit.EfPremaNormi != 0) && data.Stimulacija > 0)
                    {
                        data.Efikasnost = stimulacijeEdit.Efikasnost is null ? 0 : stimulacijeEdit.Efikasnost;
                        data.EfPremaNormi = stimulacijeEdit.EfPremaNormi is null ? 0 : stimulacijeEdit.EfPremaNormi;
                        data.Norma = stimulacijeEdit.Norma is null ? 0 : stimulacijeEdit.Norma;
                        data.PostotakDestim = stimulacijeEdit.PostotakDestim is null ? 0 : stimulacijeEdit.PostotakDestim;
                        data.Stimulacija = stimulacijeEdit.Stimulacija is null ? 0 : stimulacijeEdit.Stimulacija;
                        data.Ukupno = stimulacijeEdit.Stimulacija - stimulacijeEdit.PostotakDestim;
                        data.status = stimulacijeEdit.status;

                        context.SaveChanges();

                        TempData["uspjeh"] = "Uspješno ste ažurirali operatora " + data.@operator;

                        return RedirectToAction("StimulacijeZaObradu");

                    }
                    else
                    {
                        TempData["nule"] = "Efikasnost i/ili efikasnost prema normi mora imati vrijednost veću od 0!";
                        return RedirectToAction ("Edit", new { operator1 = operator2, godina = godina2, mjesec = mjesec2 });

                    }
                    
                }
                
            }
            
            return View(stimulacijeEdit);
        }

        public ActionResult TraziBolovanja()
        {
            if (Session["User"] != null)
            {
                ViewData["test"] = 0;

                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }



        public ActionResult GetData(string matBr)
        {
            
            if (matBr != "")
            {
                TempData["matbr"] = matBr;
                return RedirectToAction("PartialViewDestimulacije");
            }
            else
            {
                TempData["greska"] = "Niste dobro upisali matične brojeve!";
                return RedirectToAction("TraziBolovanja");
            }         
           
        }



        [HttpGet]
        public ActionResult PartialViewDestimulacije()
        {
            if (Session["User"] != null)
            {
                Stimulacije bol = new Stimulacije();
                string tekst = TempData["matbr"].ToString();

                List<Bolovanja> bolovanjaLista = bol.vratiListuBolovanja(tekst);
                //return Json(new { data = bolovanjaLista }, JsonRequestBehavior.AllowGet);

                ViewData["test"] = 1;

                return View("TraziBolovanja", bolovanjaLista);
            }
            else
            {
                return RedirectToAction("Login");
            }
              
        }


        public ActionResult DodajStimulaciju()
        {
            return View();
        }


    
        public ActionResult ProvjeriKod(int kod)
        {
            Stimulacije stimulacije = new Stimulacije();
            int destimulacija;
            destimulacija = stimulacije.VratiDestimulaciju(kod);
                      
            var postojiOperator = context.OPERATORS.Where(o => o.OpCode == kod).FirstOrDefault();

            //kreiramo novu destimulaciju popunimo godinu/mjesec/operatora/status - ostalo za dodavanje
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);
            int mjesecPrije = oneMonthAgo.Month;
            int godinaPrije = oneMonthAgo.Year;

            var data = context.StimulacijaPL.Where(x => (x.@operator == kod) && (x.godina == godinaPrije) && (x.mjesec == mjesecPrije)).FirstOrDefault();          

            if (data == null && postojiOperator != null)
            {
                var podatak = new StimulacijaPL()
                {
                    mjesec = mjesecPrije,
                    godina = godinaPrije,
                    @operator = kod,
                    ime = postojiOperator.FirstName,
                    prezime = postojiOperator.LastName,
                    PostotakDestim = destimulacija,
                    status = "2"
                };
                context.StimulacijaPL.Add(podatak);
                context.SaveChanges();

                return Json(new { success = true, url = Url.Action("Edit", new {operator1 = podatak.@operator, godina = podatak.godina, mjesec = podatak.mjesec }) }, JsonRequestBehavior.AllowGet);
            }
            else if (data == null && postojiOperator == null)
            {               
                return Json(new { success = false, error = "Ne postoji takav operator!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {               
                return Json(new { success = false, error = "Operator već postoji u stimulacijama!" }, JsonRequestBehavior.AllowGet);
            }
           
        }


        public ActionResult Logout()
        {
            Session["User"] = "";
            Session["Password"] = "";

            return RedirectToAction("Login");
        }
               

    }
}