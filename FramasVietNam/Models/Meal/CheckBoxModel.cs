using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FramasVietNam.Models.Meal
{
    public class CheckBoxModel
    {
        //Value of checkbox 
        public int Value { get; set; }
        //description of checkbox 
        public string Code { get; set; }
        public string Name { get; set; }
        //whether the checkbox is selected or not
        public bool IsChecked { get; set; }
    }
    public class CheckBoxList
    {
        //use CheckBoxModel class as list 
        public List<CheckBoxModel> CheckBox { get; set; }
    }
    public class Avatar
    {
        public string input_Avatar { get; set; }
    }
}