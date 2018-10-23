using AussieCake.Question;
using AussieCake.Util;
using System;
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
            string modifiedSenIds = GetSenIdsFromCheckBoxes(wpf_item);

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
                modifiedSenIds.ToListInt(),
                wpf_item.IsActive.IsActived);

            EditQuestion(col, edited, item_line, wpf_item.Add_sen);
        }

        private static string GetSenIdsFromCheckBoxes(ColWpfItem wpf_item)
        {
            string modifiedSenIds = string.Empty;

            foreach (var ch in wpf_item.Stk_sen.Children.OfType<CheckBoxSen>())
                if (ch.IsChecked.Value)
                    modifiedSenIds += ch.SenId + ";";

            if (!modifiedSenIds.IsEmpty())
                modifiedSenIds = modifiedSenIds.Remove(modifiedSenIds.Count(), 1);
            return modifiedSenIds;
        }

        private static void EditQuestion(IQuestion quest, IQuestion edited, StackPanel item_line, TextBox txt_addSen)
        {
            if (!txt_addSen.Text.IsEmpty())
            {
                if (!QuestControl.Update(edited, txt_addSen.Text))
                    return;
            }
            else if (!QuestControl.Update(edited))
                return;

            edited = QuestControl.Get(quest.Type).Where(q => q.Id == quest.Id).First();
            edited.LoadCrossData();

            UpdateWpfItem(item_line, edited);
        }

        private static void AddWpfItem(StackPanel stk_items, IQuestion vm)
        {
            if (vm.Type == Model.Col)
                ColWpfController.AddIntoItems(stk_items, vm as ColVM, true);
        }

        private static void UpdateWpfItem(StackPanel item_line, IQuestion vm)
        {
            item_line.Children.Clear();

            if (vm.Type == Model.Col)
                ColWpfController.AddIntoThis(vm as ColVM, item_line);
        }
    }

}
