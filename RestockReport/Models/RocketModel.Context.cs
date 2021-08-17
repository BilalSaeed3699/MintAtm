﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RestockReport.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    using System.Linq;
    
    public partial class RestockReportsEntities : DbContext
    {
        public RestockReportsEntities()
            : base("name=RestockReportsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<tblMenu> tblMenus { get; set; }
        public DbSet<tblSetting> tblSettings { get; set; }
        public DbSet<tblTerminal> tblTerminals { get; set; }
        public DbSet<tblTerminalSurchargeAccount> tblTerminalSurchargeAccounts { get; set; }
        public DbSet<tblRegion> tblRegions { get; set; }
        public DbSet<tblUserterminal> tblUserterminals { get; set; }
        public DbSet<tblUser_Menu_Right> tblUser_Menu_Right { get; set; }
        public DbSet<tblReportData> tblReportDatas { get; set; }
        public DbSet<tblTitle> tblTitles { get; set; }
        public DbSet<tblActivationCode> tblActivationCodes { get; set; }
        public DbSet<GraphDataFilter> GraphDataFilters { get; set; }
        public DbSet<tblWebmoonUser> tblWebmoonUsers { get; set; }
        public DbSet<tblAccess_Level> tblAccess_Level { get; set; }
        public DbSet<tblAccess_Level_Menu> tblAccess_Level_Menu { get; set; }
        public DbSet<tblterminalsegmented> tblterminalsegmenteds { get; set; }
        public DbSet<tblMonth> tblMonths { get; set; }
        public DbSet<tblYear> tblYears { get; set; }
        public DbSet<tblCommunicationType> tblCommunicationTypes { get; set; }
        public DbSet<tblProgramType> tblProgramTypes { get; set; }
        public DbSet<tblCompany> tblCompanies { get; set; }
        public DbSet<tblAccountStatu> tblAccountStatus { get; set; }
        public DbSet<tblAccountType> tblAccountTypes { get; set; }
        public DbSet<tblBankAccountDetail> tblBankAccountDetails { get; set; }
        public DbSet<tblStackHolder> tblStackHolders { get; set; }
        public DbSet<tblTerminalManager> tblTerminalManagers { get; set; }
        public DbSet<tblTermsDetail> tblTermsDetails { get; set; }
        public DbSet<tblUser> tblUsers { get; set; }
        public DbSet<tblNewletter> tblNewletters { get; set; }
        public DbSet<tblBankAccountList> tblBankAccountLists { get; set; }
        public DbSet<tblReportJobLog> tblReportJobLogs { get; set; }
    
        public virtual ObjectResult<Sp_Get_User_Assigned_Terminal_List_Result> Sp_Get_User_Assigned_Terminal_List(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Assigned_Terminal_List_Result>("Sp_Get_User_Assigned_Terminal_List", userIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Segmented_Terminal_List_Result> Sp_Get_User_Segmented_Terminal_List(Nullable<int> userid)
        {
            var useridParameter = userid.HasValue ?
                new ObjectParameter("Userid", userid) :
                new ObjectParameter("Userid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Segmented_Terminal_List_Result>("Sp_Get_User_Segmented_Terminal_List", useridParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Graph_Data_Result> Sp_Get_User_Graph_Data(string userid, string terminalKey, Nullable<int> monthId, Nullable<int> yearId)
        {
            var useridParameter = userid != null ?
                new ObjectParameter("Userid", userid) :
                new ObjectParameter("Userid", typeof(string));
    
            var terminalKeyParameter = terminalKey != null ?
                new ObjectParameter("TerminalKey", terminalKey) :
                new ObjectParameter("TerminalKey", typeof(string));
    
            var monthIdParameter = monthId.HasValue ?
                new ObjectParameter("MonthId", monthId) :
                new ObjectParameter("MonthId", typeof(int));
    
            var yearIdParameter = yearId.HasValue ?
                new ObjectParameter("YearId", yearId) :
                new ObjectParameter("YearId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Graph_Data_Result>("Sp_Get_User_Graph_Data", useridParameter, terminalKeyParameter, monthIdParameter, yearIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Dashboard_Menu_List_Result> Sp_Get_User_Dashboard_Menu_List(Nullable<int> accessId)
        {
            var accessIdParameter = accessId.HasValue ?
                new ObjectParameter("AccessId", accessId) :
                new ObjectParameter("AccessId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Dashboard_Menu_List_Result>("Sp_Get_User_Dashboard_Menu_List", accessIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Menu_List_Result> Sp_Get_User_Menu_List(Nullable<int> accessId)
        {
            var accessIdParameter = accessId.HasValue ?
                new ObjectParameter("AccessId", accessId) :
                new ObjectParameter("AccessId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Menu_List_Result>("Sp_Get_User_Menu_List", accessIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_Report_Data_List_Result> Sp_Get_Report_Data_List(Nullable<System.DateTime> reportDate, string terminalKey, Nullable<int> userId)
        {
            var reportDateParameter = reportDate.HasValue ?
                new ObjectParameter("ReportDate", reportDate) :
                new ObjectParameter("ReportDate", typeof(System.DateTime));
    
            var terminalKeyParameter = terminalKey != null ?
                new ObjectParameter("TerminalKey", terminalKey) :
                new ObjectParameter("TerminalKey", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_Report_Data_List_Result>("Sp_Get_Report_Data_List", reportDateParameter, terminalKeyParameter, userIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_Bank_Detail_List_Result> Sp_Get_Bank_Detail_List()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_Bank_Detail_List_Result>("Sp_Get_Bank_Detail_List");
        }
    
        public virtual ObjectResult<Sp_Get_BankNickName_List_Result> Sp_Get_BankNickName_List()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_BankNickName_List_Result>("Sp_Get_BankNickName_List");
        }
    
        public virtual ObjectResult<Sp_Get_TermsDetails_BankNickName_List_Result> Sp_Get_TermsDetails_BankNickName_List(string type)
        {
            var typeParameter = type != null ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_TermsDetails_BankNickName_List_Result>("Sp_Get_TermsDetails_BankNickName_List", typeParameter);
        }
    
        public virtual ObjectResult<Sp_Term_Manager_List_Result> Sp_Term_Manager_List()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Term_Manager_List_Result>("Sp_Term_Manager_List");
        }
    
        public virtual ObjectResult<Sp_Get_Bank_Account_UsageDetail_Result> Sp_Get_Bank_Account_UsageDetail(Nullable<int> bankVaultCashId)
        {
            var bankVaultCashIdParameter = bankVaultCashId.HasValue ?
                new ObjectParameter("BankVaultCashId", bankVaultCashId) :
                new ObjectParameter("BankVaultCashId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_Bank_Account_UsageDetail_Result>("Sp_Get_Bank_Account_UsageDetail", bankVaultCashIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_Daily_Deposit_Report_Data_List_Result> Sp_Get_Daily_Deposit_Report_Data_List(Nullable<System.DateTime> reportDate, string terminalKey, Nullable<int> userId)
        {
            var reportDateParameter = reportDate.HasValue ?
                new ObjectParameter("ReportDate", reportDate) :
                new ObjectParameter("ReportDate", typeof(System.DateTime));
    
            var terminalKeyParameter = terminalKey != null ?
                new ObjectParameter("TerminalKey", terminalKey) :
                new ObjectParameter("TerminalKey", typeof(string));
    
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_Daily_Deposit_Report_Data_List_Result>("Sp_Get_Daily_Deposit_Report_Data_List", reportDateParameter, terminalKeyParameter, userIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Terminal_List_Result> Sp_Get_User_Terminal_List(Nullable<int> userid)
        {
            var useridParameter = userid.HasValue ?
                new ObjectParameter("Userid", userid) :
                new ObjectParameter("Userid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Terminal_List_Result>("Sp_Get_User_Terminal_List", useridParameter);
        }
    
        public virtual int Sp_Get_Daily_Transaction_Graph(Nullable<System.DateTime> sTARTDATE, Nullable<System.DateTime> eNDDATE)
        {
            var sTARTDATEParameter = sTARTDATE.HasValue ?
                new ObjectParameter("STARTDATE", sTARTDATE) :
                new ObjectParameter("STARTDATE", typeof(System.DateTime));
    
            var eNDDATEParameter = eNDDATE.HasValue ?
                new ObjectParameter("ENDDATE", eNDDATE) :
                new ObjectParameter("ENDDATE", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Sp_Get_Daily_Transaction_Graph", sTARTDATEParameter, eNDDATEParameter);
        }
    
        public virtual ObjectResult<Sp_Get_Daily_Transaction_Result> Sp_Get_Daily_Transaction(Nullable<System.DateTime> sTARTDATE, Nullable<System.DateTime> eNDDATE, string terminalId)
        {
            var sTARTDATEParameter = sTARTDATE.HasValue ?
                new ObjectParameter("STARTDATE", sTARTDATE) :
                new ObjectParameter("STARTDATE", typeof(System.DateTime));
    
            var eNDDATEParameter = eNDDATE.HasValue ?
                new ObjectParameter("ENDDATE", eNDDATE) :
                new ObjectParameter("ENDDATE", typeof(System.DateTime));
    
            var terminalIdParameter = terminalId != null ?
                new ObjectParameter("TerminalId", terminalId) :
                new ObjectParameter("TerminalId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_Daily_Transaction_Result>("Sp_Get_Daily_Transaction", sTARTDATEParameter, eNDDATEParameter, terminalIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_Donut_Revenue_Graph_Result> Sp_Get_Donut_Revenue_Graph(string userid, string terminalKey)
        {
            var useridParameter = userid != null ?
                new ObjectParameter("Userid", userid) :
                new ObjectParameter("Userid", typeof(string));
    
            var terminalKeyParameter = terminalKey != null ?
                new ObjectParameter("TerminalKey", terminalKey) :
                new ObjectParameter("TerminalKey", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_Donut_Revenue_Graph_Result>("Sp_Get_Donut_Revenue_Graph", useridParameter, terminalKeyParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> Sp_Get_Gross_Commision(string terminalKey)
        {
            var terminalKeyParameter = terminalKey != null ?
                new ObjectParameter("TerminalKey", terminalKey) :
                new ObjectParameter("TerminalKey", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("Sp_Get_Gross_Commision", terminalKeyParameter);
        }
    
        public virtual ObjectResult<Sp_Get_AccountHolder_List_Result> Sp_Get_AccountHolder_List(Nullable<int> type, string accountHolderName)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            var accountHolderNameParameter = accountHolderName != null ?
                new ObjectParameter("AccountHolderName", accountHolderName) :
                new ObjectParameter("AccountHolderName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_AccountHolder_List_Result>("Sp_Get_AccountHolder_List", typeParameter, accountHolderNameParameter);
        }
    
        public virtual ObjectResult<Sp_Get_AccountHolder_Details_Result> Sp_Get_AccountHolder_Details(Nullable<int> type, string accountHolderName)
        {
            var typeParameter = type.HasValue ?
                new ObjectParameter("Type", type) :
                new ObjectParameter("Type", typeof(int));
    
            var accountHolderNameParameter = accountHolderName != null ?
                new ObjectParameter("AccountHolderName", accountHolderName) :
                new ObjectParameter("AccountHolderName", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_AccountHolder_Details_Result>("Sp_Get_AccountHolder_Details", typeParameter, accountHolderNameParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Bank_List_Result> Sp_Get_User_Bank_List(Nullable<int> userid)
        {
            var useridParameter = userid.HasValue ?
                new ObjectParameter("Userid", userid) :
                new ObjectParameter("Userid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Bank_List_Result>("Sp_Get_User_Bank_List", useridParameter);
        }
    
        public virtual ObjectResult<Sp_Load_Bank_Atm_Result> Sp_Load_Bank_Atm(Nullable<int> userId, Nullable<int> bankId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(int));
    
            var bankIdParameter = bankId.HasValue ?
                new ObjectParameter("BankId", bankId) :
                new ObjectParameter("BankId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Load_Bank_Atm_Result>("Sp_Load_Bank_Atm", userIdParameter, bankIdParameter);
        }
    
        public virtual ObjectResult<Sp_Get_User_Graph_Filter_Data__Result> Sp_Get_User_Graph_Filter_Data_(string cWhere)
        {
            var cWhereParameter = cWhere != null ?
                new ObjectParameter("cWhere", cWhere) :
                new ObjectParameter("cWhere", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Sp_Get_User_Graph_Filter_Data__Result>("Sp_Get_User_Graph_Filter_Data_", cWhereParameter);
        }
    }
}
