﻿using AussieCake.Challenge;
using AussieCake.Context;
using AussieCake.Question;
using AussieCake.Sentence;
using System.Windows;

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
        }

        private void btnSentences_click(object sender, RoutedEventArgs e)
        {
            EnableButtons();
            frame_content.Content = new Sentences();
            btnSentences.IsEnabled = false;
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
            btnSentences.IsEnabled = true;
            btnCollocations.IsEnabled = true;
            btnColChallenge.IsEnabled = true;
        }
    }
}
