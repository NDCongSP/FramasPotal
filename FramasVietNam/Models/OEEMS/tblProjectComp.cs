//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FramasVietNam.Models.OEEMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblProjectComp
    {
        public string ProjectID { get; set; }
        public int CompID { get; set; }
        public string Description { get; set; }
    
        public virtual tblComp tblComp { get; set; }
        public virtual tblProject tblProject { get; set; }
    }
}
