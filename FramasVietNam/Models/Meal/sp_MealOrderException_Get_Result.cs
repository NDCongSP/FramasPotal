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
    
    public partial class sp_MealOrderException_Get_Result
    {
        public int ID { get; set; }
        public string Day { get; set; }
        public int ShiftWorkID { get; set; }
        public string ShiftWorkName { get; set; }
        public int MenuMealID { get; set; }
        public string MenuMealName { get; set; }
        public string DepartmentName { get; set; }
        public string UserNameRequest { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsTakeMeal { get; set; }
    }
}
