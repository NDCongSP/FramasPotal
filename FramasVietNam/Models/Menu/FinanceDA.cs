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
    
    public partial class FinanceDA
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public string Brand { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal AmoQty { get; set; }
        public Nullable<bool> Status { get; set; }
        public string UserAdd { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
        public string UserUpd { get; set; }
        public Nullable<System.DateTime> DateUpd { get; set; }
    }
}
