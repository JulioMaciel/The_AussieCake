using AussieCake.Util;
using AussieCake.Util.WPF;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Question
{
    /// <summary>
    /// Interaction logic for Spellabularies.xaml
    /// </summary>
    public partial class Spellings : UserControl
    {
        public Spellings()
        {
            InitializeComponent();

            QuestControl.LoadCrossData(Model.Spell);

            BuildHeader();
        }

        private void BuildHeader()
        {
            var wpf = new SpellWpfHeader();

            var stk_insert = MyStacks.GetHeaderInsertFilter(wpf.Stk_insert, 0, 0, userControlGrid);
            wpf.Stk_items = stk_insert;

            MyGrids.Bulk_Insert(wpf.Grid_bulk_insert, userControlGrid);
            MyTxts.Bulk_Insert(wpf.Txt_bulk_insert, wpf.Grid_bulk_insert);
            var bulk_imp = MyCbBxs.Importance(wpf.Cob_bulk_imp, 0, 1, wpf.Grid_bulk_insert, Importance.My_own_relevant, false);
            bulk_imp.Height = 28;
            MyBtns.Insert_Bulk(wpf.Grid_bulk_insert, wpf);
            MyBtns.Bulk_back(wpf.Grid_bulk_insert, wpf);

            var stk_items = MyStacks.GetListItems(1, 0, userControlGrid);

            var grid_insert = MyGrids.Get(new List<int>() {
                4, 2, 2, 2, 2, 2, 2, 2, 2
            }, 2, stk_insert);
            grid_insert.Margin = new Thickness(2, 0, 2, 0);

            var filter = new SpellFilter();

            MyLbls.Header(wpf.Lbl_words, 0, 0, grid_insert, SortLbl.Words, wpf.Txt_words, filter, stk_items);
            MyLbls.Header(wpf.Lbl_imp, 0, 1, grid_insert, SortLbl.Imp, wpf.Cob_imp, filter, stk_items);
            MyLbls.Header(wpf.Lbl_avg_w, 0, 2, grid_insert, SortLbl.Score_w, wpf.Txt_avg_w, filter, stk_items);
            MyLbls.Header(wpf.Lbl_avg_m, 0, 3, grid_insert, SortLbl.Score_m, wpf.Txt_avg_m, filter, stk_items);
            MyLbls.Header(wpf.Lbl_avg_all, 0, 4, grid_insert, SortLbl.Score_all, wpf.Txt_avg_all, filter, stk_items);
            MyLbls.Header(wpf.Lbl_tries, 0, 5, grid_insert, SortLbl.Tries, wpf.Txt_tries, filter, stk_items);
            MyLbls.Header(wpf.Lbl_chance, 0, 6, grid_insert, SortLbl.Chance, wpf.Txt_chance, filter, stk_items);
            MyBtns.Is_active(wpf.Btn_isActive, 0, 7, grid_insert, true);
            MyBtns.Show_bulk_insert(0, 8, grid_insert, wpf);

            MyTxts.Get(wpf.Txt_words, 1, 0, grid_insert);
            MyCbBxs.Importance(wpf.Cob_imp, 1, 1, grid_insert, Importance.Any, true);
            MyTxts.Get(wpf.Txt_avg_w, 1, 2, grid_insert);
            MyTxts.Get(wpf.Txt_avg_m, 1, 3, grid_insert);
            MyTxts.Get(wpf.Txt_avg_all, 1, 4, grid_insert);
            MyTxts.Get(wpf.Txt_tries, 1, 5, grid_insert);
            MyTxts.Get(wpf.Txt_chance, 1, 6, grid_insert);
            MyBtns.Quest_Filter(wpf.Btn_filter, 1, 7, grid_insert, wpf, filter);
            MyBtns.Quest_Insert(wpf.Btn_insert, 1, 8, grid_insert, stk_items, wpf);

            filter.BuildStack(stk_items);
        }


    }
}
