using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Stimulacije_2022.Models
{

    public class Stimulacije
    {
        string connStringNautilus = ConfigurationManager.ConnectionStrings["Nautilus"].ConnectionString;
        string connStringZuccheti = ConfigurationManager.ConnectionStrings["Zucchetti"].ConnectionString;
        SqlConnection conn = new SqlConnection();
        List<Zdruzeno> sviPodaci = new List<Zdruzeno>();
        List<Bolovanja> podaciBolovanja = new List<Bolovanja>();

        public List<Zdruzeno> vratiPodatke()
        {

            //DataTable za stimulacije (ovdje su sve radnice)
            DataTable dtTublaEffFinal = new DataTable();
            using (conn = new SqlConnection(connStringNautilus))
            {

                var today = DateTime.Today;
                var month = new DateTime(today.Year, today.Month, 1);
                var first = month.AddMonths(-1).ToString("yyyy-MM-dd hh:mm:ss");
                var last = month.AddDays(-1).ToString("yyyy-MM-dd hh:mm:ss");

                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * " +
                                                "FROM TublaEffFinal({ ts '" + first + "' }, { ts '" + last + "'}) where Operator1 not in (1,2)", conn);
                cmd.CommandTimeout = 120;
                SqlDataAdapter adapter1 = new SqlDataAdapter(cmd);
                adapter1.Fill(dtTublaEffFinal);
            }


            //DaTaTable za bolovanja (ovdje nisu sve radnice, samo one koje su koristile bolovanje)
            DataTable dtZucchetti = new DataTable();
            using (conn = new SqlConnection(connStringZuccheti))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select * from OduzimanjeBolovanja", conn);
                SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
                adapter2.Fill(dtZucchetti);
            }


            foreach (DataRow pod in dtTublaEffFinal.Rows)
            {

                Zdruzeno podatak = new Zdruzeno();
                podatak.Operator1 = Convert.ToInt32(pod["Operator1"]);
                podatak.FirstName = pod["FirstName"].ToString();
                podatak.LastName = pod["LastName"].ToString();
                podatak.Norma = Convert.ToSingle(pod["Norma"]);
                podatak.Efikasnost = Convert.ToSingle(pod["Efikasnost"]);
                podatak.EfPremaNormi = Convert.ToSingle(pod["Stimulacija"]);

                if (podatak.EfPremaNormi >= 0 && podatak.EfPremaNormi <= 96.99)
                {
                    podatak.Stimulacija = 48.0;
                }
                else if (podatak.EfPremaNormi >= 97 && podatak.EfPremaNormi <= 98.99)
                {
                    podatak.Stimulacija = 215.5;
                }
                else if (podatak.EfPremaNormi >= 99 && podatak.EfPremaNormi <= 100.99)
                {
                    podatak.Stimulacija = 246;
                }
                else if (podatak.EfPremaNormi >= 101 && podatak.EfPremaNormi <= 103.99)
                {
                    podatak.Stimulacija = 295;
                }
                else
                {
                    podatak.Stimulacija = 356;
                }

                podatak.PostoDestim = (from DataRow r in dtZucchetti.Rows
                                       where r.Field<int>("BADGE") == Convert.ToInt32(pod["Operator1"])
                                       select r.Field<int>("PostoDestim")).FirstOrDefault();


                podatak.Ukupno = podatak.Stimulacija - podatak.PostoDestim;


                sviPodaci.Add(podatak);
            }

            //kopiranje           

            foreach (var pod in sviPodaci)
            {
                DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);

                int mjesecPrije = oneMonthAgo.Month;
                int godinaPrije = oneMonthAgo.Year;
                int ukupno = sviPodaci.Count();


                using (var context = new dbNautilusEntities4())
                {
                    var postoji = context.StimulacijaPL.Any(u => (u.godina == godinaPrije) && (u.mjesec == mjesecPrije)
                        && (u.@operator == pod.Operator1));

                    if (context.StimulacijaPL.Count() == 0 || !postoji)
                    {
                        var PodatakZaCopy = new StimulacijaPL
                        {
                            mjesec = mjesecPrije,
                            godina = godinaPrije,
                            @operator = pod.Operator1,
                            ime = pod.FirstName,
                            prezime = pod.LastName,
                            Efikasnost = Math.Round(pod.Efikasnost, 2),
                            EfPremaNormi = Math.Round(pod.EfPremaNormi, 2),
                            Norma = Math.Round(pod.Norma, 2),
                            PostotakDestim = Convert.ToInt32(pod.PostoDestim),
                            Stimulacija = Convert.ToInt32(pod.Stimulacija),
                            Ukupno = Convert.ToInt32(pod.Ukupno),
                            status = "1"
                        };

                        context.StimulacijaPL.Add(PodatakZaCopy);
                        context.SaveChanges();
                    }
                    else
                    {
                        var podatakZaUpdate = context.StimulacijaPL.First(c => c.@operator == pod.Operator1);
                        podatakZaUpdate.Efikasnost = Math.Round(pod.Efikasnost, 2);
                        podatakZaUpdate.EfPremaNormi = Math.Round(pod.EfPremaNormi, 2);
                        podatakZaUpdate.Norma = Math.Round(pod.Norma, 2);
                        podatakZaUpdate.PostotakDestim = Convert.ToInt32(pod.PostoDestim);
                        podatakZaUpdate.Stimulacija = Convert.ToInt32(pod.Stimulacija);
                        podatakZaUpdate.Ukupno = Convert.ToInt32(pod.Ukupno);
                        context.SaveChanges();
                    }

                }
            }
            //kopiranje


            return sviPodaci;

        }



        public List<Bolovanja> vratiListuBolovanja(string operatori)
        {
            string[] listaOperatora = operatori.Split(',');
            DataTable dtZucchetti = new DataTable();
            DataTable dtNautilus = new DataTable();

            foreach (var op in listaOperatora)
            {
                
                using (conn = new SqlConnection(connStringZuccheti))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from OduzimanjeBolovanja where BADGE IN ('" + op + "')", conn);
                    SqlDataAdapter adapter2 = new SqlDataAdapter(cmd);
                    adapter2.Fill(dtZucchetti);
                }

                                
                using (conn = new SqlConnection(connStringNautilus))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("select * from Operators Where OpCode IN ('" + op + "')", conn);
                    SqlDataAdapter adapterOp = new SqlDataAdapter(cmd);
                    adapterOp.Fill(dtNautilus);
                }

            }            


            foreach (DataRow zuc in dtZucchetti.Rows)
            {
                foreach (DataRow naut in dtNautilus.Rows)
                {
                    if (Convert.ToInt32(zuc["BADGE"]) == Convert.ToInt32(naut["OpCode"]))
                    {
                        Bolovanja bolovanja = new Bolovanja();
                        bolovanja.BADGE = Convert.ToInt32(zuc["BADGE"]);
                        bolovanja.PostoDestim = Convert.ToInt32(zuc["PostoDestim"]);
                        bolovanja.ime = naut["FirstName"].ToString();
                        bolovanja.prezime = naut["LastName"].ToString();
                        podaciBolovanja.Add(bolovanja);
                    }                   
                    
                    
                }
               
            }

            return podaciBolovanja;
        }


        public int VratiDestimulaciju(int Badge)
        {
            // DataTable dtZucchetti = new DataTable();
            int destimulacija = 0; ;
            using (conn = new SqlConnection(connStringZuccheti))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select PostoDestim from OduzimanjeBolovanja where BADGE IN ('" + Badge + "')", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    destimulacija = Convert.ToInt32(reader["PostoDestim"]);
                }
                else
                {
                    destimulacija = 0;
                }


                return destimulacija;
            }

            

        }
        
    } 

}