
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// AddressPort.xaml 的交互逻辑
    /// </summary>
    public partial class AddressPort : Window
    {
        
        public AddressPort()
        {
            InitializeComponent();
            DataContext=new AddressWindowVM();
            //var windowService = new WindowService();
            //DataContext = new MainViewModel(windowService);
        }
        }
    
}
