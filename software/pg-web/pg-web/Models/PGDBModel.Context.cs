﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace pg_web.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class pgworkDBEntities : DbContext
    {
        public pgworkDBEntities()
            : base("name=pgworkDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<WorkArea> WorkAreas { get; set; }
        public virtual DbSet<WorkDay> WorkDays { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Employer> Employers { get; set; }
        public virtual DbSet<Label> Labels { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<WorkEmployer> WorkEmployers { get; set; }
        public virtual DbSet<WorkShift> WorkShifts { get; set; }
        public virtual DbSet<LastAreaAccess> LastAreaAccesses { get; set; }
        public virtual DbSet<WorkShiftAccessEmployer> WorkShiftAccessEmployers { get; set; }
        public virtual DbSet<WorkShiftNotificationEmployer> WorkShiftNotificationEmployers { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<UnknownDevice> UnknownDevices { get; set; }
    }
}
