using AussieCake.Question;
using AussieCake.Sentence;
using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using static AussieCake.Util.WPF.MyChBxs;

namespace AussieCake
{
    public static class QuestWpfUtil
    {
        public static void InsertColClick(StackPanel stk_items, ColWpfHeader wpf_header)
        {
            if (wpf_header.Cob_imp.SelectedIndex == Convert.ToInt16(Importance.Any))
            {
                Errors.ThrowErrorMsg(ErrorType.InvalidImportanceAny, wpf_header.Cob_imp.SelectedIndex);
                return;
            }

            var col = new ColVM(wpf_header.Txt_pref.Text.ToListString(),
                wpf_header.Txt_comp1.Text,
                wpf_header.Chb_isComp1_v.IsChecked.Value,
                wpf_header.Txt_link.Text.ToListString(),
                wpf_header.Txt_comp2.Text,
                wpf_header.Chb_isComp2_v.IsChecked.Value,
                wpf_header.Txt_suff.Text.ToListString(),
                wpf_header.Txt_def.Text,
                wpf_header.Txt_ptbr.Text,
                (Importance)wpf_header.Cob_imp.SelectedIndex,
                wpf_header.Btn_isActive.IsActived);

            if (QuestControl.Insert(col))
            {
                wpf_header.Txt_pref.Text = string.Empty;
                wpf_header.Txt_comp1.Text = string.Empty;
                wpf_header.Chb_isComp1_v.IsChecked = false;
                wpf_header.Txt_link.Text = string.Empty;
                wpf_header.Txt_comp2.Text = string.Empty;
                wpf_header.Chb_isComp2_v.IsChecked = false;
                wpf_header.Txt_suff.Text = string.Empty;
                wpf_header.Txt_def.Text = string.Empty;
                wpf_header.Txt_ptbr.Text = string.Empty;
                wpf_header.Cob_imp.SelectedIndex = (int)Importance.Any;

                var added = QuestControl.Get(Model.Col).Last();
                added.LoadCrossData();

                AddWpfItem(stk_items, added);
            }
        }

        public static void EditColClick(ColVM col, ColWpfItem wpf_item, StackPanel item_line)
        {
            var modifiedSenIds = GetSenFromCbBoxes(wpf_item);

            var edited = new ColVM(col.Id,
                wpf_item.Pref.Text.ToListString(),
                wpf_item.Comp1.Text,
                wpf_item.IsComp1_v.IsChecked.Value,
                wpf_item.Link.Text.ToListString(),
                wpf_item.Comp2.Text,
                wpf_item.IsComp2_v.IsChecked.Value,
                wpf_item.Suff.Text.ToListString(),
                wpf_item.Def.Text,
                wpf_item.Ptbr.Text,
                (Importance)(wpf_item.Imp).SelectedValue,
                wpf_item.IsActive.IsActived);

            EditQuestion(col, edited, item_line, wpf_item.Add_sen, modifiedSenIds);
        }

        private static List<QuestSen> GetSenFromCbBoxes(ColWpfItem wpf_item)
        {
            var modifiedSenIds = new List<QuestSen>();

            List<CheckBoxSen> cbs = new List<CheckBoxSen>();
            foreach (var stk in wpf_item.Stk_sen.Children.OfType<StackPanel>())
                cbs.Add(stk.Children.OfType<CheckBoxSen>().First());

            List<ButtonActive> bts = new List<ButtonActive>();
            foreach (var stk in wpf_item.Stk_sen.Children.OfType<StackPanel>())
                bts.Add(stk.Children.OfType<ButtonActive>().First());

            for (int i = 0; i < cbs.Count(); i++)
            {
                var cb = cbs.ElementAt(i);

                if (!cb.IsChecked.Value)
                    continue;

                var bt = bts.ElementAt(i);
                //var sen = SenControl.Get().First(x => x.Id == cb.SenId);

                modifiedSenIds.Add(new QuestSen(cb.QS.Sen, bt.IsActived, cb.QS.QS_id));
            }

            return modifiedSenIds;
        }

        private static void EditQuestion(IQuest quest, IQuest edited, StackPanel item_line, TextBox txt_addSen, List<QuestSen> qss)
        {
            if (!txt_addSen.Text.IsEmpty())
            {
                if (!QuestControl.Update(edited, txt_addSen.Text))
                    return;
            }
            else if (!QuestControl.Update(edited))
                return;

            var intersect = quest.Sentences.Intersect(qss);
            foreach (var qs in intersect)
            {
                var vm = QuestSenControl.Get(edited.Type).First(x => x.Id == qs.QS_id);
                QuestSenControl.Remove(vm);
            }

            foreach (var qs in qss)
            {
                var db = QuestSenControl.Get(edited.Type).First(x => x.Id == qs.QS_id);
                var old = db.IsActive;
                var modified = qs.IsActive;

                if (old != modified)
                {
                    db.IsActive = modified;
                    QuestSenControl.Update(db);
                }
            }

            edited = QuestControl.Get(quest.Type).Where(q => q.Id == quest.Id).First();
            edited.LoadCrossData();

            UpdateWpfItem(item_line, edited);
        }

        private static void AddWpfItem(StackPanel stk_items, IQuest vm)
        {
            if (vm.Type == Model.Col)
                ColWpfController.AddIntoItems(stk_items, vm as ColVM, true);
        }

        private static void UpdateWpfItem(StackPanel item_line, IQuest vm)
        {
            item_line.Children.Clear();

            if (vm.Type == Model.Col)
                ColWpfController.AddIntoThis(vm as ColVM, item_line);
        }
    }

}
