//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FramasVietNam.Models.Meal
{
    using System;
    using System.Collections.Generic;
    
    public partial class MealOrderException
    {
        public int ID { get; set; }
        public string Day { get; set; }
        public int ShiftWorkID { get; set; }
        public int MenuMealID { get; set; }
        public string DepartmentID { get; set; }
        public string UserNameRequest { get; set; }
        public string FullName { get; set; }
        public Nullable<bool> IsTakeMeal { get; set; }
        public string UserAdd { get; set; }
        public Nullable<System.DateTime> DateAdd { get; set; }
    
        public virtual MenuMeal MenuMeal { get; set; }
        public virtual ShiftWork ShiftWork { get; set; }
    }
}