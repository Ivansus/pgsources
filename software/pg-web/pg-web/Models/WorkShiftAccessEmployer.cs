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
    
    public partial class WorkShiftAccessEmployer
    {
        public int workDayId { get; set; }
        public int workShiftId { get; set; }
        public int workEmployersId { get; set; }
    
        public virtual WorkDay WorkDay { get; set; }
        public virtual WorkEmployer WorkEmployer { get; set; }
        public virtual WorkShift WorkShift { get; set; }
    }
}
