using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FramasVietNam.Models.UserManager
{
    public class UserModuleRoleViewModels
    {
        public string MaSoNhanVien { get; set; }
        public string Email { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string BoPhan { get; set; }
        public int ModuleId { get; set; }
        public string RoleId { get; set; }
    }
}