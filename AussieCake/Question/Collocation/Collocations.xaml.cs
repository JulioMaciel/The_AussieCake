﻿using AussieCake.Sentence;
using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Question
{
    /// <summary>
    /// Interaction logic for Collocations.xaml
    /// </summary>
    public partial class Collocations : UserControl
    {
        public Collocations()
        {
            InitializeComponent();

            QuestControl.LoadCrossData(Model.Col);

            BuildHeader();
        }

        private void BuildHeader()
        {
            var stk_insert = MyStacks.GetHeaderInsertFilter(0, 0, userControlGrid);

            var stk_items = MyStacks.GetListItems(1, 0, userControlGrid);

            var grid_insert = MyGrids.Get(new List<int>() {
                3, 3, 2, 3, 3, 2, 3, 3, 3
            }, 2, stk_insert);
            grid_insert.Margin = new Thickness(2, 0, 2, 0);

            var grid_filter = MyGrids.Get(new List<int>() {
                3, 3, 3, 3, 3, 3, 3, 3, 2, 3
            }, 2, stk_insert);
            grid_filter.Margin = new Thickness(2, 0, 2, 0);

            var wpf = new ColWpfHeader();
            wpf.Stk_items = stk_items;

            var filter = new ColFilter();

            MyLbls.GetHeader(wpf.Lbl_pref, 0, 0, grid_insert, SortLbl.Col_pref, wpf.Txt_pref, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_comp1, 0, 1, grid_insert, SortLbl.Col_comp1, wpf.Txt_comp1, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_link, 0, 3, grid_insert, SortLbl.Col_link, wpf.Txt_link, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_comp2, 0, 4, grid_insert, SortLbl.Col_comp2, wpf.Txt_comp2, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_suff, 0, 6, grid_insert, SortLbl.Col_suf, wpf.Txt_suff, filter, stk_items);

            MyTxts.Get(wpf.Txt_pref, 1, 0, grid_insert);
            MyTxts.Get(wpf.Txt_comp1, 1, 1, grid_insert);
            MyTxts.Get(wpf.Txt_link, 1, 3, grid_insert);
            MyTxts.Get(wpf.Txt_comp2, 1, 4, grid_insert);
            MyTxts.Get(wpf.Txt_suff, 1, 6, grid_insert);

            MyLbls.GetHeader(wpf.Lbl_avg_w, 0, 0, grid_filter, SortLbl.Score_w, wpf.Txt_avg_w, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_avg_m, 0, 1, grid_filter, SortLbl.Score_m, wpf.Txt_avg_m, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_avg_all, 0, 2, grid_filter, SortLbl.Score_all, wpf.Txt_avg_all, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_tries, 0, 3, grid_filter, SortLbl.Tries, wpf.Txt_tries, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_sen, 0, 4, grid_filter, SortLbl.Sen, wpf.Txt_sen, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_chance, 0, 5, grid_filter, SortLbl.Chance, wpf.Txt_chance, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_imp, 0, 7, grid_insert, SortLbl.Imp, wpf.Cob_imp, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_ptBr, 0, 7, grid_filter, SortLbl.PtBr, wpf.Txt_ptbr, filter, stk_items);
            MyLbls.GetHeader(wpf.Lbl_def, 0, 6, grid_filter, SortLbl.Def, wpf.Txt_def, filter, stk_items);

            MyTxts.Get(wpf.Txt_avg_w, 1, 0, grid_filter);
            MyTxts.Get(wpf.Txt_avg_m, 1, 1, grid_filter);
            MyTxts.Get(wpf.Txt_avg_all, 1, 2, grid_filter);
            MyTxts.Get(wpf.Txt_tries, 1, 3, grid_filter);
            MyTxts.Get(wpf.Txt_sen, 1, 4, grid_filter);
            MyTxts.Get(wpf.Txt_chance, 1, 5, grid_filter);
            MyTxts.Get(wpf.Txt_def, 1, 6, grid_filter);
            MyTxts.Get(wpf.Txt_ptbr, 1, 7, grid_filter);

            MyChBxs.IsVerb(wpf.Chb_isComp1_v, 1, 2, grid_insert, false);
            MyChBxs.IsVerb(wpf.Chb_isComp2_v, 1, 5, grid_insert, false);
            MyCbBxs.GetImportance(wpf.Cob_imp, 1, 7, grid_insert, Importance.Any, true);
            MyBtns.Is_active(wpf.Btn_isActive, 1, 8, grid_filter, true);
            MyBtns.GetFilter(wpf.Btn_filter, 1, 9, grid_filter, wpf, filter);

            MyBtns.GetInsert(wpf.Btn_insert, 1, 8, grid_insert, stk_items, wpf);

            filter.BuildStack(stk_items);
        }


    }
}
