//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stimulacije_2022
{
    using System;
    using System.Collections.Generic;
    
    public partial class StimulacijaPL
    {
        public int idStimulacije { get; set; }
        public Nullable<int> mjesec { get; set; }
        public Nullable<int> godina { get; set; }
        public Nullable<int> @operator { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public Nullable<double> Efikasnost { get; set; }
        public Nullable<double> EfPremaNormi { get; set; }
        public Nullable<double> Norma { get; set; }
        public Nullable<int> PostotakDestim { get; set; }
        public Nullable<int> Stimulacija { get; set; }
        public Nullable<int> Ukupno { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> DatumPrebacivanja { get; set; }
    }
}
