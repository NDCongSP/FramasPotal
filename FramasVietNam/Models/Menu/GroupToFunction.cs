//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FramasVietNam.Models.Menu
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupToFunction
    {
        public int GroupID { get; set; }
        public int FunctionalID { get; set; }
        public string TableName { get; set; }
        public Nullable<int> Status { get; set; }
        public string PCIDAdd { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
    
        public virtual Function Function { get; set; }
        public virtual Group Group { get; set; }
    }
}
