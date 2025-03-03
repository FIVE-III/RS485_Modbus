
using System.Windows;


namespace WpfApp1
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
          
        }

        private void Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
