//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class WorkDay
    {
        public WorkDay()
        {
            this.WorkAreas = new HashSet<WorkArea>();
            this.WorkEmployers = new HashSet<WorkEmployer>();
            this.WorkShiftAccessEmployers = new HashSet<WorkShiftAccessEmployer>();
        }
    
        public int workDayId { get; set; }
        public int startTime { get; set; }
        public int endTime { get; set; }
        public int workDayState { get; set; }
    
        public virtual ICollection<WorkArea> WorkAreas { get; set; }
        public virtual ICollection<WorkEmployer> WorkEmployers { get; set; }
        public virtual ICollection<WorkShiftAccessEmployer> WorkShiftAccessEmployers { get; set; }
    }
}
