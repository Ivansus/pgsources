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
    
    public partial class Label
    {
        public int labelId { get; set; }
        public int labelData { get; set; }
        public int areaId { get; set; }
    
        public virtual Area Area { get; set; }
    }
}
