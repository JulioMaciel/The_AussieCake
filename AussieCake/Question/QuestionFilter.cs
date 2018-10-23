using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AussieCake.Question
{
    public abstract class QuestionFilter
    {
        private SortLbl Sort { get; set; }
        protected bool IsNextSortAsc { get; set; }

        protected IEnumerable<IQuestion> Original_quests { get; set; }
        protected IEnumerable<IQuestion> Filtered_quests { get; set; }

        protected QuestionFilter(IEnumerable<IQuestion> original)
        {
            Original_quests = original;
            Filtered_quests = original;

            Sort = SortLbl.Id;
            IsNextSortAsc = false;
        }

        protected void SetSort(SortLbl sort)
        {
            if (sort == Sort)
                IsNextSortAsc = !IsNextSortAsc;
            else
            {
                IsNextSortAsc = false;
                Sort = sort;
            }

            switch (sort)
            {
                case SortLbl.Score_w:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Avg_week).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Avg_week).ToList();
                    break;
                case SortLbl.Score_m:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Avg_month).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Avg_month).ToList();
                    break;
                case SortLbl.Score_all:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Avg_all).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Avg_all).ToList();
                    break;
                case SortLbl.Tries:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Tries.Count()).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Tries.Count()).ToList();
                    break;
                case SortLbl.Sen:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.SentencesId.Count()).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.SentencesId.Count()).ToList();
                    break;
                case SortLbl.Chance:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Chance).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Chance).ToList();
                    break;
                case SortLbl.Imp:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => (int)q.Importance * -1).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => (int)q.Importance * -1).ToList();
                    break;
                case SortLbl.Def:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Definition).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Definition).ToList();
                    break;
                case SortLbl.PtBr:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.PtBr).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.PtBr).ToList();
                    break;
                default:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.OrderBy(q => q.Id).ToList();
                    else
                        Filtered_quests = Filtered_quests.OrderByDescending(q => q.Id).ToList();
                    break;
            }
        }

        //protected virtual void OnFilter()
        //{
        //}

        protected void Filter(QuestWpfHeader wpf_header)
        {
            Filtered_quests = Original_quests;

            if (wpf_header.Cob_imp.SelectedIndex != Convert.ToInt16(Importance.Any))
                Filtered_quests = Filtered_quests.Where(q => q.Importance == (Importance)wpf_header.Cob_imp.SelectedIndex).ToList();

            if (!wpf_header.Txt_avg_w.Text.IsEmpty())
            {
                if (Errors.IsNotDigitsOnly(wpf_header.Txt_avg_w.Text))
                    return;

                Filtered_quests = Filtered_quests.Where(q => q.Avg_week >= Convert.ToInt16(wpf_header.Txt_avg_w.Text)).ToList();
            }
            if (!wpf_header.Txt_avg_m.Text.IsEmpty())
            {
                if (Errors.IsNotDigitsOnly(wpf_header.Txt_avg_m.Text))
                    return;

                Filtered_quests = Filtered_quests.Where(q => q.Avg_month >= Convert.ToInt16(wpf_header.Txt_avg_m.Text)).ToList();
            }
            if (!wpf_header.Txt_avg_all.Text.IsEmpty())
            {
                if (Errors.IsNotDigitsOnly(wpf_header.Txt_avg_all.Text))
                    return;

                Filtered_quests = Filtered_quests.Where(q => q.Avg_all >= Convert.ToInt16(wpf_header.Txt_avg_all.Text)).ToList();
            }
            if (!wpf_header.Txt_tries.Text.IsEmpty())
            {
                if (Errors.IsNotDigitsOnly(wpf_header.Txt_tries.Text))
                    return;

                Filtered_quests = Filtered_quests.Where(q => q.Tries.Count >= Convert.ToInt16(wpf_header.Txt_tries.Text)).ToList();
            }
            if (!wpf_header.Txt_sen.Text.IsEmpty())
            {
                if (Errors.IsNotDigitsOnly(wpf_header.Txt_sen.Text))
                    return;

                Filtered_quests = Filtered_quests.Where(q => q.SentencesId.Count >= Convert.ToInt16(wpf_header.Txt_sen.Text)).ToList();
            }
            if (!wpf_header.Txt_chance.Text.IsEmpty())
            {
                if (Errors.IsNotDigitsOnly(wpf_header.Txt_chance.Text))
                    return;

                Filtered_quests = Filtered_quests.Where(q => q.Chance >= Convert.ToInt16(wpf_header.Txt_chance.Text)).ToList();
            }

            if (!wpf_header.Txt_def.Text.IsEmpty())
                Filtered_quests = Filtered_quests.Where(q => q.Definition.Contains(wpf_header.Txt_def.Text)).ToList();

            if (!wpf_header.Txt_ptbr.Text.IsEmpty())
                Filtered_quests = Filtered_quests.Where(q => q.PtBr.Contains(wpf_header.Txt_ptbr.Text)).ToList();

            Filtered_quests = Filtered_quests.Where(q => q.IsActive == wpf_header.Btn_isActive.IsActived).ToList();
            
            (wpf_header.Stk_items.Parent as ScrollViewer).ScrollToTop();
        }

        //protected void BuildStack(StackPanel stk_items)
        //{
        //    stk_items.Children.Clear();
        //}
    }
}
