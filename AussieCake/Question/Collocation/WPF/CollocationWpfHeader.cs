using System.Windows.Controls;

namespace AussieCake.Question
{
    public class ColWpfHeader : QuestWpfHeader
    {
        public TextBox Txt_words { get; set; }
        public TextBox Txt_answer { get; set; }

        public Label Lbl_words { get; set; }
        public Label Lbl_answer { get; set; }

        public ColWpfHeader()
        {
            Init();

            Txt_words = new TextBox();
            Txt_answer = new TextBox();
            Lbl_words = new Label();
            Lbl_answer = new Label();
        }
    }
}
