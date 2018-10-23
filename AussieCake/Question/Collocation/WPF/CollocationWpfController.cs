using AussieCake.Util;
using AussieCake.Util.WPF;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Question
{
    public static class ColWpfController
    {
        public static void AddIntoItems(StackPanel stack_items, ColVM col, bool isNew)
        {
            var item_line = MyStacks.GetItemLine(col, stack_items, isNew);
            AddIntoThis(col, item_line);
        }

        public static void AddIntoThis(ColVM col, StackPanel item_line)
        {
            var row1 = MyGrids.GetRowItem(new List<int>() {
                1, 2, 1, 1, 2, 1, 1, 2
            }, item_line);

            var row2 = MyGrids.GetRowItem(new List<int>() {
                2, 3, 2, 2, 2, 3, 2, 1, 1, 1, 1, 1
            }, item_line);

            var row3 = new StackPanel();
            row3.Margin = new Thickness(1, 2, 1, 2);
            item_line.Children.Add(row3);

            var wpf = new ColWpfItem();

             MyTxts.Get(wpf.Pref,0, 0, row1, col.Prefixes.ToText());
             MyTxts.Get(wpf.Comp1,0, 1, row1, col.Component1);
             MyChBxs.GetIsVerb(wpf.IsComp1_v,0, 2, row1, col.IsComp1Verb);
             MyTxts.Get(wpf.Link,0, 3, row1, col.LinkWords.ToText());
             MyTxts.Get(wpf.Comp2,0, 4, row1, col.Component2);
             MyChBxs.GetIsVerb(wpf.IsComp2_v,0, 5, row1, col.IsComp2Verb); 
             MyTxts.Get(wpf.Suff,0, 6, row1, col.Suffixes.ToText());
             MyCbBxs.GetImportance(wpf.Imp,0, 7, row1, col.Importance, false);

             MyLbls.GetAvgWeekScore(wpf.Avg_w,0, 0, row2, col);
             MyLbls.GetAvgMonthScore(wpf.Avg_m,0, 1, row2, col);
             MyLbls.GetAvgAllScore(wpf.Avg_all,0, 2, row2, col);
             MyLbls.GetTries(wpf.Tries,0, 3, row2, col);
             MyLbls.GetLastTry(wpf.Last_try,0, 4, row2, col);
             MyLbls.GetChance(wpf.Chance,0, 5, row2, col);

             MyBtns.GetSentences(wpf.Show_sen,0, 6, row2, col, wpf.Stk_sen);
             MyBtns.GetIsActive(wpf.IsActive,0, 7, row2, col.IsActive);
             MyBtns.GetPtBr(wpf.Show_ptbr,0, 8, row2, col.PtBr, wpf.Ptbr);
             MyBtns.GetDefinition(wpf.Show_def,0, 9, row2, col.Definition, wpf.Def);
             MyBtns.GetEdit(wpf.Edit,0, 10, row2, col, wpf, item_line);
             MyBtns.GetRemoveQuestion(wpf.Remove,0, 11, row2, col, item_line);

             MyTxts.GetDefinition(wpf.Def,col, row3);
             MyTxts.GetPtBr(wpf.Ptbr,col, row3);
             MyStacks.GetSentences(wpf.Stk_sen,col, row3);

             MyTxts.GetAddSentence(wpf.Add_sen,wpf.Stk_sen);
        }
    }
}
