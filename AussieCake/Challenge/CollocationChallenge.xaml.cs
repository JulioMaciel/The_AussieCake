using AussieCake.Sentence;
using AussieCake.Util;
using AussieCake.Util.WPF;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Challenge
{
    /// <summary>
    /// Interaction logic for CollocationChallenge.xaml
    /// </summary>
    public partial class ColChallenge : UserControl
    {
        List<ChalLine> lines = new List<ChalLine>(4);
        Microsoft.Office.Interop.Word.Application WordApp;

        public ColChallenge()
        {
            InitializeComponent();

            if (!SenControl.Get().Any() || SenControl.Get().Count() <= 4)
            {
                Errors.ThrowErrorMsg(ErrorType.NullOrEmpty, "There's no enough sentences to generate the challenge.");
                return;
            }

            WordApp = new Microsoft.Office.Interop.Word.Application();

            ChalWPFControl.PopulateRows(userControlGrid, Model.Col, lines, WordApp);

            CreateFrame();
        }

        private void CreateFrame()
        {
            var stk_btns = MyStacks.Get(new StackPanel(), 4, 0, userControlGrid);
            stk_btns.HorizontalAlignment = HorizontalAlignment.Right;

            var btn_next = new Button();
            var btn_verify = new Button();

            MyBtns.Chal_verify(btn_verify, stk_btns, btn_next);
            btn_verify.Click += (source, e) => lines.ForEach(x => ChalWPFControl.Verify(x, btn_verify, btn_next));

            MyBtns.Chal_next(btn_next, stk_btns, btn_verify, userControlGrid, lines, Model.Col, WordApp);
        }

        
    }
}
