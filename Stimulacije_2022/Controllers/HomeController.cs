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
                //List<Zdruzeno> listaPodataka = new List<Zdruzeno>();
                              

                Stimulacije podaci = new Stimulacije();
                //podaci = null;
                
                //listaPodataka = podaci.vratiPodatke();

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
            
            return View(context.StimulacijaPL.ToList());
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
            var data = context.StimulacijaPL.Where(x => (x.@operator == operator1) && (x.godina == godina) && (x.mjesec == mjesec)).FirstOrDefault();

            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(StimulacijaPL stimulacijeEdit)
        {
            if (ModelState.IsValid) {
                using (var context = new dbNautilusEntities4())
                {
                    var data = context.StimulacijaPL.Where(x => x.idStimulacije == stimulacijeEdit.idStimulacije).FirstOrDefault();

                    if (data != null)
                    {
                        data.Efikasnost = stimulacijeEdit.Efikasnost;
                        data.EfPremaNormi = stimulacijeEdit.EfPremaNormi;
                        data.Norma = stimulacijeEdit.Norma;
                        data.PostotakDestim = stimulacijeEdit.PostotakDestim;
                        data.Stimulacija = stimulacijeEdit.Stimulacija;
                        data.Ukupno = stimulacijeEdit.Ukupno;
                        data.status = stimulacijeEdit.status;

                    }
                    context.SaveChanges();

                    TempData["uspjeh"] = "Uspješno ste ažurirali operatora " + data.@operator;
                }

                return RedirectToAction("StimulacijeZaObradu");
            }

            return View(stimulacijeEdit);
           
        }


        public ActionResult Logout()
        {
            Session["User"] = "";
            Session["Password"] = "";

            return RedirectToAction("Login");
        }



        

    }
}