using AussieCake.Util;
using System.Collections.Generic;

namespace AussieCake.Question
{
    public interface IQuest
    {
        int Id { get; }

        bool IsActive { get; }
        string PtBr { get; }
        string Definition { get; }
        Importance Importance { get; }

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

        double Index_show { get; set; }

        void RemoveAllAttempts();

        void LoadCrossData();
        void Disable();

        string ToText();
        string ToLudwigUrl();
    }
}
