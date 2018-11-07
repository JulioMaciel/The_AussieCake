﻿using AussieCake.Challenge;
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
        public static Label AvgScore(Label reference, int row, int column, Grid parent, IQuest quest, int duration, bool useColours = true)
        {
            var isWeek = duration <= 7;
            var isMonth = duration <= 30;

            var avg = isWeek ? quest.Avg_week : (isMonth ? quest.Avg_month : quest.Avg_all);
            var content = avg + "% " + (isWeek ? "(w)" : isMonth ? "(m)" : ""); 

            var lbl = Get(reference, row, column, parent, content);

            if (useColours)
                lbl.Foreground = UtilWPF.GetAvgColor(avg);

            lbl.ToolTip = isWeek ? "week" : isMonth ? "month" : "all";

            return lbl;
        }

        public static Label Tries(Label reference, int row, int column, Grid parent, IQuest quest)
        {
            var content = quest.Tries.Count > 0 ? quest.Tries.Count + " tries" : "New";

            return Get(reference, row, column, parent, content);
        }

        public static Label LastTry(Label reference, int row, int column, Grid parent, IQuest quest)
        {
            var content = string.Empty;
            if (quest.Tries.Any())
            {
                var last = DateTime.Now.Subtract(quest.LastTry.When).Days;
                content = last != 0 ? last + "d ago" : "Today";
            }
            else
                content = "Never";

            return Get(reference, row, column, parent, content);
        }

        public static Label Chance(Label reference, int row, int column, Grid parent, IQuest quest)
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

            lbl.MouseLeftButtonDown += (source, e) => filter.SetSort(sort, stk_items);

            if (sort == SortLbl.Col_comp1 || sort == SortLbl.Col_comp2)
                Grid.SetColumnSpan(lbl, 2);

            return lbl;
        }

        public static Label Get(Label reference, int row, int column, Grid parent, string content)
        {
            var lbl = Get(reference, content);
            UtilWPF.SetGridPosition(reference, row, column, parent);

            return reference;
        }

        public static Label Get(Label reference, StackPanel parent, string content)
        {
            var lbl = Get(reference, content);

            if (!parent.Children.Contains(lbl))
                parent.Children.Add(lbl);

            return lbl;
        }

        public static Label Get(Label reference, string content)
        {
            reference.Content = content;
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.HorizontalContentAlignment = HorizontalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);

            return reference;
        }

        public static Label Get(int row, int column, Grid parent, string content)
        {
            return Get(new Label(), row, column, parent, content);
        }

        public static Label Chal_answer(Label reference, StackPanel parent, string content)
        {
            var lbl = Get(reference, parent, content);
            return lbl;
        }

        public static Label Chal_id(bool isQuest, ChalLine line)
        {
            var reference = isQuest ? line.Chal.Id_col : line.Chal.Id_sen;
            var id = isQuest ? line.Quest.Id : line.QS.Sen.Id;
            var content = id + (isQuest ? " (quest)" : " (sen)");
            var column = isQuest ? 3 : 1;
            Get(reference, 0, column, line.Chal.Row_4, content);
            reference.ToolTip = "Right click to copy the Id";
            reference.MouseRightButtonDown += (source, e) => Clipboard.SetText(id.ToString());

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

    [Description("Prefixes [Id]")]
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
