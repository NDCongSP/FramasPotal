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
    
    public partial class Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Group()
        {
            this.GroupToFunctions = new HashSet<GroupToFunction>();
            this.ModuleToGroups = new HashSet<ModuleToGroup>();
        }
    
        public int ID { get; set; }
        public string Code { get; set; }
        public string TableName { get; set; }
        public string Image { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public Nullable<int> Status { get; set; }
        public string PCIDAdd { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupToFunction> GroupToFunctions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ModuleToGroup> ModuleToGroups { get; set; }
    }
}
