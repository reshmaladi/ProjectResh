//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HackathonPMA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Fund
    {
        public Fund()
        {
            this.FundProjects = new HashSet<FundProject>();
        }
    
        public int Id { get; set; }
        public string TotalAmount { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string SpentAmount { get; set; }
    
        public virtual ICollection<FundProject> FundProjects { get; set; }
    }
}
