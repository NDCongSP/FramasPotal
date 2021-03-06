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
    
    public partial class tblOEEMS_Header
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblOEEMS_Header()
        {
            this.tblOEEMS_Process = new HashSet<tblOEEMS_Process>();
            this.tblOEEMS_PercentComplete = new HashSet<tblOEEMS_PercentComplete>();
        }
    
        public string OEEID { get; set; }
        public Nullable<System.DateTime> OEEDate { get; set; }
        public string ProjectID { get; set; }
        public Nullable<int> CompID { get; set; }
        public Nullable<int> SizeID { get; set; }
        public Nullable<int> PlanTime { get; set; }
        public Nullable<int> RealTime { get; set; }
        public Nullable<int> DownTime { get; set; }
        public Nullable<double> OEE { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Status { get; set; }
    
        public virtual tblComp tblComp { get; set; }
        public virtual tblOEEMS_Status tblOEEMS_Status { get; set; }
        public virtual tblProject tblProject { get; set; }
        public virtual tblSize tblSize { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOEEMS_Process> tblOEEMS_Process { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOEEMS_PercentComplete> tblOEEMS_PercentComplete { get; set; }
    }
}
