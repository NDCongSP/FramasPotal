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
    
    public partial class tblProject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblProject()
        {
            this.tblOEEMS_Header = new HashSet<tblOEEMS_Header>();
            this.tblProjectComps = new HashSet<tblProjectComp>();
            this.tblProjectSizes = new HashSet<tblProjectSize>();
        }
    
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual tblCustomer tblCustomer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblOEEMS_Header> tblOEEMS_Header { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProjectComp> tblProjectComps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProjectSize> tblProjectSizes { get; set; }
        public virtual tblCategory tblCategory { get; set; }
    }
}
