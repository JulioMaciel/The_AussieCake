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
                case SortLbl.Words:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.Text.FirstOrDefault());
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.Text);
                    break;
                case SortLbl.Answer:
                    if (IsNextSortAsc)
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderBy(q => q.Answer);
                    else
                        Filtered_quests = Filtered_quests.Cast<ColVM>().OrderByDescending(q => q.Answer);
                    break;
            }

            BuildStack(stk_items);
        }

        public void Filter(ColWpfHeader wpf_header)
        {
            base.Filter(wpf_header);

            var filtered_cols = Filtered_quests.Cast<ColVM>();

            if (!wpf_header.Txt_words.IsEmpty())
            {
                if (wpf_header.Txt_words.Text.IsDigitsOnly() && filtered_cols.Any(x => x.Id == Convert.ToInt16(wpf_header.Txt_words.Text)))
                    filtered_cols = filtered_cols.Where(q => q.Id == Convert.ToInt16(wpf_header.Txt_words.Text));
                else
                    filtered_cols = filtered_cols.Where(q => q.Text.Contains(wpf_header.Txt_words.Text));
            }

            if (!wpf_header.Txt_answer.IsEmpty())
                filtered_cols = filtered_cols.Where(q => q.Answer.Contains(wpf_header.Txt_answer.Text));

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
