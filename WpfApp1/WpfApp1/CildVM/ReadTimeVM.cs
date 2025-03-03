using System;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;


namespace WpfApp1.ChildVM
{

    public class ReadTimeVM : ViewModelBase
    {

        private ObservableCollection<string> _keyCombox;
        private ConnecterPortService connecterPortService;
        private MainViewModel viewModeltxt;
        public ObservableCollection<string> KeyCombox
        {
            get { return _keyCombox; }
            set
            {
                _keyCombox = value;
                RaisePropertyChanged(() => KeyCombox);
            }
        }
        private static string _selectedkey = "设备1";
        public string Selectedkey
        {
            get { return _selectedkey; }
            set
            {
                _selectedkey = value;
                RaisePropertyChanged(() => Selectedkey);
            }
        }
    

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="mainView"></param>
        public ReadTimeVM(ConnecterPortService manager, MainViewModel mainView)
        {
            connecterPortService = manager;
            viewModeltxt = mainView;
            _keyCombox = new ObservableCollection<string>();
            UP_keydata();
            
        }
        public ReadTimeVM()
        {
        }

        private static string _readTime = "1";
        public string ReadTime
        {
            get { return _readTime; }
            set
            {
                _readTime = value;
                RaisePropertyChanged(() => ReadTime);
            }
        }

       
        /// <summary>
        /// 控件命令
        /// </summary>
        public ICommand Cilcktime_Upnum
        {
            get
            {
                return new RelayCommand(UpFunc, ACanExecute);
            }
        }
        public ICommand Cilcktime_Downnum
        {
            get
            {
                return new RelayCommand(DownFunc, ACanExecute);
            }
        }
        public ICommand Cilcktime_OK
        {
            get
            {
                return new RelayCommand(GetAddress, ACanExecute);
            }
        }
        public ICommand Cilcktime_Exit
        {
            get
            {
                return new RelayCommand(ExitAddressWindow, ACanExecute);
            }
        }

    
        void UpFunc()
        {
            try
            {
                string num = _readTime;
                if (int.TryParse(num, out int number))
                {
                    if (number > 0 && number < 120)
                    {
                        number++;
                        ReadTime = number.ToString();
                    }
                    else
                    {
                        ReadTime = "1";
                    }
                }
                else
                {
                    ReadTime = "1";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}");
            }
        }
        void DownFunc()
        {
            try
            {
                string num = _readTime;
                if (int.TryParse(num, out int number))
                {
                    if (number > 0 && number < 120)
                    {
                        number--;
                        ReadTime = number.ToString();
                    }
                    else
                    {
                        ReadTime = "1";
                    }

                }
                else { ReadTime = "1"; }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}");
            }

        }
        /// <summary>
        /// 确认设置时间方法
        /// </summary>
        void GetAddress()
        {
            try
            {
                if (connecterPortService.IsConnected)
                {
                    if (int.TryParse(_readTime, out int number) && _readTime != "0")
                    {
                        ReadTime = number.ToString();
                        viewModeltxt.TextLog += $"【设置时间为：{number}秒，开始定时读取设备--{Selectedkey}--】\n";
                        connecterPortService.ConnectTime(number);
                        System.Windows.MessageBox.Show($"值为：{Selectedkey}");
                        //connecterPortService.TestFunc();
                        viewModeltxt.IsbtnEnableTime=true;
                        viewModeltxt.BtntimedataOpenOrClose = "关闭定时读取";
                        ExitAddressWindow();

                    }
                    else
                    {
                        ReadTime = "1";
                        System.Windows.MessageBox.Show($"设置的值无效！");
                        ExitAddressWindow();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show($"串口未打开！！！请检查！");
                    ExitAddressWindow();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"读取失败这是readtime{ex.Message}");
            }

        }

        private void ExitAddressWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            if (window != null)
            {
                window.Close();
            }
        }
        bool ACanExecute()
        {
            return true;
        }

        public void UP_keydata()
        {
            foreach (KeyValuePair<string, byte[]> pair in connecterPortService.dict)
            {
                _keyCombox.Add(pair.Key);
            }
        }

        }
    }
