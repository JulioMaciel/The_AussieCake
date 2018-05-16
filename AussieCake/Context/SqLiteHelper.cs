using AussieCake.Helper;
using AussieCake.Models;
using AussieCake.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

/// <summary>
/// The Entity Framework Database Context.
/// </summary>
public class SqLiteHelper : SqLiteCommands
{
  private static string InsertSQL = "insert into {0} values(NULL, ";
	private static string UpdateSQL = "update {0} set {1} where Id = {2}";
	private static string RemoveSQL = "delete from {0} where Id = {1}";

	protected static List<Collocation> CollocationsDB { get; set; }
	protected static List<User> UsersDB { get; set; }
	protected static List<CollocationAttempt> CollocationAttemptsDB { get; set; }
	protected static List<Sentence> SentencesDB { get; set; }
	protected static List<Verb> VerbsDB { get; set; }

	public static bool WasDataBaseEmpty = false;

	public static void InitializeDB()
	{
		CreateIfEmptyDB();
		GetRawsFromDB();
  }

	#region GetDB

	protected static void GetCollocationsDB()
	{		
		var dataset = GetTable(ModelsType.Collocation);

		CollocationsDB = dataset.Tables[0]
									 				 .AsEnumerable()
													 .Select(dataRow => new Collocation(
														 Convert.ToInt16(dataRow.Field<Int64>("Id")),
														 dataRow.Field<string>("Prefixes"),
														 dataRow.Field<string>("Component1"),
														 dataRow.Field<string>("LinkWords"),
														 dataRow.Field<string>("Component2"),
														 dataRow.Field<string>("Suffixes"),
                             dataRow.Field<string>("PtBr"),
                             Convert.ToInt16(dataRow.Field<Int64>("Importance")),
                             dataRow.Field<string>("SentencesId")))
													 .ToList();
	}

	protected static void GetUsersDB()
	{
		var dataset = GetTable(ModelsType.User);

		UsersDB = dataset.Tables[0]
														.AsEnumerable()
													 .Select(dataRow => new User(
														 Convert.ToInt16(dataRow.Field<Int64>("Id")),
														 dataRow.Field<string>("Logins"),
														 dataRow.Field<string>("Password")))
													 .ToList();
	}

	protected static void GetSentencesDB()
	{
		var dataset = GetTable(ModelsType.Sentence);

		SentencesDB = dataset.Tables[0]
														.AsEnumerable()
													 .Select(dataRow => new Sentence(
														 Convert.ToInt16(dataRow.Field<Int64>("Id")),
														 dataRow.Field<string>("Text"),
														 dataRow.Field<string>("PtBr")))
													 .ToList();
	}

	protected static void GetCollocationAttemptsDB()
	{
		var dataset = GetTable(ModelsType.CollocationAttempt);
		
		CollocationAttemptsDB = dataset.Tables[0]
														.AsEnumerable()
													 .Select(dataRow => new CollocationAttempt(
														 Convert.ToInt16(dataRow.Field<Int64>("Id")),
														 Convert.ToInt16(dataRow.Field<Int64>("IdUser")),
														 Convert.ToInt16(dataRow.Field<Int64>("IdCollocation")),
														 Convert.ToBoolean(dataRow.Field<Int64>("IsCorrect")),
														 Convert.ToDateTime(dataRow.Field<string>("When"))))
													 .ToList();
	}

	protected static void GetVerbsDB()
	{
		var dataset = GetTable(ModelsType.Verb);

		VerbsDB = dataset.Tables[0]
														.AsEnumerable()
													 .Select(dataRow => new Verb(
														 dataRow.Field<string>("Infinitive"),
														 dataRow.Field<string>("Past"),
														 dataRow.Field<string>("PP"),
														 dataRow.Field<string>("Gerund"),
														 dataRow.Field<string>("Person")))
													 .ToList();
	}

	#endregion

	#region Inserts

	protected static void InsertCollocation(Collocation col)
	{
		string query = string.Format(InsertSQL +
																"'{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
																  ModelsType.Collocation.ToDescString(),
																	col.Prefixes, col.Component1, col.LinkWords, col.Component2, 
																	col.Suffixes, col.PtBr, col.SentencesId);
		SendQuery(query);

		//insert in memory
		GetCollocationsDB();
	}

	protected static void InsertUser(User user)
	{
		string query = string.Format(InsertSQL + 
																	"'{1}', '{2}')", 
																	ModelsType.User.ToDescString(),
																	user.Logins, user.Password);
		SendQuery(query);

		//insert in memory
		GetUsersDB();
	}

	protected static void InsertSentence(Sentence sen)
	{
		string query = string.Format(InsertSQL +
																"'{1}', '{2}')",
																	ModelsType.Sentence.ToDescString(),
																	sen.Text, sen.PtBr);
		SendQuery(query);

		//insert in memory
		GetSentencesDB();
	}

	protected static void InsertCollocationAttempt(CollocationAttempt col)
	{
		string query = string.Format(InsertSQL +
																"'{1}', '{2}', '{3}', '{4}')",
																	ModelsType.CollocationAttempt.ToDescString(),
																	col.IdUser, col.IdCollocation, col.IsCorrect, col.When);
		SendQuery(query);

		//insert in memory
		GetCollocationAttemptsDB(); // talvez pegar o id da question e chamar metodo para atualizar
	}

	protected static void InsertVerb(Verb verb)
	{
		string query = string.Format(InsertSQL +
																	"'{1}', '{2}', '{3}', '{4}', '{5}')",
																	ModelsType.Verb.ToDescString(),
																	verb.Infinitive, verb.Past, verb.PastParticiple, verb.Person, verb.Gerund);
		SendQuery(query);

		//insert in memory
		GetVerbsDB();
	}

	#endregion

	#region Updates

	protected static void UpdateCollocation(Collocation col)
	{
		var oldCol = CollocationsDB.FirstOrDefault(c => c.Id == col.Id);
		int field = 0;
		string columnsToUpdate = string.Empty;

		if (oldCol.Prefixes != col.Prefixes)
		{
			columnsToUpdate += "Prefixes = " + "'" + col.Prefixes + "'";
			field++;
		}
		if (oldCol.Component1 != col.Component1)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "Component1 = " + "'" + col.Component1 + "'";
			field++;
		}
		if (oldCol.LinkWords != col.LinkWords)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "LinkWords = " + "'" + col.LinkWords + "'";
			field++;
		}
		if (oldCol.Component2 != col.Component2)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "Component2 = " + "'" + col.Component2 + "'";
			field++;
		}
		if (oldCol.Suffixes != col.Suffixes)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "Suffixes = " + "'" + col.Suffixes + "'";
			field++;
		}
		if (oldCol.PtBr != col.PtBr)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "PtBr = " + "'" + col.PtBr + "'";
			field++;
		}
		if (oldCol.SentencesId != col.SentencesId)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "SentencesId = " + "'" + col.SentencesId + "'";
			field++;
		}

		//update DB
		string query = string.Format(UpdateSQL,
																	ModelsType.Collocation.ToDescString(),
																	columnsToUpdate,
																	col.Id);
		SendQuery(query);

		//update in memory
		oldCol = col;
	}

	protected static void UpdateUser(User user)
	{
		var oldUser = UsersDB.FirstOrDefault(c => c.Id == user.Id);
		int field = 0;
		string columnsToUpdate = string.Empty;

		if (oldUser.Logins != user.Logins)
		{
			columnsToUpdate += "Logins = " + "'" + user.Logins + "'";
			field++;
		}
		if (oldUser.Password != user.Password)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "Password = " + "'" + user.Password + "'";
			field++;
		}

		//update DB
		string query = string.Format(UpdateSQL,
																	ModelsType.User.ToDescString(),
																	columnsToUpdate,
																	user.Id);
		SendQuery(query);

		//update in memory
		oldUser = user;
	}

	protected static void UpdateSentence(Sentence sen)
	{
		var oldSen = SentencesDB.FirstOrDefault(c => c.Id == sen.Id);
		int field = 0;
		string columnsToUpdate = string.Empty;

		if (oldSen.Text != sen.Text)
		{
			columnsToUpdate += "Text = " + "'" + sen.Text + "'";
			field++;
		}
		if (oldSen.PtBr != sen.PtBr)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "PtBr = " + "'" + sen.PtBr + "'";
			field++;
		}

		//update DB
		string query = string.Format(UpdateSQL,
																	ModelsType.Sentence.ToDescString(),
																	columnsToUpdate,
																	sen.Id);
		SendQuery(query);

		//update in memory
		oldSen = sen;
	}

	protected static void UpdateCollocationAttempt(CollocationAttempt col)
	{
		var oldcol = CollocationAttemptsDB.FirstOrDefault(c => c.Id == col.Id);
		int field = 0;
		string columnsToUpdate = string.Empty;

		if (oldcol.IdCollocation != col.IdCollocation)
		{
			columnsToUpdate += "IdCollocation = " + "'" + col.IdCollocation + "'";
			field++;
		}
		if (oldcol.IsCorrect != col.IsCorrect)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "IsCorrect = " + "'" + col.IsCorrect + "'";
			field++;
		}
		if (oldcol.When != col.When)
		{
			columnsToUpdate += field > 0 ? ", " : string.Empty;
			columnsToUpdate += "When = " + "'" + col.When + "'";
			field++;
		}

		//update DB
		string query = string.Format(UpdateSQL,
																	ModelsType.CollocationAttempt.ToDescString(),
																	columnsToUpdate,
																	col.Id);
		SendQuery(query);

		//update in memory
		oldcol = col;
	}

	#endregion

	#region Removes

	protected static void RemoveCollocation(Collocation col)
	{
		string query = string.Format(RemoveSQL,
																	ModelsType.Collocation.ToDescString(),
																	col.Id);
		SendQuery(query);

		// in memory
		CollocationsDB.Remove(col);
	}

	protected static void RemoveUser(User user)
	{
		string query = string.Format(RemoveSQL,
																	ModelsType.User.ToDescString(),
																	user.Id);
		SendQuery(query);

		// in memory
		UsersDB.Remove(user);
	}

	protected static void RemoveSentence(Sentence sen)
	{
		string query = string.Format(RemoveSQL,
																	ModelsType.Sentence.ToDescString(),
																	sen.Id);
		SendQuery(query);

		// in memory
		SentencesDB.Remove(sen);
	}

	protected static void RemoveCollocationAttempt(CollocationAttempt col)
	{
		string query = string.Format(RemoveSQL,
																	ModelsType.CollocationAttempt.ToDescString(),
																	col.Id);
		SendQuery(query);

		// in memory
		CollocationAttemptsDB.Remove(col);
	}

	#endregion

	private static void GetRawsFromDB()
	{
		GetCollocationsDB();
		GetSentencesDB();
		GetUsersDB();
		GetCollocationAttemptsDB();
		GetVerbsDB();
	}

	private static void CreateIfEmptyDB()
	{
		SendQuery("CREATE TABLE IF NOT EXISTS 'Collocation' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 'Prefixes' TEXT, 'Component1' TEXT NOT NULL, 'LinkWords' TEXT, 'Component2' TEXT NOT NULL, 'Suffixes' TEXT, 'PtBr' TEXT, 'Importance' INTEGER NOT NULL, 'SentencesId' TEXT )");
		SendQuery("CREATE TABLE IF NOT EXISTS 'CollocationAttempt' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 'IdUser' INTEGER NOT NULL, 'IdCollocation' INTEGER NOT NULL, 'IsCorrect' INTEGER NOT NULL, 'When' TEXT NOT NULL )");
		SendQuery("CREATE TABLE IF NOT EXISTS 'Sentence' ( 'Id' INTEGER NOT NULL CONSTRAINT 'PK_Sentence' PRIMARY KEY AUTOINCREMENT, 'Text' TEXT NOT NULL, 'PtBr' TEXT NULL)");
		SendQuery("CREATE TABLE IF NOT EXISTS 'User' ( 'Id' INTEGER NOT NULL CONSTRAINT 'PK_User' PRIMARY KEY AUTOINCREMENT, 'Logins' TEXT NOT NULL, 'Password' TEXT )");
		SendQuery("CREATE TABLE IF NOT EXISTS 'Verb' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 'Infinitive' TEXT NOT NULL, 'Past' TEXT NOT NULL, 'PP' TEXT NOT NULL, 'Gerund' TEXT NOT NULL, 'Person' TEXT NOT NULL )");

		InsertStaticValues();
	}

	private static void InsertStaticValues()
	{
		WasDataBaseEmpty = (GetTable(ModelsType.Verb).Tables[0].Rows.Count == 0) ||
											 (GetTable(ModelsType.Collocation).Tables[0].Rows.Count == 0) ||
											 (GetTable(ModelsType.Sentence).Tables[0].Rows.Count == 0);

		if (WasDataBaseEmpty)
		{
			var verb_lines = File.ReadAllLines(CakePaths.ScriptVerbs);
			var verb_joined = String.Join(Environment.NewLine, verb_lines);
			SendQuery(verb_joined);

			var col_lines = File.ReadAllLines(CakePaths.ScriptCollocations);
			var col_joined = String.Join(Environment.NewLine, col_lines);
			SendQuery(col_joined);

			var sen_lines = File.ReadAllLines(CakePaths.ScriptSentences);
			var sen_joined = String.Join(Environment.NewLine, sen_lines);
			SendQuery(sen_joined);
		}
  }
}