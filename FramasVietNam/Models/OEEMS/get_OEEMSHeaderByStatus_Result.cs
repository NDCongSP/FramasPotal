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
    
    public partial class get_OEEMSHeaderByStatus_Result
    {
        public string OEEID { get; set; }
        public Nullable<System.DateTime> OEEDate { get; set; }
        public string ProjectID { get; set; }
        public Nullable<int> CompID { get; set; }
        public Nullable<int> SizeID { get; set; }
        public Nullable<decimal> Column1 { get; set; }
        public Nullable<decimal> RealTime { get; set; }
        public Nullable<decimal> DownTime { get; set; }
        public string OEE { get; set; }
        public Nullable<int> Status { get; set; }
    }
}
