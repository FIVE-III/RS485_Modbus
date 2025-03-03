using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
      
        public MainWindow()
        {
            InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
        }



        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{

        //    MessageBoxResult result = MessageBox.Show("确定要关闭吗？", "关闭", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (result == MessageBoxResult.No)
        //    {
        //        e.Cancel = true; // 取消关闭操作
        //    }
        //    else
        //    {
        //        // 调用你自定义的关闭命令
        //        var viewModel = this.DataContext as MainViewModel;
        //        if (viewModel != null && viewModel.DisposWindow.CanExecute(null))
        //        {
        //            viewModel.DisposWindow.Execute(null);
        //        }
        //    }
        //}
        //}
    }
    }
    
