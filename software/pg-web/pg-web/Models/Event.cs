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
    
    public partial class Event
    {
        public int eventId { get; set; }
        public int createTime { get; set; }
        public string eventUrl { get; set; }
        public int lifeTime { get; set; }
    }
}