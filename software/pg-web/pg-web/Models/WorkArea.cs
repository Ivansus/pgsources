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
    
    public partial class WorkArea
    {
        public WorkArea()
        {
            this.WorkShifts = new HashSet<WorkShift>();
        }
    
        public int workAreaId { get; set; }
        public int workDayId { get; set; }
        public string workAreaName { get; set; }
        public int areaId { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual WorkDay WorkDay { get; set; }
        public virtual ICollection<WorkShift> WorkShifts { get; set; }
    }
}