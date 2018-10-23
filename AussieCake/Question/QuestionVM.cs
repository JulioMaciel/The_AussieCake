using AussieCake.Attempt;
using AussieCake.Sentence;
using AussieCake.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Question
{
    public abstract class QuestVM : IQuestion
    {
        public int Id { get; protected set; }

        public bool IsActive { get; protected set; }
        public string PtBr { get; protected set; }
        public string Definition { get; protected set; }
        public Importance Importance { get; protected set; }
        public List<int> SentencesId { get; protected set; }

        public Model Type { get; protected set; }

        public List<DateTry> Tries { get; set; }
        public int Corrects { get; protected set; }
        public int Incorrects { get; protected set; }
        public double Avg_week { get; protected set; }
        public double Avg_month { get; protected set; }
        public double Avg_all { get; protected set; }
        public DateTry LastTry { get; set; }
        public double Chance { get; protected set; }
        public double Chance_real { get; set; }
        public string Chance_toolTip { get; protected set; }

        protected bool IsReal { get; set; } = false;

        protected QuestVM(int id, string definition, string ptBr, Importance importance, List<int> sentencesId, bool isActive, Model type)
            : this(definition, ptBr, importance, isActive, type)
        {
            Id = id;
            SentencesId = sentencesId;

            IsReal = true;
        }

        protected QuestVM(string definition, string ptBr, Importance importance, bool isActive, Model type)
        {
            Definition = definition;
            PtBr = ptBr;
            Importance = importance;
            IsActive = isActive;

            Type = type;
        }

        public virtual void LoadCrossData()
        {
            Tries = new List<DateTry>();

            var attempts = AttemptsControl.Get(Type).Where(x => x.IdQuestion == Id);

            foreach (var item in attempts)
                Tries.Add(new DateTry(item.Score, item.When));

            LastTry = Tries.Any() ? Tries.Last() : null;

            Avg_week = GetAverageScoreByTime(7);
            Avg_month = GetAverageScoreByTime(30);
            Avg_all = GetAverageScoreByTime(int.MaxValue);

            LoadChanceToAppear();
        }

        public void AddLastSentence()
        {
            if (SentencesId == null)
                SentencesId = new List<int>();

            SentencesId.Add(SenControl.Get().Last().Id);
        }

        private double GetAverageScoreByTime(int lastDays)
        {
            if (!Tries.Any())
                return 0;
            else
                //return Math.Round((double)(100 * 
                //    Tries.Where(x => x.IsCorrect && x.When <= (DateTime.Now.AddDays(-lastDays))).ToList().Count / 
                //                                                                                        (Tries.Count)));
                return Tries.Where(x => x.When <= (DateTime.Now.AddDays(-lastDays))).Average(x => x.Score);
        }

        private List<SenVM> GetSentences()
        {
            var sentences = new List<SenVM>();
            foreach (var senId in SentencesId)
            {
                int senIdInt = Convert.ToInt16(senId);
                sentences.Add(SenControl.Get().First(x => x.Id == senIdInt));
            }

            return sentences;
        }

        public void RemoveAllAttempts()
        {
            foreach (var att in AttemptsControl.Get(Type).Where(q => q.IdQuestion == Id))
                AttemptsControl.Remove(att);
        }

        private void LoadChanceToAppear()
        {
            // peso 2
            var daysSince = LastTry != null ? DateTime.Now.Subtract(LastTry.When).Days : 100;
            var lastTry_score = 0;
            if (daysSince < 20 && daysSince >= 1)
                lastTry_score = daysSince / 2;
            else if (daysSince == 0)
            {
                Chance = 1;
                Chance_toolTip = "Question already completed today.";
                return;
            }
            else
                lastTry_score = 20;

            // peso 4
            var inv_avg_week = Avg_week - 100 * -0.2;
            var inv_avg_month = Avg_month - 100 * -0.15;
            var inv_avg_all = Avg_all - 100 * -0.05;
            var inv_avg = daysSince <= 7 ? (inv_avg_week + inv_avg_month + inv_avg_all) :
                        (daysSince <= 30 ? (Avg_month + Avg_all) * 2 : Avg_all - 100 * -0.4);

            // peso 1
            var lastWasWrong = Tries.Any() ? (LastTry.Score == 100 ? 0 : 10) : 10;

            // peso 3
            var imp_score = ScoreHelper.GetScoreFromImportance(Importance) * 3;

            Chance = lastTry_score + inv_avg_week + inv_avg_month + inv_avg_all + lastWasWrong + imp_score;

            Chance_toolTip = inv_avg + " (inv_avg) -> " + (daysSince <= 7 ? "avg_week (20%) + avg_month (15%) + avg_all (5%)" :
                (daysSince <= 30 ? "avg_month (30%) + avg_all (10%)" :
                "avg_all (40%) + ")) + "\n";
            Chance_toolTip += lastTry_score + " (lastTry) + " + lastWasWrong + " (lastWrong) + " + imp_score + " (imp)";
        }
    }

    public class DateTry
    {
        public int Score { get; set; }
        public DateTime When { get; set; }

        public DateTry(int score, DateTime when)
        {
            Score = score;
            When = when;
        }
    }
}
