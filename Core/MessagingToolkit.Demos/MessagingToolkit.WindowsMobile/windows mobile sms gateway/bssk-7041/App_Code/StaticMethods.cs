using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.DLinq;
using System.Query;
using System.Web.Configuration;
using Microsoft.Web.Blinq.Utils;

namespace bulksms {

  public partial class Bulksms : DataContext {
    public static Bulksms CreateDataContext() {
      ConnectionUtil connectionUtil = new ConnectionUtil();
      ConnectionStringSettings connectionStringSettings = WebConfigurationManager.ConnectionStrings["BulksmsConnectionString"];
      return new Bulksms(connectionUtil.CreateConnection(connectionStringSettings));
    }
  }

  public partial class Error {
    // This method retrieves all Errors.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Error> GetAllErrors() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Errors;
    }
    // This method gets record counts of all Errors.
    // Do not change this method.
    public static int GetAllErrorsCount() {
      return GetAllErrors().Count();
    }
    // This method retrieves a single Error.
    // Change this method to alter how that record is received.
    public static Error GetError(Int32 Id) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Errors.Where(x=>x.Id == Id).FirstOrDefault();
    }
    // This method pages and sorts over all Errors.
    // Do not change this method.
    public static IQueryable<Error> GetAllErrors(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllErrors().SortAndPage(sortExpression, startRowIndex, maximumRows, "Id");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(Error x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Errors.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(Error x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Errors.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(Error original_x, Error x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Errors.Attach(original_x);
      original_x.Username = x.Username;
      original_x.Date = x.Date;
      original_x.Num = x.Num;
      original_x.Msg = x.Msg;
      original_x.File = x.File;
      original_x.Line = x.Line;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class Sysdiagram {
    // This method retrieves all Sysdiagrams.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Sysdiagram> GetAllSysdiagrams() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Sysdiagrams;
    }
    // This method gets record counts of all Sysdiagrams.
    // Do not change this method.
    public static int GetAllSysdiagramsCount() {
      return GetAllSysdiagrams().Count();
    }
    // This method retrieves a single Sysdiagram.
    // Change this method to alter how that record is received.
    public static Sysdiagram GetSysdiagram(Int32 DiagramId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Sysdiagrams.Where(x=>x.DiagramId == DiagramId).FirstOrDefault();
    }
    // This method pages and sorts over all Sysdiagrams.
    // Do not change this method.
    public static IQueryable<Sysdiagram> GetAllSysdiagrams(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllSysdiagrams().SortAndPage(sortExpression, startRowIndex, maximumRows, "DiagramId");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(Sysdiagram x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Sysdiagrams.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(Sysdiagram x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Sysdiagrams.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(Sysdiagram original_x, Sysdiagram x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Sysdiagrams.Attach(original_x);
      original_x.Name = x.Name;
      original_x.PrincipalId = x.PrincipalId;
      original_x.Version = x.Version;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class MsgHistory {
    // This method retrieves all MsgHistories.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgHistory> GetAllMsgHistories() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgHistories;
    }
    // This method gets record counts of all MsgHistories.
    // Do not change this method.
    public static int GetAllMsgHistoriesCount() {
      return GetAllMsgHistories().Count();
    }
    // This method retrieves a single MsgHistory.
    // Change this method to alter how that record is received.
    public static MsgHistory GetMsgHistory(Int32 MsgHistoryId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgHistories.Where(x=>x.MsgHistoryId == MsgHistoryId).FirstOrDefault();
    }
    // This method retrieves MsgHistories by MsgMain.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgHistory> GetMsgHistoriesByMsgMain(Int32 Msgid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgMains.Where(x=>x.Msgid == Msgid).SelectMany(x=>x.MsgHistories);
    }
    // This method retrieves MsgHistories by GrpMain.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgHistory> GetMsgHistoriesByGrpMain(Int32 Groupid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpMains.Where(x=>x.Groupid == Groupid).SelectMany(x=>x.MsgHistories);
    }
    // This method retrieves MsgHistories by User.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgHistory> GetMsgHistoriesByUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).SelectMany(x=>x.MsgHistories);
    }
    // This method gets sorted and paged records of all MsgHistories filtered by a specified field.
    // Do not change this method.
    public static IQueryable<MsgHistory> GetMsgHistories(string tableName, Int32 MsgHistories_Msgid, Int32 MsgHistories_Groupid, String MsgHistories_Username, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<MsgHistory> x = GetFilteredMsgHistories(tableName, MsgHistories_Msgid, MsgHistories_Groupid, MsgHistories_Username);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "MsgHistoryId");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<MsgHistory> GetFilteredMsgHistories(string tableName, Int32 MsgHistories_Msgid, Int32 MsgHistories_Groupid, String MsgHistories_Username) {
      switch (tableName) {
        case "MsgMain_MsgHistories":
          return GetMsgHistoriesByMsgMain(MsgHistories_Msgid);
        case "GrpMain_MsgHistories":
          return GetMsgHistoriesByGrpMain(MsgHistories_Groupid);
        case "User_MsgHistories":
          return GetMsgHistoriesByUser(MsgHistories_Username);
        default:
          return GetAllMsgHistories();
      }
    }
    // This method gets records counts of all MsgHistories filtered by a specified field.
    // Do not change this method.
    public static int GetMsgHistoriesCount(string tableName, Int32 MsgHistories_Msgid, Int32 MsgHistories_Groupid, String MsgHistories_Username) {
      IQueryable<MsgHistory> x = GetFilteredMsgHistories(tableName, MsgHistories_Msgid, MsgHistories_Groupid, MsgHistories_Username);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(MsgHistory x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgHistories.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(MsgHistory x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgHistories.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(MsgHistory original_x, MsgHistory x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgHistories.Attach(original_x);
      original_x.Username = x.Username;
      original_x.Msgid = x.Msgid;
      original_x.Groupid = x.Groupid;
      original_x.Total = x.Total;
      original_x.Hide = x.Hide;
      original_x.Statusid = x.Statusid;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class SenderId {
    // This method retrieves all SenderIds.
    // Change this method to alter how records are retrieved.
    public static IQueryable<SenderId> GetAllSenderIds() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.SenderIds;
    }
    // This method gets record counts of all SenderIds.
    // Do not change this method.
    public static int GetAllSenderIdsCount() {
      return GetAllSenderIds().Count();
    }
    // This method retrieves a single SenderId.
    // Change this method to alter how that record is received.
    public static SenderId GetSenderId(Decimal Tableid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.SenderIds.Where(x=>x.Tableid == Tableid).FirstOrDefault();
    }
    // This method retrieves SenderIds by User.
    // Change this method to alter how records are retrieved.
    public static IQueryable<SenderId> GetSenderIdsByUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).SelectMany(x=>x.SenderIds);
    }
    // This method gets sorted and paged records of all SenderIds filtered by a specified field.
    // Do not change this method.
    public static IQueryable<SenderId> GetSenderIds(string tableName, String SenderIds_Username, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<SenderId> x = GetFilteredSenderIds(tableName, SenderIds_Username);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "Tableid");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<SenderId> GetFilteredSenderIds(string tableName, String SenderIds_Username) {
      switch (tableName) {
        case "User_SenderIds":
          return GetSenderIdsByUser(SenderIds_Username);
        default:
          return GetAllSenderIds();
      }
    }
    // This method gets records counts of all SenderIds filtered by a specified field.
    // Do not change this method.
    public static int GetSenderIdsCount(string tableName, String SenderIds_Username) {
      IQueryable<SenderId> x = GetFilteredSenderIds(tableName, SenderIds_Username);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(SenderId x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.SenderIds.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(SenderId x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.SenderIds.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(SenderId original_x, SenderId x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.SenderIds.Attach(original_x);
      original_x.Username = x.Username;
      original_x.Senderid = x.Senderid;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class Country {
    // This method retrieves all Countries.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Country> GetAllCountries() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Countries;
    }
    // This method gets record counts of all Countries.
    // Do not change this method.
    public static int GetAllCountriesCount() {
      return GetAllCountries().Count();
    }
    // This method retrieves a single Country.
    // Change this method to alter how that record is received.
    public static Country GetCountry(Int32 Code) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Countries.Where(x=>x.Code == Code).FirstOrDefault();
    }
    // This method pages and sorts over all Countries.
    // Do not change this method.
    public static IQueryable<Country> GetAllCountries(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllCountries().SortAndPage(sortExpression, startRowIndex, maximumRows, "Code");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(Country x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Countries.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(Country x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Countries.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(Country original_x, Country x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Countries.Attach(original_x);
      original_x.Content = x.Content;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class Operator {
    // This method retrieves all Operators.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Operator> GetAllOperators() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Operators;
    }
    // This method gets record counts of all Operators.
    // Do not change this method.
    public static int GetAllOperatorsCount() {
      return GetAllOperators().Count();
    }
    // This method retrieves a single Operator.
    // Change this method to alter how that record is received.
    public static Operator GetOperator(Int32 OpCode) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Operators.Where(x=>x.OpCode == OpCode).FirstOrDefault();
    }
    // This method retrieves Operators by Country.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Operator> GetOperatorsByCountry(Int32 Code) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Countries.Where(x=>x.Code == Code).SelectMany(x=>x.Operators);
    }
    // This method gets sorted and paged records of all Operators filtered by a specified field.
    // Do not change this method.
    public static IQueryable<Operator> GetOperators(string tableName, Int32 Operators_Code, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<Operator> x = GetFilteredOperators(tableName, Operators_Code);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "OpCode");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<Operator> GetFilteredOperators(string tableName, Int32 Operators_Code) {
      switch (tableName) {
        case "Country_Operators":
          return GetOperatorsByCountry(Operators_Code);
        default:
          return GetAllOperators();
      }
    }
    // This method gets records counts of all Operators filtered by a specified field.
    // Do not change this method.
    public static int GetOperatorsCount(string tableName, Int32 Operators_Code) {
      IQueryable<Operator> x = GetFilteredOperators(tableName, Operators_Code);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(Operator x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Operators.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(Operator x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Operators.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(Operator original_x, Operator x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Operators.Attach(original_x);
      original_x.Code = x.Code;
      original_x.OpName = x.OpName;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class MsgOutbox {
    // This method retrieves all MsgOutboxes.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgOutbox> GetAllMsgOutboxes() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgOutboxes;
    }
    // This method gets record counts of all MsgOutboxes.
    // Do not change this method.
    public static int GetAllMsgOutboxesCount() {
      return GetAllMsgOutboxes().Count();
    }
    // This method retrieves a single MsgOutbox.
    // Change this method to alter how that record is received.
    public static MsgOutbox GetMsgOutbox(Int32 MsgOutboxId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgOutboxes.Where(x=>x.MsgOutboxId == MsgOutboxId).FirstOrDefault();
    }
    // This method retrieves MsgOutboxes by MsgMain.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgOutbox> GetMsgOutboxesByMsgMain(Int32 Msgid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgMains.Where(x=>x.Msgid == Msgid).SelectMany(x=>x.MsgOutboxes);
    }
    // This method retrieves MsgOutboxes by User.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgOutbox> GetMsgOutboxesByUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).SelectMany(x=>x.MsgOutboxes);
    }
    // This method gets sorted and paged records of all MsgOutboxes filtered by a specified field.
    // Do not change this method.
    public static IQueryable<MsgOutbox> GetMsgOutboxes(string tableName, Int32 MsgOutboxes_Msgid, String MsgOutboxes_Username, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<MsgOutbox> x = GetFilteredMsgOutboxes(tableName, MsgOutboxes_Msgid, MsgOutboxes_Username);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "MsgOutboxId");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<MsgOutbox> GetFilteredMsgOutboxes(string tableName, Int32 MsgOutboxes_Msgid, String MsgOutboxes_Username) {
      switch (tableName) {
        case "MsgMain_MsgOutboxes":
          return GetMsgOutboxesByMsgMain(MsgOutboxes_Msgid);
        case "User_MsgOutboxes":
          return GetMsgOutboxesByUser(MsgOutboxes_Username);
        default:
          return GetAllMsgOutboxes();
      }
    }
    // This method gets records counts of all MsgOutboxes filtered by a specified field.
    // Do not change this method.
    public static int GetMsgOutboxesCount(string tableName, Int32 MsgOutboxes_Msgid, String MsgOutboxes_Username) {
      IQueryable<MsgOutbox> x = GetFilteredMsgOutboxes(tableName, MsgOutboxes_Msgid, MsgOutboxes_Username);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(MsgOutbox x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgOutboxes.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(MsgOutbox x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgOutboxes.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(MsgOutbox original_x, MsgOutbox x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgOutboxes.Attach(original_x);
      original_x.Mobile = x.Mobile;
      original_x.Username = x.Username;
      original_x.Msgid = x.Msgid;
      original_x.Groupid = x.Groupid;
      original_x.Statusid = x.Statusid;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class MsgMain {
    // This method retrieves all MsgMains.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgMain> GetAllMsgMains() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgMains;
    }
    // This method gets record counts of all MsgMains.
    // Do not change this method.
    public static int GetAllMsgMainsCount() {
      return GetAllMsgMains().Count();
    }
    // This method retrieves a single MsgMain.
    // Change this method to alter how that record is received.
    public static MsgMain GetMsgMain(Int32 Msgid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgMains.Where(x=>x.Msgid == Msgid).FirstOrDefault();
    }
    // This method retrieves MsgMains by SenderId.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgMain> GetMsgMainsBySenderId(Decimal Tableid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.SenderIds.Where(x=>x.Tableid == Tableid).SelectMany(x=>x.MsgMains);
    }
    // This method retrieves MsgMains by User.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgMain> GetMsgMainsByUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).SelectMany(x=>x.MsgMains);
    }
    // This method gets sorted and paged records of all MsgMains filtered by a specified field.
    // Do not change this method.
    public static IQueryable<MsgMain> GetMsgMains(string tableName, Decimal MsgMains_Tableid, String MsgMains_Username, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<MsgMain> x = GetFilteredMsgMains(tableName, MsgMains_Tableid, MsgMains_Username);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "Msgid");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<MsgMain> GetFilteredMsgMains(string tableName, Decimal MsgMains_Tableid, String MsgMains_Username) {
      switch (tableName) {
        case "SenderId_MsgMains":
          return GetMsgMainsBySenderId(MsgMains_Tableid);
        case "User_MsgMains":
          return GetMsgMainsByUser(MsgMains_Username);
        default:
          return GetAllMsgMains();
      }
    }
    // This method gets records counts of all MsgMains filtered by a specified field.
    // Do not change this method.
    public static int GetMsgMainsCount(string tableName, Decimal MsgMains_Tableid, String MsgMains_Username) {
      IQueryable<MsgMain> x = GetFilteredMsgMains(tableName, MsgMains_Tableid, MsgMains_Username);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(MsgMain x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgMains.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(MsgMain x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgMains.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(MsgMain original_x, MsgMain x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgMains.Attach(original_x);
      original_x.Username = x.Username;
      original_x.Message = x.Message;
      original_x.Type = x.Type;
      original_x.Sender = x.Sender;
      original_x.Date = x.Date;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class MsgTemplates {
    // This method retrieves all MsgTemplates.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgTemplates> GetAllMsgTemplates() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgTemplates;
    }
    // This method gets record counts of all MsgTemplates.
    // Do not change this method.
    public static int GetAllMsgTemplatesCount() {
      return GetAllMsgTemplates().Count();
    }
    // This method retrieves a single MsgTemplates.
    // Change this method to alter how that record is received.
    public static MsgTemplates GetMsgTemplates(Int32 Tempid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgTemplates.Where(x=>x.Tempid == Tempid).FirstOrDefault();
    }
    // This method pages and sorts over all MsgTemplates.
    // Do not change this method.
    public static IQueryable<MsgTemplates> GetAllMsgTemplates(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllMsgTemplates().SortAndPage(sortExpression, startRowIndex, maximumRows, "Tempid");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(MsgTemplates x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgTemplates.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(MsgTemplates x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgTemplates.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(MsgTemplates original_x, MsgTemplates x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgTemplates.Attach(original_x);
      original_x.Userid = x.Userid;
      original_x.Template = x.Template;
      original_x.MobileColumn = x.MobileColumn;
      original_x.Type = x.Type;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class PointHistory {
    // This method retrieves all PointHistories.
    // Change this method to alter how records are retrieved.
    public static IQueryable<PointHistory> GetAllPointHistories() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.PointHistories;
    }
    // This method gets record counts of all PointHistories.
    // Do not change this method.
    public static int GetAllPointHistoriesCount() {
      return GetAllPointHistories().Count();
    }
    // This method retrieves a single PointHistory.
    // Change this method to alter how that record is received.
    public static PointHistory GetPointHistory(Int32 PointHistoryId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.PointHistories.Where(x=>x.PointHistoryId == PointHistoryId).FirstOrDefault();
    }
    // This method retrieves PointHistories by User.
    // Change this method to alter how records are retrieved.
    public static IQueryable<PointHistory> GetPointHistoriesByUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).SelectMany(x=>x.PointHistories);
    }
    // This method gets sorted and paged records of all PointHistories filtered by a specified field.
    // Do not change this method.
    public static IQueryable<PointHistory> GetPointHistories(string tableName, String PointHistories_Username, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<PointHistory> x = GetFilteredPointHistories(tableName, PointHistories_Username);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "PointHistoryId");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<PointHistory> GetFilteredPointHistories(string tableName, String PointHistories_Username) {
      switch (tableName) {
        case "User_PointHistories":
          return GetPointHistoriesByUser(PointHistories_Username);
        default:
          return GetAllPointHistories();
      }
    }
    // This method gets records counts of all PointHistories filtered by a specified field.
    // Do not change this method.
    public static int GetPointHistoriesCount(string tableName, String PointHistories_Username) {
      IQueryable<PointHistory> x = GetFilteredPointHistories(tableName, PointHistories_Username);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(PointHistory x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.PointHistories.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(PointHistory x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.PointHistories.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(PointHistory original_x, PointHistory x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.PointHistories.Attach(original_x);
      original_x.Username = x.Username;
      original_x.Date = x.Date;
      original_x.Debit = x.Debit;
      original_x.Credit = x.Credit;
      original_x.Details = x.Details;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class New {
    // This method retrieves all News.
    // Change this method to alter how records are retrieved.
    public static IQueryable<New> GetAllNews() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.News;
    }
    // This method gets record counts of all News.
    // Do not change this method.
    public static int GetAllNewsCount() {
      return GetAllNews().Count();
    }
    // This method retrieves a single New.
    // Change this method to alter how that record is received.
    public static New GetNew(Int32 NewsId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.News.Where(x=>x.NewsId == NewsId).FirstOrDefault();
    }
    // This method pages and sorts over all News.
    // Do not change this method.
    public static IQueryable<New> GetAllNews(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllNews().SortAndPage(sortExpression, startRowIndex, maximumRows, "NewsId");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(New x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.News.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(New x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.News.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(New original_x, New x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.News.Attach(original_x);
      original_x.Title = x.Title;
      original_x.News = x.News;
      original_x.NewsDate = x.NewsDate;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class Payment {
    // This method retrieves all Payments.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Payment> GetAllPayments() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Payments;
    }
    // This method gets record counts of all Payments.
    // Do not change this method.
    public static int GetAllPaymentsCount() {
      return GetAllPayments().Count();
    }
    // This method retrieves a single Payment.
    // Change this method to alter how that record is received.
    public static Payment GetPayment(String PaymentID) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Payments.Where(x=>x.PaymentID == PaymentID).FirstOrDefault();
    }
    // This method pages and sorts over all Payments.
    // Do not change this method.
    public static IQueryable<Payment> GetAllPayments(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllPayments().SortAndPage(sortExpression, startRowIndex, maximumRows, "PaymentID");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(Payment x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Payments.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(Payment x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Payments.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(Payment original_x, Payment x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Payments.Attach(original_x);
      original_x.ResultCode = x.ResultCode;
      original_x.AuthCode = x.AuthCode;
      original_x.TranID = x.TranID;
      original_x.PostDate = x.PostDate;
      original_x.TrackID = x.TrackID;
      original_x.Udf2 = x.Udf2;
      original_x.CurrentTime = x.CurrentTime;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class MsgDetail {
    // This method retrieves all MsgDetails.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgDetail> GetAllMsgDetails() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgDetails;
    }
    // This method gets record counts of all MsgDetails.
    // Do not change this method.
    public static int GetAllMsgDetailsCount() {
      return GetAllMsgDetails().Count();
    }
    // This method retrieves a single MsgDetail.
    // Change this method to alter how that record is received.
    public static MsgDetail GetMsgDetail(Int32 MsgDetailId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgDetails.Where(x=>x.MsgDetailId == MsgDetailId).FirstOrDefault();
    }
    // This method retrieves MsgDetails by MsgMain.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgDetail> GetMsgDetailsByMsgMain(Int32 Msgid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.MsgMains.Where(x=>x.Msgid == Msgid).SelectMany(x=>x.MsgDetails);
    }
    // This method retrieves MsgDetails by GrpMain.
    // Change this method to alter how records are retrieved.
    public static IQueryable<MsgDetail> GetMsgDetailsByGrpMain(Int32 Groupid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpMains.Where(x=>x.Groupid == Groupid).SelectMany(x=>x.MsgDetails);
    }
    // This method gets sorted and paged records of all MsgDetails filtered by a specified field.
    // Do not change this method.
    public static IQueryable<MsgDetail> GetMsgDetails(string tableName, Int32 MsgDetails_Msgid, Int32 MsgDetails_Groupid, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<MsgDetail> x = GetFilteredMsgDetails(tableName, MsgDetails_Msgid, MsgDetails_Groupid);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "MsgDetailId");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<MsgDetail> GetFilteredMsgDetails(string tableName, Int32 MsgDetails_Msgid, Int32 MsgDetails_Groupid) {
      switch (tableName) {
        case "MsgMain_MsgDetails":
          return GetMsgDetailsByMsgMain(MsgDetails_Msgid);
        case "GrpMain_MsgDetails":
          return GetMsgDetailsByGrpMain(MsgDetails_Groupid);
        default:
          return GetAllMsgDetails();
      }
    }
    // This method gets records counts of all MsgDetails filtered by a specified field.
    // Do not change this method.
    public static int GetMsgDetailsCount(string tableName, Int32 MsgDetails_Msgid, Int32 MsgDetails_Groupid) {
      IQueryable<MsgDetail> x = GetFilteredMsgDetails(tableName, MsgDetails_Msgid, MsgDetails_Groupid);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(MsgDetail x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgDetails.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(MsgDetail x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgDetails.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(MsgDetail original_x, MsgDetail x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.MsgDetails.Attach(original_x);
      original_x.Mobile = x.Mobile;
      original_x.Groupid = x.Groupid;
      original_x.Msgid = x.Msgid;
      original_x.Hide = x.Hide;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class GrpMain {
    // This method retrieves all GrpMains.
    // Change this method to alter how records are retrieved.
    public static IQueryable<GrpMain> GetAllGrpMains() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpMains;
    }
    // This method gets record counts of all GrpMains.
    // Do not change this method.
    public static int GetAllGrpMainsCount() {
      return GetAllGrpMains().Count();
    }
    // This method retrieves a single GrpMain.
    // Change this method to alter how that record is received.
    public static GrpMain GetGrpMain(Int32 Groupid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpMains.Where(x=>x.Groupid == Groupid).FirstOrDefault();
    }
    // This method retrieves GrpMains by User.
    // Change this method to alter how records are retrieved.
    public static IQueryable<GrpMain> GetGrpMainsByUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).SelectMany(x=>x.GrpMains);
    }
    // This method gets sorted and paged records of all GrpMains filtered by a specified field.
    // Do not change this method.
    public static IQueryable<GrpMain> GetGrpMains(string tableName, String GrpMains_Username, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<GrpMain> x = GetFilteredGrpMains(tableName, GrpMains_Username);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "Groupid");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<GrpMain> GetFilteredGrpMains(string tableName, String GrpMains_Username) {
      switch (tableName) {
        case "User_GrpMains":
          return GetGrpMainsByUser(GrpMains_Username);
        default:
          return GetAllGrpMains();
      }
    }
    // This method gets records counts of all GrpMains filtered by a specified field.
    // Do not change this method.
    public static int GetGrpMainsCount(string tableName, String GrpMains_Username) {
      IQueryable<GrpMain> x = GetFilteredGrpMains(tableName, GrpMains_Username);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(GrpMain x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.GrpMains.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(GrpMain x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.GrpMains.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(GrpMain original_x, GrpMain x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.GrpMains.Attach(original_x);
      original_x.Username = x.Username;
      original_x.Groupname = x.Groupname;
      original_x.Detail = x.Detail;
      original_x.Grouporder = x.Grouporder;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class GrpDetail {
    // This method retrieves all GrpDetails.
    // Change this method to alter how records are retrieved.
    public static IQueryable<GrpDetail> GetAllGrpDetails() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpDetails;
    }
    // This method gets record counts of all GrpDetails.
    // Do not change this method.
    public static int GetAllGrpDetailsCount() {
      return GetAllGrpDetails().Count();
    }
    // This method retrieves a single GrpDetail.
    // Change this method to alter how that record is received.
    public static GrpDetail GetGrpDetail(Decimal GrpDetailId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpDetails.Where(x=>x.GrpDetailId == GrpDetailId).FirstOrDefault();
    }
    // This method retrieves GrpDetails by GrpMain.
    // Change this method to alter how records are retrieved.
    public static IQueryable<GrpDetail> GetGrpDetailsByGrpMain(Int32 Groupid) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.GrpMains.Where(x=>x.Groupid == Groupid).SelectMany(x=>x.GrpDetails);
    }
    // This method gets sorted and paged records of all GrpDetails filtered by a specified field.
    // Do not change this method.
    public static IQueryable<GrpDetail> GetGrpDetails(string tableName, Int32 GrpDetails_Groupid, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<GrpDetail> x = GetFilteredGrpDetails(tableName, GrpDetails_Groupid);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "GrpDetailId");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<GrpDetail> GetFilteredGrpDetails(string tableName, Int32 GrpDetails_Groupid) {
      switch (tableName) {
        case "GrpMain_GrpDetails":
          return GetGrpDetailsByGrpMain(GrpDetails_Groupid);
        default:
          return GetAllGrpDetails();
      }
    }
    // This method gets records counts of all GrpDetails filtered by a specified field.
    // Do not change this method.
    public static int GetGrpDetailsCount(string tableName, Int32 GrpDetails_Groupid) {
      IQueryable<GrpDetail> x = GetFilteredGrpDetails(tableName, GrpDetails_Groupid);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(GrpDetail x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.GrpDetails.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(GrpDetail x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.GrpDetails.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(GrpDetail original_x, GrpDetail x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.GrpDetails.Attach(original_x);
      original_x.Groupid = x.Groupid;
      original_x.Mobile = x.Mobile;
      original_x.Name = x.Name;
      original_x.Detail = x.Detail;
      original_x.Picked = x.Picked;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class PriorityGroup {
    // This method retrieves all PriorityGroups.
    // Change this method to alter how records are retrieved.
    public static IQueryable<PriorityGroup> GetAllPriorityGroups() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.PriorityGroups;
    }
    // This method gets record counts of all PriorityGroups.
    // Do not change this method.
    public static int GetAllPriorityGroupsCount() {
      return GetAllPriorityGroups().Count();
    }
    // This method retrieves a single PriorityGroup.
    // Change this method to alter how that record is received.
    public static PriorityGroup GetPriorityGroup(Int32 PriorityGroupId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.PriorityGroups.Where(x=>x.PriorityGroupId == PriorityGroupId).FirstOrDefault();
    }
    // This method pages and sorts over all PriorityGroups.
    // Do not change this method.
    public static IQueryable<PriorityGroup> GetAllPriorityGroups(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllPriorityGroups().SortAndPage(sortExpression, startRowIndex, maximumRows, "PriorityGroupId");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(PriorityGroup x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.PriorityGroups.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(PriorityGroup x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.PriorityGroups.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(PriorityGroup original_x, PriorityGroup x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.PriorityGroups.Attach(original_x);
      original_x.GroupName = x.GroupName;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class User {
    // This method retrieves all Users.
    // Change this method to alter how records are retrieved.
    public static IQueryable<User> GetAllUsers() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users;
    }
    // This method gets record counts of all Users.
    // Do not change this method.
    public static int GetAllUsersCount() {
      return GetAllUsers().Count();
    }
    // This method retrieves a single User.
    // Change this method to alter how that record is received.
    public static User GetUser(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Users.Where(x=>x.Username == Username).FirstOrDefault();
    }
    // This method retrieves Users by PriorityGroup.
    // Change this method to alter how records are retrieved.
    public static IQueryable<User> GetUsersByPriorityGroup(Int32 PriorityGroupId) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.PriorityGroups.Where(x=>x.PriorityGroupId == PriorityGroupId).SelectMany(x=>x.Users);
    }
    // This method gets sorted and paged records of all Users filtered by a specified field.
    // Do not change this method.
    public static IQueryable<User> GetUsers(string tableName, Int32 Users_PriorityGroupId, string sortExpression, int startRowIndex, int maximumRows) {
      IQueryable<User> x = GetFilteredUsers(tableName, Users_PriorityGroupId);
      return x.SortAndPage(sortExpression, startRowIndex, maximumRows, "Username");
    }
    // This method routes a request for filtering by a field value to another method.
    // Do not change this method.
    private static IQueryable<User> GetFilteredUsers(string tableName, Int32 Users_PriorityGroupId) {
      switch (tableName) {
        case "PriorityGroup_Users":
          return GetUsersByPriorityGroup(Users_PriorityGroupId);
        default:
          return GetAllUsers();
      }
    }
    // This method gets records counts of all Users filtered by a specified field.
    // Do not change this method.
    public static int GetUsersCount(string tableName, Int32 Users_PriorityGroupId) {
      IQueryable<User> x = GetFilteredUsers(tableName, Users_PriorityGroupId);
      return x.Count();
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(User x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Users.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(User x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Users.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(User original_x, User x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Users.Attach(original_x);
      original_x.ProorityGroup = x.ProorityGroup;
      original_x.Userid = x.Userid;
      original_x.Name = x.Name;
      original_x.Country = x.Country;
      original_x.City = x.City;
      original_x.Sex = x.Sex;
      original_x.Birthday = x.Birthday;
      original_x.Tel = x.Tel;
      original_x.Mobileno = x.Mobileno;
      original_x.Mobileno2 = x.Mobileno2;
      original_x.Fax = x.Fax;
      original_x.Email = x.Email;
      original_x.Website = x.Website;
      original_x.Regcode = x.Regcode;
      original_x.Active = x.Active;
      original_x.Points = x.Points;
      original_x.Regdate = x.Regdate;
      original_x.Passcode = x.Passcode;
      original_x.Lastvisit = x.Lastvisit;
      original_x.Expirydate = x.Expirydate;
      original_x.Company = x.Company;
      original_x.Timezone = x.Timezone;
      db.SubmitChanges();
      return 1;
    }
  }

  public partial class Admin {
    // This method retrieves all Admins.
    // Change this method to alter how records are retrieved.
    public static IQueryable<Admin> GetAllAdmins() {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Admins;
    }
    // This method gets record counts of all Admins.
    // Do not change this method.
    public static int GetAllAdminsCount() {
      return GetAllAdmins().Count();
    }
    // This method retrieves a single Admin.
    // Change this method to alter how that record is received.
    public static Admin GetAdmin(String Username) {
      Bulksms db = Bulksms.CreateDataContext();
      return db.Admins.Where(x=>x.Username == Username).FirstOrDefault();
    }
    // This method pages and sorts over all Admins.
    // Do not change this method.
    public static IQueryable<Admin> GetAllAdmins(string sortExpression, int startRowIndex, int maximumRows) {
      return GetAllAdmins().SortAndPage(sortExpression, startRowIndex, maximumRows, "Username");
    }
    // This method deletes a record in the table.
    // Change this method to alter how records are deleted.
    public static int Delete(Admin x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Admins.Remove(x);
      db.SubmitChanges();
      return 1;
    }
    // This method inserts a new record in the table.
    // Change this method to alter how records are inserted.
    public static int Insert(Admin x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Admins.Add(x);
      db.SubmitChanges();
      return 1;
    }
    // This method updates a record in the table.
    // Change this method to alter how records are updated.
    public static int Update(Admin original_x, Admin x) {
      Bulksms db = Bulksms.CreateDataContext();
      db.Admins.Attach(original_x);
      original_x.Password = x.Password;
      original_x.Lastvisit = x.Lastvisit;
      original_x.Sold = x.Sold;
      db.SubmitChanges();
      return 1;
    }
  }
}
