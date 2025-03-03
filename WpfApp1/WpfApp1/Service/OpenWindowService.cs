
using WpfApp1.CildWindow;

namespace WpfApp1.Service
{
    public class OpenWindowService : IOpenWindowService
    {

        //创建并打开窗体
        public void OpenAboutWindow()
        {
            var window = new About();
            window.ShowDialog();
        }
        public void OpenAddressWindow()
        {
            var window = new AddressPort();
            window.ShowDialog();
        }
    }
}
