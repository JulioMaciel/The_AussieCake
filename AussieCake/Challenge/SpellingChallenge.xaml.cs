using AussieCake.Question;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AussieCake.Challenge
{
    /// <summary>
    /// Interaction logic for SpellChallenge.xaml
    /// </summary>
    public partial class SpellChallenge : UserControl
    {
        List<ChalLine> lines = new List<ChalLine>(4);

        public SpellChallenge()
        {
            InitializeComponent();

            LoadRequirements();

            ChalWPFControl.PopulateRows(userControlGrid, Model.Spell, lines);

            CreateFrame();
        }

        private void LoadRequirements()
        {
            QuestControl.LoadCrossData(Model.Spell);
        }

        private void CreateFrame()
        {
            var stk_btns = MyStacks.Get(new StackPanel(), 4, 0, userControlGrid);
            stk_btns.HorizontalAlignment = HorizontalAlignment.Right;

            var btn_next = new Button();
            var btn_verify = new Button();

            MyBtns.Chal_verify(btn_verify, stk_btns, btn_next);
            btn_verify.Click += (source, e) => lines.ForEach(x => ChalWPFControl.Verify(x, btn_verify, btn_next));

            MyBtns.Chal_next(btn_next, stk_btns, btn_verify, userControlGrid, lines, Model.Spell);
        }


    }
}
