using System.Windows.Controls;

namespace AussieCake.Question
{
    public class ColWpfItem : QuestWpfItem
    {
        public TextBox Pref { get; set; }
        public TextBox Comp1 { get; set; }
        public TextBox Link { get; set; }
        public TextBox Comp2 { get; set; }
        public TextBox Suff { get; set; }

        public CheckBox IsComp1_v { get; set; }
        public CheckBox IsComp2_v { get; set; }

        public ColWpfItem()
        {
            Pref = new TextBox();
            Comp1 = new TextBox();
            Link = new TextBox();
            Comp2 = new TextBox();
            Suff = new TextBox();
            IsComp1_v = new CheckBox();
            IsComp2_v = new CheckBox();
        }
    }
}
