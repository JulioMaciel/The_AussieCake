using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace AussieCake.Question
{
    public class ColFilter : QuestionFilter, IFilter
    {
        public ColFilter() : base (QuestControl.Get(Model.Col))
        {

        }

        public void SetSort(SortLbl sort, StackPanel stk_items)
        {
            base.SetSort(sort);

            switch (sort)
            {
                case SortLbl.Col_pref:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.Prefixes.FirstOrDefault());
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.Prefixes.ToText());
                    break;
                case SortLbl.Col_comp1:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.Component1);
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.Component1);
                    break;
                case SortLbl.Col_link:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.LinkWords.FirstOrDefault());
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.LinkWords.ToText());
                    break;
                case SortLbl.Col_comp2:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.Component2);
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.Component2);
                    break;
                case SortLbl.Col_suf:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.Suffixes.FirstOrDefault());
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.Suffixes.ToText());
                    break;
            }

            BuildStack(stk_items);
        }

        public void Filter(ColWpfHeader wpf_header)
        {
            base.Filter(wpf_header);

            var filtered_cols = Filtered_quests.Cast<ColVM>();

            if (!wpf_header.Txt_pref.IsEmpty())
            {
                if (wpf_header.Txt_pref.Text.IsDigitsOnly() && filtered_cols.Any(x => x.Id == Convert.ToInt16(wpf_header.Txt_pref.Text)))
                    filtered_cols = filtered_cols.Where(q => q.Id == Convert.ToInt16(wpf_header.Txt_pref.Text));
                else
                    filtered_cols = filtered_cols.Where(q => q.Prefixes.ToText().Contains(wpf_header.Txt_pref.Text));
            }

            if (!wpf_header.Txt_comp1.IsEmpty())
                filtered_cols = filtered_cols.Where(q => q.Component1.Contains(wpf_header.Txt_comp1.Text));

            if (!wpf_header.Txt_link.IsEmpty())
                filtered_cols = filtered_cols.Where(q => q.LinkWords.ToText().Contains(wpf_header.Txt_link.Text));

            if (!wpf_header.Txt_comp2.IsEmpty())
                filtered_cols = filtered_cols.Where(q => q.Component2.Contains(wpf_header.Txt_comp2.Text));

            if (!wpf_header.Txt_suff.IsEmpty())
                filtered_cols = filtered_cols.Where(q => q.Suffixes.ToText().Contains(wpf_header.Txt_suff.Text));

            Filtered_quests = filtered_cols;

            BuildStack(wpf_header.Stk_items);
        }

        public void BuildStack(StackPanel stk_items)
        {
            var watcher = new Stopwatch();
            watcher.Start();

            stk_items.Children.Clear();

            foreach (ColVM col in Filtered_quests.Take(30))
                ColWpfController.AddIntoItems(stk_items, col, false);

            Footer.Log("Showing " + Filtered_quests.Take(30).Count() + " collocations of a total of " + Filtered_quests.Count() + 
                       ". Loaded in " + watcher.Elapsed.TotalSeconds + " seconds.");
        }

    }
}
