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
            var item_line = MyStacks.GetItemLine(stack_items, isNew);
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

            MyTxts.Get(wpf.Pref, 0, 0, row1, col.Prefixes.ToText());
            wpf.Pref.ToolTip = "Id " + col.Id;
            MyTxts.Get(wpf.Comp1, 0, 1, row1, col.Component1);
            MyChBxs.IsVerb(wpf.IsComp1_v, 0, 2, row1, col.IsComp1Verb);
            MyTxts.Get(wpf.Link, 0, 3, row1, col.LinkWords.ToText());
            MyTxts.Get(wpf.Comp2, 0, 4, row1, col.Component2);
            MyChBxs.IsVerb(wpf.IsComp2_v, 0, 5, row1, col.IsComp2Verb);
            MyTxts.Get(wpf.Suff, 0, 6, row1, col.Suffixes.ToText());
            MyCbBxs.GetImportance(wpf.Imp, 0, 7, row1, col.Importance, false);

            MyLbls.AvgScore(wpf.Avg_w, 0, 0, row2, col, 7);
            MyLbls.AvgScore(wpf.Avg_m, 0, 1, row2, col, 30);
            MyLbls.AvgScore(wpf.Avg_all, 0, 2, row2, col, 2000);
            MyLbls.Tries(wpf.Tries, 0, 3, row2, col);
            MyLbls.LastTry(wpf.Last_try, 0, 4, row2, col);
            MyLbls.Chance(wpf.Chance, 0, 5, row2, col);

            MyBtns.Show_sentences(wpf.Show_sen, 0, 6, row2, col, wpf.Stk_sen);
            MyBtns.Is_active(wpf.IsActive, 0, 7, row2, col.IsActive);
            MyBtns.PtBr(wpf.Show_ptbr, 0, 8, row2, col.PtBr, wpf.Ptbr);
            MyBtns.Definition(wpf.Show_def, 0, 9, row2, col.Definition, wpf.Def);
            MyBtns.Quest_Edit(wpf.Edit, 0, 10, row2, col, wpf, item_line);
            MyBtns.Remove_quest(wpf.Remove, 0, 11, row2, col, item_line);

            MyTxts.Definition(wpf.Def, col.Definition, row3);
            MyTxts.PtBr(wpf.Ptbr, col.PtBr, row3);
            MyStacks.Sentences(wpf.Stk_sen, col, row3);

            MyTxts.Add_sentence(wpf.Add_sen, wpf.Stk_sen);
        }
    }
}
