using AussieCake.Attempt;
using AussieCake.Question;
using AussieCake.Util;
using AussieCake.Verb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AussieCake.Context
{

    public class SqLiteHelper : SqLiteCommands
    {
        private static readonly string InsertSQL = "insert into {0} values(NULL, ";
        private static readonly string UpdateSQL = "update {0} set {1} where Id = {2}";
        private static readonly string RemoveSQL = "delete from {0} where Id = {1}";

        private static readonly string Null = "NULL";

        protected static List<ColVM> Collocations { get; private set; }
        protected static List<AttemptVM> CollocationAttempts { get; private set; }
        protected static List<VerbModel> Verbs { get; private set; }
        protected static List<AttemptVM> EssayAttempts { get; private set; }
        protected static List<AttemptVM> SumRetellAttempts { get; private set; }
        protected static List<AttemptVM> DescImgAttempts { get; private set; }

        public static void InitializeDB()
        {
            if (!CreateIfEmptyDB())
                return;
        }

        #region GetDB

        protected static void GetCollocationsDB()
        {
            var dataset = GetTable(Model.Col);

            Collocations = new List<ColVM>();

            var tables = dataset.Tables[0];
            var enumerable = tables.AsEnumerable();
            var selected = enumerable.Select(GetDatarowCollocations());

            foreach (var model in selected)
                Collocations.Add(model.ToVM());
        }

        protected static void GetColAttemptsDB()
        {
            var dataset = GetTable(GetDBAttemptName(Model.Col));
            CollocationAttempts = dataset.Tables[0].AsEnumerable().Select(GetDatarowAttempts(Model.Col)).ToList();
        }

        protected static void GetEssayAttemptsDB()
        {
            var dataset = GetTable(GetDBAttemptName(Model.Essay));
            EssayAttempts = dataset.Tables[0].AsEnumerable().Select(GetDatarowAttempts(Model.Essay)).ToList();
        }

        protected static void GetSumRetellAttemptsDB()
        {
            var dataset = GetTable(GetDBAttemptName(Model.SumRetell));
            SumRetellAttempts = dataset.Tables[0].AsEnumerable().Select(GetDatarowAttempts(Model.SumRetell)).ToList();
        }

        protected static void GetDescImgAttemptsDB()
        {
            var dataset = GetTable(GetDBAttemptName(Model.DescImg));
            DescImgAttempts = dataset.Tables[0].AsEnumerable().Select(GetDatarowAttempts(Model.DescImg)).ToList();
        }

        protected static void GetVerbsDB()
        {
            var dataset = GetTable(Model.Verb);

            Verbs = dataset.Tables[0].AsEnumerable().Select(GetDatarowVerbs()).ToList();
        }

        #endregion

        #region Inserts

        protected static bool InsertCollocation(ColModel col)
        {
            string query = string.Format(InsertSQL + "'{1}', '{2}', {3}, '{4}', '{5}', {6})",
                                        Model.Col.ToDesc(),
                                        col.Text, col.Answer,
                                        Null, Null, col.Importance, col.IsActive);
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(Model.Col.ToDesc());
            Collocations.Add(inserted.Tables[0].AsEnumerable().
                                Select(GetDatarowCollocations()).First().ToVM());

            return true;
        }

        protected static bool InsertAttempt(AttemptVM att)
        {
            string query = string.Format(InsertSQL + "'{1}', '{2}', '{3}')",
                                         GetDBAttemptName(att.Type),
                                         att.IdQuestion, att.Score, att.When);
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(GetDBAttemptName(att.Type));

            if (att.Type == Model.Col)
            {
                if (CollocationAttempts == null)
                    CollocationAttempts = new List<AttemptVM>();

                CollocationAttempts.Add(inserted.Tables[0].AsEnumerable().
                                        Select(GetDatarowAttempts(att.Type)).First());
            }
            else if (att.Type == Model.Essay)
            {
                if (EssayAttempts == null)
                    EssayAttempts = new List<AttemptVM>();

                EssayAttempts.Add(inserted.Tables[0].AsEnumerable().
                                        Select(GetDatarowAttempts(att.Type)).First());
            }
            else if (att.Type == Model.SumRetell)
            {
                if (SumRetellAttempts == null)
                    SumRetellAttempts = new List<AttemptVM>();

                SumRetellAttempts.Add(inserted.Tables[0].AsEnumerable().
                                        Select(GetDatarowAttempts(att.Type)).First());
            }
            else if (att.Type == Model.DescImg)
            {
                if (DescImgAttempts == null)
                    DescImgAttempts = new List<AttemptVM>();

                DescImgAttempts.Add(inserted.Tables[0].AsEnumerable().
                                        Select(GetDatarowAttempts(att.Type)).First());
            }

            return true;
        }

        protected static bool InsertVerb(VerbModel verb)
        {
            string query = string.Format(InsertSQL + "'{1}', '{2}', '{3}', '{4}', '{5}')",
                                                     Model.Verb.ToDesc(),
                                                     verb.Infinitive, verb.Past, verb.PastParticiple, verb.Person, verb.Gerund);
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(Model.Verb.ToDesc());
            Verbs.Add(inserted.Tables[0].AsEnumerable().
                                Select(GetDatarowVerbs()).First());

            return true;
        }

        #endregion

        #region Updates

        protected static bool UpdateCollocation(ColModel col)
        {
            var oldCol = Collocations.First(c => c.Id == col.Id).ToModel();
            int field = 0;
            string columnsToUpdate = string.Empty;

            CheckFieldUpdate("Words", col.Text, oldCol.Text, ref field, ref columnsToUpdate);
            CheckFieldUpdate("Answer", col.Answer, oldCol.Answer, ref field, ref columnsToUpdate);

            CheckQuestionChanges(col, oldCol, ref field, ref columnsToUpdate);

            if (columnsToUpdate.IsEmpty())
                return Errors.ThrowErrorMsg(ErrorType.NullOrEmpty, columnsToUpdate);

            string query = string.Format(UpdateSQL, Model.Col.ToDesc(), columnsToUpdate, col.Id);

            if (!SendQuery(query))
                return false;

            return true;
        }

        private static void CheckFieldUpdate(string fieldName, object newValue, object oldValue, ref int field, ref string columnsToUpdate)
        {
            if (newValue != oldValue)
            {
                var ToInt = newValue is bool || newValue is Int16;
                columnsToUpdate += field > 0 ? ", " : string.Empty;
                columnsToUpdate += fieldName + " = " + "'" + (ToInt ? Convert.ToInt16(newValue) : newValue) + "'";

                field++;
            }
        }

        private static void CheckQuestionChanges(QuestionModel quest, QuestionModel oldQuest, ref int field, ref string columnsToUpdate)
        {
            CheckFieldUpdate("Definition", quest.Definition, oldQuest.Definition, ref field, ref columnsToUpdate);
            CheckFieldUpdate("PtBr", quest.PtBr, oldQuest.PtBr, ref field, ref columnsToUpdate);
            CheckFieldUpdate("Importance", quest.Importance, oldQuest.Importance, ref field, ref columnsToUpdate);
            CheckFieldUpdate("IsActive", quest.IsActive, oldQuest.IsActive, ref field, ref columnsToUpdate);
        }

        #endregion

        #region Private Members

        private static Func<DataRow, ColModel> GetDatarowCollocations()
        {
            return dataRow => new ColModel(
                                Convert.ToInt16(dataRow.Field<Int64>("Id")),
                                dataRow.Field<string>("Words"),
                                dataRow.Field<string>("Answer"),
                                dataRow.Field<string>("PtBr"),
                                dataRow.Field<string>("Definition"),
                                Convert.ToInt16(dataRow.Field<Int64>("Importance")),
                                Convert.ToInt16(dataRow.Field<Int64>("IsActive")));
        }

        private static Func<DataRow, AttemptVM> GetDatarowAttempts(Model type)
        {
            return dataRow => new AttemptVM(
                        Convert.ToInt16(dataRow.Field<Int64>("Id")),
                        Convert.ToInt16(dataRow.Field<Int64>("Id" + type.ToDesc())),
                        Convert.ToInt16(dataRow.Field<Int64>("Score")),
                        Convert.ToDateTime(dataRow.Field<string>("When")),
                        type
                    );
        }

        private static Func<DataRow, VerbModel> GetDatarowVerbs()
        {
            return dataRow => new VerbModel(
                        dataRow.Field<string>("Infinitive"),
                        dataRow.Field<string>("Past"),
                        dataRow.Field<string>("PP"),
                        dataRow.Field<string>("Gerund"),
                        dataRow.Field<string>("Person")
                    );
        }

        #endregion

        #region Removes

        protected static bool RemoveCollocation(ColVM col)
        {
            string query = string.Format(RemoveSQL, Model.Col.ToDesc(), col.Id);

            if (!SendQuery(query))
                return false;

            Collocations.Remove(col);

            return true;
        }

        protected static bool RemoveQuestionAttempt(AttemptVM att)
        {
            string query = string.Format(RemoveSQL, GetDBAttemptName(att.Type), att.Id);

            if (!SendQuery(query))
                return false;

            CollocationAttempts.Remove(att);

            return true;
        }

        #endregion

        private static bool CreateIfEmptyDB()
        {
            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'Collocation' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'Words' TEXT NOT NULL, " +
                "'Answer' TEXT NOT NULL, " +
                "'Definition' TEXT, " +
                "'PtBr' TEXT, " +
                "'Importance' INTEGER NOT NULL, " +
                "'IsActive' INTEGER NOT NULL )"))
                return false;

            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'CollocationAttempt' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'IdCollocation' INTEGER NOT NULL, " +
                "'Score' INTEGER NOT NULL, " +
                "'When' TEXT NOT NULL, " +
                "FOREIGN KEY(`IdCollocation`) REFERENCES `Collocation`(`Id`) )"))
                return false;

            if (!SendQuery(GetCreatingQueryForTemplate(Model.Essay)))
                return false;

            if (!SendQuery(GetCreatingQueryForTemplate(Model.SumRetell)))
                return false;

            if (!SendQuery(GetCreatingQueryForTemplate(Model.DescImg)))
                return false;

            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'Verb' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'Infinitive' TEXT NOT NULL, " +
                "'Past' TEXT NOT NULL, " +
                "'PP' TEXT NOT NULL, " +
                "'Person' TEXT NOT NULL, " +
                "'Gerund' TEXT NOT NULL )"))
                return false;

            InsertStaticValuesIfEmpty(Model.Verb, CakePaths.ScriptVerbs);
            InsertStaticValuesIfEmpty(Model.Col, CakePaths.ScriptCollocations);

            return true;
        }

        private static void InsertStaticValuesIfEmpty(Model type, string scriptFile)
        {
            if (GetTable(type).Tables[0].Rows.Count == 0)
                SendQuery(ScriptFileCommands.GetStringFromScriptFile(scriptFile));
        }

        private static string GetDBAttemptName(Model type)
        {
            return type.ToDesc() + "Attempt";
        }

        private static string GetCreatingQueryForTemplate(Model type)
        {
            return "CREATE TABLE IF NOT EXISTS '" + type.ToDesc() + "Attempt' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'Id" + type.ToDesc() + "' INTEGER NOT NULL, " +
                "'Score' INTEGER NOT NULL, " +
                "'When' TEXT NOT NULL )";
        }
    }
}