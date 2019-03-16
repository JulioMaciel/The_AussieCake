using System.Windows.Controls;

namespace AussieCake.Question
{
    public class ColWpfItem : QuestWpfItem
    {
        public TextBox Words { get; set; }
        public TextBox Answer { get; set; }

        public ColWpfItem()
        {
            Words = new TextBox();
            Answer = new TextBox();
        }
    }
}
