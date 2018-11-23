using AussieCake.Question;
using AussieCake.Util;
using System;
using System.Linq;
using System.Windows.Controls;

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

            Footer.Log("The question has been inserted.");
        }

        public static void EditColClick(ColVM col, ColWpfItem wpf_item, StackPanel item_line)
        {
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

            EditQuestion(col, edited, item_line);
        }

        private static void EditQuestion(IQuest quest, IQuest edited, StackPanel item_line)
        {
            if (!QuestControl.Update(edited))
                return;

            edited = QuestControl.Get(quest.Type).Where(q => q.Id == quest.Id).First();
            edited.LoadCrossData();

            UpdateWpfItem(item_line, edited);

            Footer.Log("The question has been edited.");
        }

        public static void AddWpfItem(StackPanel stk_items, IQuest vm)
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
