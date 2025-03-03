using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    public class AddressWindowVM:ViewModelBase
    {
        //地址字段
        private static string _address = "1";
        public string Address
        {
            get{ return _address;}
            set {
                _address = value;
                RaisePropertyChanged(() => Address);
            }
        }

        public ICommand Cilck_Upnum
        {
            get
            {
                return new RelayCommand(UpFunc, ACanExecute);
            }
        }
        public ICommand Cilck_Downnum
        {
            get
            {
                return new RelayCommand(DownFunc, ACanExecute);
            }
        }
        public ICommand Cilck_OK
        {
            get
            {
                return new RelayCommand(GetAddress, ACanExecute);
            }
        }
        public ICommand Cilck_Exit
        {
            get
            {
                return new RelayCommand(ExitAddressWindow, ACanExecute);
            }
        }

        public AddressWindowVM()
        {
          
        }
        void UpFunc()
        {
            try
            {
                string num = _address;
                if (int.TryParse(num, out int number))
                {
                    if (number > 0&&number<248)
                    {
                        number++;
                        Address = number.ToString();
                    }
                    else
                    {
                        Address = "1";
                    }
                }
                else
                {
                    Address = "1";
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
                string num = _address;
                if (int.TryParse(num, out int number))
                {
                    if (number > 0&&number < 248)
                    {
                        number--;
                        Address = number.ToString();
                    }
                    else
                    {
                        Address = "1";
                    }

                }
                else { Address = "1"; }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}");
            }

        }
        void GetAddress()
        {
            try
            {
                if (int.TryParse(_address, out int number)&& _address!="0")
                {
                    Address = number.ToString();
                   // System.Windows.MessageBox.Show($"值是：{_address}");
                    ExitAddressWindow();

                }
                else
                {
                    Address = "1";
                  //System.Windows.MessageBox.Show($"值是：{_address}");
                    ExitAddressWindow();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}");
            }

        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
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
    }
}
