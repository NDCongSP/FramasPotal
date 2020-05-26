using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FramasVietNam.Models.FingerScan
{
    public class DeliveryShow
    {
        public long ID { get; set; }
        public int MachineNumber { get; set; }
        public int IndRegID { get; set; }
        public string TenNhanVien { get; set; }
        public int MenuMealID { get; set; }
        public string MenuMealName { get; set; }
        public int ShiftWorkID { get; set; }
        public string ShiftWorkName { get; set; }
        public Boolean IsTakeMeal { get; set; }
        public Boolean isCheck { get; set; }
        public string MsgCheck { get; set; }
        public string DateTimeFinger { get; set; }
    }
}