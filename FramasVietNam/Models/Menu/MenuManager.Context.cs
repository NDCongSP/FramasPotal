﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MenuManagerEntities : DbContext
    {
        public MenuManagerEntities()
            : base("name=MenuManagerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DeptToModule> DeptToModules { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<EventList> EventLists { get; set; }
        public virtual DbSet<FSComputer> FSComputers { get; set; }
        public virtual DbSet<Function> Functions { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<GroupToFunction> GroupToFunctions { get; set; }
        public virtual DbSet<ListWorkLoad> ListWorkLoads { get; set; }
        public virtual DbSet<MailSupport> MailSupports { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ModuleToGroup> ModuleToGroups { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<SaturdayOffPlanning> SaturdayOffPlannings { get; set; }
        public virtual DbSet<Slider> Sliders { get; set; }
        public virtual DbSet<sys_Language> sys_Language { get; set; }
        public virtual DbSet<sys_Translation> sys_Translation { get; set; }
        public virtual DbSet<UserToDept> UserToDepts { get; set; }
    
        [DbFunction("MenuManagerEntities", "SplitStringToTable")]
        public virtual IQueryable<SplitStringToTable_Result> SplitStringToTable(string @string, string delimiter)
        {
            var stringParameter = @string != null ?
                new ObjectParameter("string", @string) :
                new ObjectParameter("string", typeof(string));
    
            var delimiterParameter = delimiter != null ?
                new ObjectParameter("delimiter", delimiter) :
                new ObjectParameter("delimiter", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<SplitStringToTable_Result>("[MenuManagerEntities].[SplitStringToTable](@string, @delimiter)", stringParameter, delimiterParameter);
        }
    
        public virtual int SearchAllTables(string searchStr)
        {
            var searchStrParameter = searchStr != null ?
                new ObjectParameter("SearchStr", searchStr) :
                new ObjectParameter("SearchStr", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SearchAllTables", searchStrParameter);
        }
    
        public virtual int sp_ActionLog(string actionName, string controllerName, string method, string content, string detail, string userName, Nullable<System.DateTime> date)
        {
            var actionNameParameter = actionName != null ?
                new ObjectParameter("ActionName", actionName) :
                new ObjectParameter("ActionName", typeof(string));
    
            var controllerNameParameter = controllerName != null ?
                new ObjectParameter("ControllerName", controllerName) :
                new ObjectParameter("ControllerName", typeof(string));
    
            var methodParameter = method != null ?
                new ObjectParameter("Method", method) :
                new ObjectParameter("Method", typeof(string));
    
            var contentParameter = content != null ?
                new ObjectParameter("Content", content) :
                new ObjectParameter("Content", typeof(string));
    
            var detailParameter = detail != null ?
                new ObjectParameter("Detail", detail) :
                new ObjectParameter("Detail", typeof(string));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            var dateParameter = date.HasValue ?
                new ObjectParameter("Date", date) :
                new ObjectParameter("Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ActionLog", actionNameParameter, controllerNameParameter, methodParameter, contentParameter, detailParameter, userNameParameter, dateParameter);
        }
    
        public virtual ObjectResult<sp_BirthdayEmployeeGet_Result> sp_BirthdayEmployeeGet(string fromDate, string toDate)
        {
            var fromDateParameter = fromDate != null ?
                new ObjectParameter("FromDate", fromDate) :
                new ObjectParameter("FromDate", typeof(string));
    
            var toDateParameter = toDate != null ?
                new ObjectParameter("ToDate", toDate) :
                new ObjectParameter("ToDate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_BirthdayEmployeeGet_Result>("sp_BirthdayEmployeeGet", fromDateParameter, toDateParameter);
        }
    
        public virtual ObjectResult<sp_BookingGet_Result> sp_BookingGet()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_BookingGet_Result>("sp_BookingGet");
        }
    
        public virtual ObjectResult<sp_CheckPermissionUser_Result> sp_CheckPermissionUser(string user, string action)
        {
            var userParameter = user != null ?
                new ObjectParameter("User", user) :
                new ObjectParameter("User", typeof(string));
    
            var actionParameter = action != null ?
                new ObjectParameter("Action", action) :
                new ObjectParameter("Action", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_CheckPermissionUser_Result>("sp_CheckPermissionUser", userParameter, actionParameter);
        }
    
        public virtual ObjectResult<sp_EventList_Get_Result> sp_EventList_Get(Nullable<int> language)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_EventList_Get_Result>("sp_EventList_Get", languageParameter);
        }
    
        public virtual ObjectResult<sp_Function_Get_Result> sp_Function_Get(Nullable<int> language, Nullable<int> groupID)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            var groupIDParameter = groupID.HasValue ?
                new ObjectParameter("GroupID", groupID) :
                new ObjectParameter("GroupID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_Function_Get_Result>("sp_Function_Get", languageParameter, groupIDParameter);
        }
    
        public virtual ObjectResult<sp_Group_Get_Result> sp_Group_Get(Nullable<int> language)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_Group_Get_Result>("sp_Group_Get", languageParameter);
        }
    
        public virtual ObjectResult<sp_MesListWorkLoad_Result> sp_MesListWorkLoad(Nullable<System.DateTime> dt, string dept)
        {
            var dtParameter = dt.HasValue ?
                new ObjectParameter("dt", dt) :
                new ObjectParameter("dt", typeof(System.DateTime));
    
            var deptParameter = dept != null ?
                new ObjectParameter("Dept", dept) :
                new ObjectParameter("Dept", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_MesListWorkLoad_Result>("sp_MesListWorkLoad", dtParameter, deptParameter);
        }
    
        public virtual ObjectResult<sp_Module_Get_Result> sp_Module_Get(Nullable<int> language, string userName)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            var userNameParameter = userName != null ?
                new ObjectParameter("UserName", userName) :
                new ObjectParameter("UserName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_Module_Get_Result>("sp_Module_Get", languageParameter, userNameParameter);
        }
    
        public virtual ObjectResult<sp_ModuleGroupGet_Result> sp_ModuleGroupGet(Nullable<int> language, Nullable<int> moduleID, Nullable<int> groupID)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            var moduleIDParameter = moduleID.HasValue ?
                new ObjectParameter("ModuleID", moduleID) :
                new ObjectParameter("ModuleID", typeof(int));
    
            var groupIDParameter = groupID.HasValue ?
                new ObjectParameter("GroupID", groupID) :
                new ObjectParameter("GroupID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ModuleGroupGet_Result>("sp_ModuleGroupGet", languageParameter, moduleIDParameter, groupIDParameter);
        }
    
        public virtual ObjectResult<sp_News_Get_Result> sp_News_Get(Nullable<int> language)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_News_Get_Result>("sp_News_Get", languageParameter);
        }
    
        public virtual ObjectResult<sp_NewsDetail_Get_Result> sp_NewsDetail_Get(Nullable<int> language)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_NewsDetail_Get_Result>("sp_NewsDetail_Get", languageParameter);
        }
    
        public virtual ObjectResult<sp_rpt_Function_Result> sp_rpt_Function()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_rpt_Function_Result>("sp_rpt_Function");
        }
    
        public virtual ObjectResult<sp_SaturdayOffPlanning_Get_Result> sp_SaturdayOffPlanning_Get(Nullable<System.DateTime> dt, string dept)
        {
            var dtParameter = dt.HasValue ?
                new ObjectParameter("dt", dt) :
                new ObjectParameter("dt", typeof(System.DateTime));
    
            var deptParameter = dept != null ?
                new ObjectParameter("Dept", dept) :
                new ObjectParameter("Dept", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SaturdayOffPlanning_Get_Result>("sp_SaturdayOffPlanning_Get", dtParameter, deptParameter);
        }
    
        public virtual ObjectResult<sp_ShortNews_Get_Result> sp_ShortNews_Get(Nullable<int> language)
        {
            var languageParameter = language.HasValue ?
                new ObjectParameter("language", language) :
                new ObjectParameter("language", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_ShortNews_Get_Result>("sp_ShortNews_Get", languageParameter);
        }
    
        public virtual ObjectResult<string> sp_SliderImage()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("sp_SliderImage");
        }
    
        public virtual int sp_SliderImageDel(string image)
        {
            var imageParameter = image != null ?
                new ObjectParameter("image", image) :
                new ObjectParameter("image", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_SliderImageDel", imageParameter);
        }
    
        public virtual ObjectResult<sp_SalaryGet_Result> sp_SalaryGet()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_SalaryGet_Result>("sp_SalaryGet");
        }
    }
}