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
    
    public partial class get_OEEMSHeaderByDistanceDate_Result
    {
        public Nullable<long> STT { get; set; }
        public string OEEID { get; set; }
        public Nullable<System.DateTime> OEEDate { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> CompID { get; set; }
        public Nullable<int> SizeID { get; set; }
        public string CompName { get; set; }
        public string SizeName { get; set; }
        public Nullable<decimal> PlanTime { get; set; }
        public Nullable<decimal> RealTime { get; set; }
        public Nullable<decimal> DownTime { get; set; }
        public string OEE { get; set; }
        public Nullable<int> Status { get; set; }
    }
}
