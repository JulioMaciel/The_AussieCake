using AussieCake.Attempt;
using AussieCake.Question;
using AussieCake.Sentence;
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
        protected static List<QuestSenVM> CollocationSentences { get; private set; }
        protected static List<SenVM> Sentences { get; private set; }
        protected static List<VerbModel> Verbs { get; private set; }

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

        protected static void GetSentencesDB()
        {
            var dataset = GetTable(Model.Sen);

            Sentences = dataset.Tables[0].AsEnumerable().Select(GetDatarowSentences()).ToList();
        }

        protected static void GetColAttemptsDB()
        {
            var dataset = GetTable(GetDBAttemptName(Model.Col));
            CollocationAttempts = dataset.Tables[0].AsEnumerable().Select(GetDatarowAttempts(Model.Col)).ToList();
        }

        protected static void GetColSentencesDB()
        {
            var dataset = GetTable(GetDBSentenceName(Model.Col));
            CollocationSentences = dataset.Tables[0].AsEnumerable().Select(GetDatarowQuestSentences(Model.Col)).ToList();
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
            string query = string.Format(InsertSQL + "'{1}', '{2}', {3}, '{4}', '{5}', {6}, '{7}', {8}, {9}, {10}, {11})",
                                        Model.Col.ToDesc(),
                                        col.Prefixes, col.Component1, col.IsComp1Verb, col.LinkWords,
                                        col.Component2, col.IsComp2Verb, col.Suffixes,
                                        Null, Null, col.Importance, col.IsActive);
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(Model.Col.ToDesc());
            Collocations.Add(inserted.Tables[0].AsEnumerable().
                                Select(GetDatarowCollocations()).First().ToVM());

            return true;
        }

        protected static bool InsertSentence(SenVM sen)
        {
            string query = string.Format(InsertSQL + "'{1}', '{2}')",
                                         Model.Sen.ToDesc(),
                                         sen.Text.Replace("\'", "\'\'"), sen.PtBr);
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(Model.Sen.ToDesc());
            Sentences.Add(inserted.Tables[0].AsEnumerable().
                                Select(GetDatarowSentences()).First());

            return true;
        }

        protected static bool InsertQuestionAttempt(AttemptVM att)
        {
            string query = string.Format(InsertSQL + "'{1}', '{2}', '{3}')",
                                         GetDBAttemptName(att.Type),
                                         att.IdQuestion, att.Score, att.When);
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(GetDBAttemptName(att.Type));

            if (CollocationAttempts == null)
                CollocationAttempts = new List<AttemptVM>();

            CollocationAttempts.Add(inserted.Tables[0].AsEnumerable().
                                    Select(GetDatarowAttempts(att.Type)).First());

            return true;
        }

        protected static bool InsertQuestionSentence(QuestSenVM vm)
        {
            string query = string.Format(InsertSQL + "'{1}', '{2}', '{3}')",
                                         GetDBSentenceName(vm.Type),
                                         vm.IdQuest, vm.IdSen, vm.IsActive.ToInt());
            if (!SendQuery(query))
                return false;

            var inserted = GetLast(GetDBSentenceName(vm.Type));
            var inserted_vm = inserted.Tables[0].AsEnumerable().
                                    Select(GetDatarowQuestSentences(vm.Type)).First();

            if (CollocationSentences == null)
                CollocationSentences = new List<QuestSenVM>();

            if (vm.Type == Model.Col)
                CollocationSentences.Add(inserted_vm);

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

            CheckFieldUpdate("Prefixes", col.Prefixes, oldCol.Prefixes, ref field, ref columnsToUpdate);
            CheckFieldUpdate("Component1", col.Component1, oldCol.Component1, ref field, ref columnsToUpdate);
            CheckFieldUpdate("IsComp1Verb", col.IsComp1Verb, oldCol.IsComp1Verb, ref field, ref columnsToUpdate);
            CheckFieldUpdate("LinkWords", col.LinkWords, oldCol.LinkWords, ref field, ref columnsToUpdate);
            CheckFieldUpdate("Component2", col.Component2, oldCol.Component2, ref field, ref columnsToUpdate);
            CheckFieldUpdate("IsComp2Verb", col.IsComp2Verb, oldCol.IsComp2Verb, ref field, ref columnsToUpdate);
            CheckFieldUpdate("Suffixes", col.Suffixes, oldCol.Suffixes, ref field, ref columnsToUpdate);

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

                //if (ToInt)
                //    columnsToUpdate += fieldName + " = " + Convert.ToInt16(newValue);
                //else
                //    columnsToUpdate += fieldName + " = " + "'" + newValue + "'";

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

        protected static bool UpdateSentence(SenVM sen)
        {
            var oldSen = Sentences.First(c => c.Id == sen.Id);
            int field = 0;
            string columnsToUpdate = string.Empty;

            CheckFieldUpdate("Text", sen.Text, oldSen.Text, ref field, ref columnsToUpdate);
            CheckFieldUpdate("PtBr", sen.PtBr, oldSen.PtBr, ref field, ref columnsToUpdate);

            if (columnsToUpdate.IsEmpty())
                return Errors.ThrowErrorMsg(ErrorType.NullOrEmpty, columnsToUpdate);

            string query = string.Format(UpdateSQL, Model.Sen.ToDesc(), columnsToUpdate, sen.Id);

            if (!SendQuery(query))
                return false;

            return true;
        }

        protected static bool UpdateQuestSentence(QuestSenVM vm)
        {
            var old = new QuestSenVM();

            if (vm.Type == Model.Col)
                old = CollocationSentences.First(c => c.Id == vm.Id);

            int field = 0;
            string columnsToUpdate = string.Empty;

            CheckFieldUpdate("IsActive", vm.IsActive, old.IsActive, ref field, ref columnsToUpdate);

            if (columnsToUpdate.IsEmpty())
                return Errors.ThrowErrorMsg(ErrorType.NullOrEmpty, columnsToUpdate);

            string query = string.Format(UpdateSQL, GetDBSentenceName(vm.Type), columnsToUpdate, vm.Id);

            if (!SendQuery(query))
                return false;

            return true;
        }

        #endregion

        #region Private Members

        private static Func<DataRow, ColModel> GetDatarowCollocations()
        {
            return dataRow => new ColModel(
                                Convert.ToInt16(dataRow.Field<Int64>("Id")),
                                dataRow.Field<string>("Prefixes"),
                                dataRow.Field<string>("Component1"),
                                Convert.ToInt16(dataRow.Field<Int64>("IsComp1Verb")),
                                dataRow.Field<string>("LinkWords"),
                                dataRow.Field<string>("Component2"),
                                Convert.ToInt16(dataRow.Field<Int64>("IsComp2Verb")),
                                dataRow.Field<string>("Suffixes"),
                                dataRow.Field<string>("PtBr"),
                                dataRow.Field<string>("Definition"),
                                Convert.ToInt16(dataRow.Field<Int64>("Importance")),
                                Convert.ToInt16(dataRow.Field<Int64>("IsActive")));
        }

        private static Func<DataRow, SenVM> GetDatarowSentences()
        {
            return dataRow => new SenVM(
                                Convert.ToInt16(dataRow.Field<Int64>("Id")),
                                dataRow.Field<string>("Text"),
                                dataRow.Field<string>("PtBr")
                              );
        }

        private static Func<DataRow, AttemptVM> GetDatarowAttempts(Model type)
        {
            return dataRow => new AttemptVM(
                        Convert.ToInt16(dataRow.Field<Int64>("Id")),
                        Convert.ToInt16(dataRow.Field<Int64>("IdCollocation")),
                        Convert.ToInt16(dataRow.Field<Int64>("Score")),
                        Convert.ToDateTime(dataRow.Field<string>("When")),
                        type
                    );
        }

        private static Func<DataRow, QuestSenVM> GetDatarowQuestSentences(Model type)
        {
            return dataRow => new QuestSenVM(
                        Convert.ToInt16(dataRow.Field<Int64>("Id")),
                        Convert.ToInt16(dataRow.Field<Int64>("IdCollocation")),
                        Convert.ToInt16(dataRow.Field<Int64>("IdSentence")),
                        Convert.ToBoolean(Convert.ToInt16(dataRow.Field<Int64>("IsActive"))),
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

        protected static bool RemoveSentence(SenVM sen)
        {
            string query = string.Format(RemoveSQL, Model.Sen.ToDesc(), sen.Id);

            if (!SendQuery(query))
                return false;

            Sentences.Remove(sen);

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

        protected static bool RemoveQuestionSentence(QuestSenVM vm)
        {
            string query = string.Format(RemoveSQL, GetDBSentenceName(vm.Type), vm.Id);

            if (!SendQuery(query))
                return false;

            if (vm.Type == Model.Col)
                CollocationSentences.Remove(vm);

            return true;
        }

        #endregion

        private static bool CreateIfEmptyDB()
        {
            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'Collocation' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'Prefixes' TEXT, " +
                "'Component1' TEXT NOT NULL, " +
                "'IsComp1Verb' INTEGER NOT NULL, " +
                "'LinkWords' TEXT, " +
                "'Component2' TEXT NOT NULL, " +
                "'IsComp2Verb' INTEGER NOT NULL, " +
                "'Suffixes' TEXT, " +
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

            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'Sentence' " +
                "( 'Id' INTEGER NOT NULL CONSTRAINT " +
                "'PK_Sentence' PRIMARY KEY AUTOINCREMENT, " +
                "'Text' TEXT NOT NULL, " +
                "'PtBr' TEXT NULL )"))
                return false;

            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'Verb' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'Infinitive' TEXT NOT NULL, " +
                "'Past' TEXT NOT NULL, " +
                "'PP' TEXT NOT NULL, " +
                "'Person' TEXT NOT NULL, " +
                "'Gerund' TEXT NOT NULL )"))
                return false;

            if (!SendQuery("CREATE TABLE IF NOT EXISTS 'CollocationSentence' " +
                "( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                "'IdCollocation' INTEGER NOT NULL, " +
                "'IdSentence' INTEGER NOT NULL, " +
                "'IsActive' INTEGER NOT NULL, " +
                "FOREIGN KEY('IdSentence') REFERENCES 'Sentence'('Id'), " +
                "FOREIGN KEY('IdCollocation') REFERENCES 'Collocation'('Id') ) "))
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

        private static string GetDBSentenceName(Model type)
        {
            return type.ToDesc() + "Sentence";
        }
    }
}