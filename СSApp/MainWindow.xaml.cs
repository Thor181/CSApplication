using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace СSApp
{
    public partial class MainWindow : Window
    {
        private bool AutoScroll = true;

        public MainWindow()
        {
            InitializeComponent();
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