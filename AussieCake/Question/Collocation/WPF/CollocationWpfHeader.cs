using System.Windows.Controls;

namespace AussieCake.Question
{
    public class ColWpfHeader : QuestWpfHeader
    {
        public TextBox Txt_pref { get; set; }
        public TextBox Txt_comp1 { get; set; }
        public TextBox Txt_link { get; set; }
        public TextBox Txt_comp2 { get; set; }
        public TextBox Txt_suff { get; set; }

        public Label Lbl_pref { get; set; }
        public Label Lbl_comp1 { get; set; }
        public Label Lbl_link { get; set; }
        public Label Lbl_comp2 { get; set; }
        public Label Lbl_suff { get; set; }

        public CheckBox Chb_isComp1_v { get; set; }
        public CheckBox Chb_isComp2_v { get; set; }

        public ColWpfHeader()
        {
            Init();

            Txt_pref = new TextBox();
            Txt_comp1 = new TextBox();
            Txt_link = new TextBox();
            Txt_comp2 = new TextBox();
            Txt_suff = new TextBox();
            Lbl_pref = new Label();
            Lbl_comp1 = new Label();
            Lbl_link = new Label();
            Lbl_comp2 = new Label();
            Lbl_suff = new Label();
            Chb_isComp1_v = new CheckBox();
            Chb_isComp2_v = new CheckBox();
        }
    }
}
