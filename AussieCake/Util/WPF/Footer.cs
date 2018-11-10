using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AussieCake.Util
{
    public static class Footer
    {
        private static TextBlock LabelFooter = ((MainWindow)Application.Current.MainWindow).lblFooter;
        private static Button BtnFooter = ((MainWindow)Application.Current.MainWindow).btnShowDetails;

        public static void Log(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LabelFooter.Foreground = Brushes.Black;
                LabelFooter.Text = msg;
            });
        }

        public static void LogError(string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BtnFooter.Visibility = Visibility.Collapsed;

                LabelFooter.Foreground = Brushes.DarkRed;
                LabelFooter.Text = msg;
            });
        }

        public static void LogError(string msg, object details)
        {
            LogError(msg);
            Application.Current.Dispatcher.Invoke(() =>
            {
                BtnFooter.CleanClickEvents();
                BtnFooter.Visibility = Visibility.Visible;

                BtnFooter.Click += (e,sender) => MessageBox.Show(details.ToString(), "Error Details");
            });
        }
    }
}