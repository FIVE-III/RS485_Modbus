using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
   
    public class CildWindowVM : ViewModelBase
    {
        private static readonly object _lock = new object();
        private static CildWindowVM _instance;
        public static CildWindowVM Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CildWindowVM();
                        }
                    }
                }
                return _instance;
            }
        }

        private MainViewModel model;
        //combobox的数据源
        private ObservableCollection<string> _portCombox = new ObservableCollection<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6" , "COM7", "COM8", "COM9" , "COM10", "COM11", "COM12", "COM13", "COM14" };
        private ObservableCollection<string> _baudCombox = new ObservableCollection<string> { "1200", "2400", "4800", "9600", "14400", "19200", "38400" };
        private ObservableCollection<string> _databitsCombox = new ObservableCollection<string> { "5", "6", "7", "8" };
        private ObservableCollection<string> _stopbitsCombox = new ObservableCollection<string> { "1", "1.5", "2" };
        private ObservableCollection<string> _parityCombox = new ObservableCollection<string> { "None", "Odd", "Even", "Mark", "Space" };

        public ObservableCollection<string> PortCombox
        {
            get { return _portCombox; }
            set
            {
                _portCombox = value;
                RaisePropertyChanged(() => PortCombox);
            }
        }
        public ObservableCollection<string> BaudCombox
        {
            get { return _baudCombox; }
            set
            {
                _baudCombox = value;
                RaisePropertyChanged(() => BaudCombox);
            }
        }
        public ObservableCollection<string> DatabitsCombox
        {
            get { return _databitsCombox; }
            set
            {
                _databitsCombox = value;
                RaisePropertyChanged(() => DatabitsCombox);
            }
        }
        public ObservableCollection<string> StopbitsCombox
        {
            get { return _stopbitsCombox; }
            set
            {
                _stopbitsCombox = value;
                RaisePropertyChanged(() => StopbitsCombox);
            }
        }
        public ObservableCollection<string> ParityCombox
        {
            get { return _parityCombox; }
            set
            {
                _parityCombox = value;
                RaisePropertyChanged(() => ParityCombox);
            }
        }

        private static string _selectedPort = "COM13";
        public string SelectedPort
        {
            get { return _selectedPort; }
            set
            {
                _selectedPort = value;
                RaisePropertyChanged(() => SelectedPort);
            }
        }
        private static string _selectedbaud = "9600";
        public string Selectedbaud
        {
            get { return _selectedbaud; }
            set
            {
                _selectedbaud = value;
                RaisePropertyChanged(() => Selectedbaud);
            }
        }
        private static string _selectedDatabits = "8";
        public string SelectedDatabits
        {
            get { return _selectedDatabits; }
            set
            {
                _selectedDatabits = value;
                RaisePropertyChanged(() => SelectedDatabits);
            }
        }
        private static string _selectedStopbits = "1";
        public string SelectedStopbits
        {
            get { return _selectedStopbits; }
            set
            {
                _selectedStopbits = value;
                RaisePropertyChanged(() => SelectedStopbits);
            }
        }
        private static string _selectedParity = "Odd";
        public string SelectedParity
        {
            get { return _selectedParity; }
            set
            {
                _selectedParity = value;
                RaisePropertyChanged(() => SelectedParity);
            }
        }


      
    

        /// <summary>
        /// 提交数据
        /// </summary>
        private void Commitdata()
        {
            try
            {
                if (portdata.Count > 0)
                {
                    portdata.Clear();
                }
                else
                {
                    string[] temp = { SelectedPort, Selectedbaud, SelectedDatabits, SelectedStopbits, SelectedParity };
                    portdata.AddRange(temp);
                    model.IsbtnEnable = true;
                    ExitWindow();

                    //System.Windows.MessageBox.Show($"数据是：{Portdata()}---其他是{_selectedPort}{_selectedbaud}{_selectedDatabits}{_selectedStopbits}{_selectedParity}");
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show($"数据提交出错{ex.Message}");
            }
        }
        public ICommand CommitWindow {
            get
            {
                return new RelayCommand(Commitdata, ACanExecute);
            }

        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void ExitWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (window != null)
            {
                window.Close();
                model.IsbtnEnable = true;
            }
        }
         public ICommand CloseWindow
        {
            get
            {
                return new RelayCommand(ExitWindow, ACanExecute);
            }
        }


        bool ACanExecute()
        {
            return true;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CildWindowVM(MainViewModel mainView)
        {

            model = mainView;

        }
        public CildWindowVM()
        {
        }

        /// <summary>
        /// 获取保存的串口数据
        /// </summary>
        public List<string> Portdata()
        {
            string[] temp = { _selectedPort, _selectedbaud, _selectedDatabits, _selectedStopbits, _selectedParity };
            if (portdata.Count == 0)
            {
                return temp.ToList();
            }
            return portdata.ToList();
        }
        static List<string> portdata=new List<string>();
    }
}
