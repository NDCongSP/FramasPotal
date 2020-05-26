using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FramasVietNam.Models.OEEMS
{
    public class tblWorkPieceModel
    {
        public long ID { get; set; }
        public int WorkpieceID { get; set; }
        public int ProcessID { get; set; }
        public string Description { get; set; }
        public string WorkpieceName { get; set; }
        public string ProcessName { get; set; }
        public int hour { get; set; }
        public int munite { get; set; }
        public int plantime { get; set; }
        public int Realhour { get; set; }
        public int Realmunite { get; set; }
        public int Realtime { get; set; }
    }
    public class tblWorkPieceList
    {
        //use CheckBoxModel class as list 
        public List<tblWorkPieceModel> tblWorkPiece { get; set; }
    }
    public class tblOEEMS_PercentCompleteList
    {
        //use CheckBoxModel class as list 
        public List<tblOEEMS_PercentComplete> tblOEEMS_PercentComplete { get; set; }
    }
}