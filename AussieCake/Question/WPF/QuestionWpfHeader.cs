﻿using AussieCake.Util.WPF;
using System.Windows.Controls;

namespace AussieCake.Question
{
public class QuestWpfHeader
    {
        public StackPanel Stk_items { get; set; }

        public TextBox Txt_avg_w { get; set; }
        public TextBox Txt_avg_m { get; set; }
        public TextBox Txt_avg_all { get; set; }
        public TextBox Txt_tries { get; set; }
        public TextBox Txt_sen { get; set; }
        public TextBox Txt_chance { get; set; }
        public TextBox Txt_def { get; set; }
        public TextBox Txt_ptbr { get; set; }

        public Label Lbl_avg_w { get; set; }
        public Label Lbl_avg_m { get; set; }
        public Label Lbl_avg_all { get; set; }
        public Label Lbl_tries { get; set; }
        public Label Lbl_sen { get; set; }
        public Label Lbl_chance { get; set; }
        public Label Lbl_imp { get; set; }
        public Label Lbl_ptBr { get; set; }
        public Label Lbl_def { get; set; }

        public ComboBox Cob_imp { get; set; }
        public ButtonActive Btn_isActive { get; set; }
        public Button Btn_insert { get; set; }
        public Button Btn_filter { get; set; }

        protected void Init()
        {
            Stk_items = new StackPanel();
            Txt_avg_w = new TextBox();
            Txt_avg_m = new TextBox();
            Txt_avg_all = new TextBox();
            Txt_tries = new TextBox();
            Txt_sen = new TextBox();
            Txt_chance = new TextBox();
            Txt_def = new TextBox();
            Txt_ptbr = new TextBox();
            Lbl_avg_w = new Label();
            Lbl_avg_m = new Label();
            Lbl_avg_all = new Label();
            Lbl_tries = new Label();
            Lbl_sen = new Label();
            Lbl_chance = new Label();
            Lbl_imp = new Label();
            Lbl_ptBr = new Label();
            Lbl_def = new Label();
            Cob_imp = new ComboBox();
            Btn_isActive = new ButtonActive();
            Btn_insert = new Button();
            Btn_filter = new Button();

        }
    }
}