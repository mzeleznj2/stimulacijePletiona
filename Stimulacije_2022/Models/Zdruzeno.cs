using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stimulacije_2022.Models
{
    public class Zdruzeno
    {                
        public int Operator1 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float Efikasnost { get; set; }
        public float EfPremaNormi { get; set; }
        public float Norma { get; set; }
        public double PostoDestim { get; set; }
        public double Stimulacija { get; set; }
        public double Ukupno { get; set; }

    }
}