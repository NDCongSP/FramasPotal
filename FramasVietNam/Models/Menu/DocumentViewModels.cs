using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FramasVietNam.Models.Menu
{
    public class DocumentViewModels
    {
        public int ID { get; set; }
        public string DocName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string FileUrl { get; set; }
        public string CreateBy { get; set; }

        public bool IsActive { get; set; }
    }
}