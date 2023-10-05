using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace СSApp
{
    public partial class MainWindow : Window
    {
        private bool AutoScroll = true;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                File.WriteAllText("Error.txt", $"{e.Message} | {e.StackTrace}");
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange == 0)
                if (ScrollViewer.VerticalOffset == ScrollViewer.ScrollableHeight)
                    AutoScroll = true;
                else
                    AutoScroll = false;

            if (AutoScroll && e.ExtentHeightChange != 0)
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.ExtentHeight);
        }
    }
}