using AussieCake.Util;
using AussieCake.Util.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AussieCake.Templates
{
    /// <summary>
    /// Interaction logic for Essay.xaml
    /// </summary>
    public partial class Essay : UserControl
    {
        List<CellTemplate> CellList;

        double percentageTxt = 100;

        public Essay()
        {
            InitializeComponent();

            slider.Value = 80;

            CellList = TemplateWPF.BuildTemplate(TemplateEssay.Words, 0, StkTemplate);
            //HighlightEssayType();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            btnStart.Content = "Start " + e.NewValue + "%";
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            int lastDays = 99;

            if (!txtFilter.Text.IsEmpty() && txtFilter.Text.IsDigitsOnly() && txtFilter.Text != "0")
                lastDays = Convert.ToInt32(txtFilter.Text);

            CellList = TemplateWPF.BuildTemplate(TemplateEssay.Words, 0, StkTemplate);
            TemplateWPF.ShowScoredTemplate(CellList, lastDays);

            btnFinish.IsEnabled = false;
            lblScore.Visibility = Visibility.Hidden;
        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            lblScore.Visibility = Visibility.Visible;
            var result = TemplateWPF.CheckAnswers(CellList);
            lblScore.Content = result.Item1;
            lblScore.Foreground = UtilWPF.GetAvgColor(result.Item2);

            btnFinish.IsEnabled = false;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (cb_33.IsChecked.Value)
                percentageTxt = 33;
            else
                percentageTxt = 100 - slider.Value;

            CellList = TemplateWPF.BuildTemplate(TemplateEssay.Words, percentageTxt, StkTemplate);
            //HighlightEssayType();

            btnFinish.IsEnabled = true;
            lblScore.Visibility = Visibility.Hidden;
        }

        private void Cb_33_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(((CheckBox)sender).IsChecked.Value) == true)
            {
                slider.IsEnabled = false;
                btnStart.Content = "Start 33%";
            }
            else
            {
                slider.IsEnabled = true;
                percentageTxt = 100;
                btnStart.Content = "Start " + slider.Value + "%";
            }

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.Content = "Start 33%";
        }

        //private void HighlightEssayType()
        //{
        //    foreach (var quest in CellList)
        //    {
        //        if (TemplateEssay.ProblemSolutionExclusives.Any(x => x == quest.Quest.Id))
        //        {
        //            var Vocour = Brushes.LightSkyBlue;
        //            quest.Lbl.Background = Vocour;
        //            if (quest.Txt != null)
        //                quest.Txt.Background = Vocour;
        //        }
        //        else if (TemplateEssay.OneTwoTopicsExclusives.Any(x => x == quest.Quest.Id))
        //        {
        //            var Vocour = Brushes.Moccasin;
        //            quest.Lbl.Background = Vocour;
        //            if (quest.Txt != null)
        //                quest.Txt.Background = Vocour;
        //        }
        //        else if (TemplateEssay.GeneralOptionalExclusives.Any(x => x == quest.Quest.Id))
        //        {
        //            var Vocour = Brushes.Silver;
        //            quest.Lbl.Background = Vocour;
        //            if (quest.Txt != null)
        //                quest.Txt.Background = Vocour;
        //        }
        //    }
        //}
    }
}
