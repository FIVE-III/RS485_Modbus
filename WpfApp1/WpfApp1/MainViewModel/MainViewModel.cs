using System;
using System.Windows;
using System.Windows.Input;
using WpfApp1.ChildVM;
using WpfApp1.CildWindow;
using WpfApp1.Service;


namespace WpfApp1
{
    public class MainViewModel : ViewModelBase
    {
        private ConnecterPortService connecterPortService;
        private IWindowService _windowService;
        private IOpenWindowService _openWindowService;
     
        private string _labeladdress;
        private string _labelPortState;
        private string _btnopenport;
        private string _textLog;
        private bool _isbtnEnable;
        private string _btntimedataOpenOrClose;
        private bool _isbtnEnableTime = false;

        public string Labeladdress
        {
            get { return _labeladdress; }
            set
            {
                _labeladdress = value;
                RaisePropertyChanged(() => Labeladdress);
            }
        }
        public string LabelPortState
        {
            get { return _labelPortState; }
            set
            {
                _labelPortState = value;
                RaisePropertyChanged(() => LabelPortState);
            }
        }
        public string Btnopenport
        {
            get { return _btnopenport; }
            set
            {
                _btnopenport = value;
                RaisePropertyChanged("Btnopenport");
            }
        }
        public string TextLog
        {
            get { return _textLog; }
            set
            {
                _textLog = value;
                RaisePropertyChanged(() => TextLog);
            }
        }
        public bool IsbtnEnable
        {
            get { return _isbtnEnable; }
            set
            {
                _isbtnEnable = value;
                RaisePropertyChanged(() => IsbtnEnable);
            }
        }
        public bool IsbtnEnableTime
        {
            get { return _isbtnEnableTime; }
            set
            {
                _isbtnEnableTime = value;
                RaisePropertyChanged(() => IsbtnEnableTime);
            }
        }
        public string BtntimedataOpenOrClose
        {
            get { return _btntimedataOpenOrClose; }
            set
            {
                _btntimedataOpenOrClose = value;
                RaisePropertyChanged(() => BtntimedataOpenOrClose);
            }
        }
        /// <summary>
        /// 按钮是否可用
        /// </summary>
        /// <returns></returns>
        bool ACanExecute()
        {
            return true;
        }





        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="manager"></param>
        public MainViewModel(ConnecterPortService manager)
        {
            _windowService = new WindowService();
            _openWindowService = new OpenWindowService();
            connecterPortService = manager;
            connecterPortService.SetMainViewModel(this);


            SeePortState();
            Labeladdress = $"流量计地址：{GetAddressData()}";
            Btnopenport = "打开串口";
            BtntimedataOpenOrClose = "打开定时读取";
            IsbtnEnable = true;

            //CurrentTime = DateTime.Now;
        }




        /// <summary>
        /// 打开子窗体命令
        /// </summary>
        public ICommand OnPenAddressWindCommand
        {
            get
            {
                return new RelayCommand(OnPenAddressWind, ACanExecute);
            }
        }
        public ICommand OpenPortWindCommand
        {
            get
            {
                return new RelayCommand(OpenPortWind, ACanExecute);
            }
        }
        public ICommand OpenAboutwindowCommand
        {
            get
            {
                return new RelayCommand(OpenAboutWind, ACanExecute);
            }
        }
        public ICommand OpenReadSetTimesWindCommand
        {
            get
            {
                return new RelayCommand(OpenReadSetTimesWind, ACanExecute);
            }
        }


        /// <summary>
        /// 传递给子窗体的服务
        /// </summary>
        public ConnecterPortService ConncetinManager
        {
            get { return connecterPortService; }
            set
            {
                connecterPortService = value;
                RaisePropertyChanged(() => ConncetinManager);
            }
        }



        /// <summary>
        /// 打开子窗体方法
        /// </summary>
        public void OpenAboutWind()
        {
            _openWindowService.OpenAboutWindow();
        }
        public void OpenPortWind()
        {
            var Portwind = new CildWindowVM(this);
            Port portWindow = new Port();
            portWindow.DataContext = Portwind; 
            portWindow.Show();
            IsbtnEnable = false;
        }
        public void OpenReadSetTimesWind()
        {
            if (connecterPortService.IsConnected)
            {
                if (!_isbtnEnableTime)
                {
                    var readTimeVM = new ReadTimeVM(ConncetinManager, this);
                    ReadSetTimes readSetTimes = new ReadSetTimes();
                    readSetTimes.DataContext = readTimeVM;
                    readSetTimes.Show();
                }
                else
                {
                    connecterPortService.DisConnectTimer();
                    BtntimedataOpenOrClose = "打开定时读取";
                    TextLog += $"【已关闭定时读取】\n";
                    IsbtnEnableTime = false;
                }
            }
            else
            {
                TextLog += $"请先打开串口！！！\n";
            }
        }
        public void OnPenAddressWind()
        {
            _openWindowService.OpenAddressWindow();
        }


        /// <summary>
        /// 获取串口地址方法
        /// </summary>
        /// <returns></returns>
        public string GetAddressData()
        {
            return _windowService.AddressId();
        }
        /// <summary>
        /// 显示串口状态方法
        /// </summary>
        public void SeePortState()
        {
            if(connecterPortService.IsConnected)
            {
                LabelPortState = $"串口状态：{connecterPortService.Portnum}";
            }
            else
            {
                LabelPortState = $"串口状态：已关闭";
            }
        }


        /// <summary>
        /// 打开串口命令、方法
        /// </summary>
        public ICommand Openpoat
        {
            get
            {
                return new RelayCommand(OpenPort, ACanExecute);
            }
          
        }
        public void OpenPort()
        {
           
            try
            {
                if (connecterPortService.IsConnected)
                {
                    connecterPortService.ClosePort();
                    IsbtnEnable = true;
                    Btnopenport = "打开串口";
                    SeePortState();
                    Labeladdress = $"流量计地址：{connecterPortService.GetAddress}";
                }
                else
                {
                    connecterPortService.OpenPort();
                    if (connecterPortService.IsConnected)
                    {
                        IsbtnEnable = false;
                        Btnopenport = "关闭串口";
                        SeePortState();
                        Labeladdress = $"流量计地址：{connecterPortService.GetAddress}";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"这是main打开失败：{ex.Message}");
                IsbtnEnable = true;
            }
         
        }



        /// <summary>
        /// 获取实时数据命令
        /// </summary>
        public ICommand commandConnect
        {
            get
            {
                return new RelayCommand(Connect, ACanExecute);
            }
        }
        /// <summary>
        /// 获取实时数据方法
        /// </summary>
        private  void Connect()
        {
            try
            {
                if (connecterPortService.IsConnected)
                {
                    string[] data =connecterPortService.ConnectGetportData();
                    if (data != null)
                    {
                        TextLog += $"数据是：";
                        foreach (var item in data)
                        {
                            TextLog += $"{item}\n";
                        }
                    }
                }
                else
                {
                    TextLog += $"串口未打开！！！\n";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"读取数据失败：{ex.Message}");
            }
        }



        public ICommand CommandReadData
        {
            get
            {
                return new RelayCommand(ReadDatafunc, ACanExecute);
            }
        }
        /// <summary>
        /// 读取全部设备
        /// </summary>
        private void ReadDatafunc()
        {
            if (connecterPortService.IsConnected)
            {
                connecterPortService.ReadNowData();
            }
            else
            {
                TextLog += $"串口未打开！！！\n";
            }
        }




        /// <summary>
        /// 关闭系统的方法和命令
        /// </summary>
        public ICommand DisposWindow
        {
            get
            {
                return new RelayCommand(DisposWindows, ACanExecute);
            }
        }
        private void DisposWindows()
        {
            MessageBoxResult boxResult = System.Windows.MessageBox.Show($"是否要退出本系统？", "信息提示", MessageBoxButton.YesNo);
            try
            {
                if (boxResult == MessageBoxResult.Yes)
                {
                    if (connecterPortService.IsConnected)
                    {
                        connecterPortService.ClosePort();
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
                return;
             
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}");
            }
        }





        public ICommand SaveLogfile
        {
            get
            {
                return new RelayCommand(SavefileLogFunc, ACanExecute);
            }
        }
        /// <summary>
        /// 保存日志文件方法
        /// </summary>
        private void SavefileLogFunc()
        {
            connecterPortService.SaveFileDialog();
        }


        public ICommand DeleteLogfile
        {
            get
            {
                return new RelayCommand(DeletelogfileFunc, ACanExecute);
            }
        }
        /// <summary>
        /// 删除日志文件方法
        /// </summary>
        private void DeletelogfileFunc()
        {
            MessageBoxResult Resul =System.Windows.MessageBox.Show($"是否确定删除日志","信息提示",MessageBoxButton.YesNo);
            if (Resul == MessageBoxResult.Yes)
            {
                TextLog = "";
            }
            return;
        }
    }
}
