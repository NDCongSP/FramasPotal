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
    
    public partial class News
    {
        public int ID { get; set; }
        public string Title_EG { get; set; }
        public string Title_GR { get; set; }
        public string Title_VN { get; set; }
        public string ShortContent_EG { get; set; }
        public string ShortContent_GR { get; set; }
        public string ShortContent_VN { get; set; }
        public string Content_EG { get; set; }
        public string Content_GR { get; set; }
        public string Content_VN { get; set; }
        public Nullable<int> NewsType { get; set; }
        public string Images { get; set; }
        public string TableName { get; set; }
        public Nullable<int> Status { get; set; }
        public string UserAdd { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
        public string UserUpd { get; set; }
        public Nullable<System.DateTime> DateUpd { get; set; }
    }
}