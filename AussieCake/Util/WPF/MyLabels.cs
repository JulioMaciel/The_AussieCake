using AussieCake.Question;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AussieCake.Util.WPF
{
    public static class MyLbls
    {
        private static Label GetAvgScore(Label reference, int row, int column, Grid parent, IQuestion quest, int duration)
        {
            var isWeek = duration <= 7;
            var isMonth = duration <= 30;

            var avg = isWeek ? quest.Avg_week : (isMonth ? quest.Avg_month : quest.Avg_all);
            var content = avg + "% " + (isWeek ? "(w)" : isMonth ? "(m)" : "");
            var foreground = UtilWPF.GetAvgColor(avg);

            var lbl = Get(reference, row, column, parent, content);
            lbl.Foreground = foreground;
            lbl.ToolTip = isWeek ? "week" : isMonth ? "month" : "all";

            return lbl;
        }

        public static Label GetAvgWeekScore(Label reference, int row, int column, Grid parent, IQuestion quest)
        {
            return GetAvgScore(reference, row, column, parent, quest, 7);
        }

        public static Label GetAvgMonthScore(Label reference, int row, int column, Grid parent, IQuestion quest)
        {
            return GetAvgScore(reference, row, column, parent, quest, 30);
        }

        public static Label GetAvgAllScore(Label reference, int row, int column, Grid parent, IQuestion quest)
        {
            return GetAvgScore(reference, row, column, parent, quest, int.MaxValue);
        }

        public static Label GetTries(Label reference, int row, int column, Grid parent, IQuestion quest)
        {
            var content = quest.Tries.Count > 0 ? quest.Tries.Count + " tries" : "New";

            return Get(reference, row, column, parent, content);
        }

        public static Label GetLastTry(Label reference, int row, int column, Grid parent, IQuestion quest)
        {
            var content = quest.Tries.Any() ? DateTime.Now.Subtract(quest.LastTry.When).Days + "d ago" : "Never";

            return Get(reference, row, column, parent, content);
        }

        public static Label GetChance(Label reference, int row, int column, Grid parent, IQuestion quest)
        {
            var lbl = Get(reference, row, column, parent, quest.Chance + " (" + Math.Round(quest.Chance_real, 2) + "%)");
            lbl.ToolTip = quest.Chance_toolTip;

            lbl.MouseEnter += new MouseEventHandler((source, e) => lbl.Foreground = Brushes.DarkRed);
            lbl.MouseLeave += new MouseEventHandler((source, e) => lbl.Foreground = Brushes.Black);

            return lbl;
        }

        public static Label GetHeader(Label reference, int row, int column, Grid parent, SortLbl sort, Control control, IFilter filter, StackPanel stk_items)
        {
            var lbl = Get(reference, row, column, parent, sort.ToDesc());

            lbl.MouseEnter += new MouseEventHandler((source, e) => lbl.Foreground = Brushes.DarkRed);
            lbl.MouseLeave += new MouseEventHandler((source, e) => lbl.Foreground = Brushes.Black);

            lbl.ToolTip = "Left click to sort";

            if (control is TextBox)
            {
                lbl.MouseRightButtonDown += (source, e) => (control as TextBox).Text = string.Empty;
                lbl.ToolTip += "; Right to erase; Double [txt] to paste.";
            }

            lbl.MouseDoubleClick += (source, e) => filter.SetSort(sort, stk_items);

            if (sort == SortLbl.Col_comp1 || sort == SortLbl.Col_comp2)
                Grid.SetColumnSpan(lbl, 2);

            return lbl;
        }

        public static Label Get(Label reference, int row, int column, Grid parent, string content)
        {
            reference.Content = content;
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.HorizontalContentAlignment = HorizontalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);

            UtilWPF.SetGridPosition(reference, row, column, parent);

            return reference;
        }
    }
}

public enum SortLbl
{
    Id,

    [Description("(w) Score")]
    Score_w,

    [Description("(m) Score")]
    Score_m,

    [Description("Score")]
    Score_all,

    [Description("Tries")]
    Tries,

    [Description("Sentences")]
    Sen,

    [Description("% Show")]
    Chance,

    [Description("Importance")]
    Imp,

    [Description("Definition")]
    Def,

    [Description("PtBr")]
    PtBr,

    [Description("Prefixes")]
    Col_pref,

    [Description("Component 1")]
    Col_comp1,

    [Description("Link")]
    Col_link,

    [Description("Component 2")]
    Col_comp2,

    [Description("Suffixes")]
    Col_suf,
}
