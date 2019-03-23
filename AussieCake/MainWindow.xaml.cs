using AussieCake.Challenge;
using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Templates;
using AussieCake.Util;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AussieCake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SqLiteHelper.InitializeDB();

            ShowWelcomeCake();
        }

        private void ShowWelcomeCake()
        {
            var stk = new StackPanel();
            stk.HorizontalAlignment = HorizontalAlignment.Center;
            stk.VerticalAlignment = VerticalAlignment.Center;

            var welcome_cake = new Image();
            welcome_cake.Source = new BitmapImage(new Uri(CakePaths.WelcomeCake));
            welcome_cake.Height = 256;
            welcome_cake.Width = 256;
            stk.Children.Add(welcome_cake);

            var lbl_welcome = new TextBlock();
            lbl_welcome.HorizontalAlignment = HorizontalAlignment.Center;
            lbl_welcome.FontSize = 30;
            lbl_welcome.Margin = new Thickness(0, 10, 0, 0);
            lbl_welcome.Text = "The cake is a lie!";
            stk.Children.Add(lbl_welcome);

            frame_content.Content = stk;
        }

        private void btnCollocations_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new Collocations();
            btnCollocations.IsEnabled = false;
        }

        private void btnColChallenge_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new ColChallenge();
            btnColChallenge.IsEnabled = false;
        }

        private void EnableButtons()
        {
            btnCollocations.IsEnabled = true;
            btnColChallenge.IsEnabled = true;
            btnEssay.IsEnabled = true;
            btnEssayByTopic.IsEnabled = true;
            btnSumRetell.IsEnabled = true;
            btnDescImg.IsEnabled = true;
        }

        private void btnEssay_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new Essay();
            btnEssay.IsEnabled = false;
        }

        private void BtnEssayByTopic_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new ByTopic();
            btnEssayByTopic.IsEnabled = false;
        }

        private void btnSumRetell_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new SumRetell();
            btnSumRetell.IsEnabled = false;
        }

        private void btnDescImg_Click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new DescImg();
            btnDescImg.IsEnabled = false;
        }
    }
}
