﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbNautilusEntities4 : DbContext
    {
        public dbNautilusEntities4()
            : base("name=dbNautilusEntities4")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<StimulacijaLOGIN> StimulacijaLOGIN { get; set; }
        public DbSet<StimulacijaPL> StimulacijaPL { get; set; }
    }
}