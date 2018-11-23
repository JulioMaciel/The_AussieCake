﻿using AussieCake.Question;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
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

            LoadRequirements();

            ChalWPFControl.PopulateRows(userControlGrid, Model.Col, lines, WordApp);

            CreateFrame();
        }

        private void LoadRequirements()
        {
            WordApp = new Microsoft.Office.Interop.Word.Application();
            QuestControl.LoadCrossData(Model.Col);
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
