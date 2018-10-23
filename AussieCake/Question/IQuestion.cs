using AussieCake.Util;
using System.Collections.Generic;

namespace AussieCake.Question
{
    public interface IQuestion
    {
        int Id { get; }

        bool IsActive { get; }
        string PtBr { get; }
        string Definition { get; }
        Importance Importance { get; }
        List<int> SentencesId { get; }

        Model Type { get; }

        int Corrects { get; }
        int Incorrects { get; }
        double Avg_week { get; }
        double Avg_month { get; }
        double Avg_all { get; }
        List<DateTry> Tries { get; }
        DateTry LastTry { get; }
        double Chance { get; }
        double Chance_real { get; set; }
        string Chance_toolTip { get; }

        void RemoveAllAttempts();
        void AddLastSentence();

        void LoadCrossData();
    }
}
