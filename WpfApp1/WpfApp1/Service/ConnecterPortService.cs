using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using Microsoft.Win32;
using Modbus.Device;
using WpfApp1.Service;

namespace WpfApp1
{
    public class ConnecterPortService : ViewModelBase
    {
        //双向检查锁定
        private static readonly object _lock = new object();
        private static ConnecterPortService _instance;
        public static ConnecterPortService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConnecterPortService();
                        }
                    }
                }
                return _instance;
            }
        }
        //报文
        public Dictionary<string, byte[]> dict;
        //public ObservableCollection<byte[]> Messageslist { get; set; }
        //public ObservableCollection<byte[]> Messageslist222 { get; set; }
        //public ObservableCollection<string> Messagesname { get; set; }
        //public ObservableCollection<string> Strings { get; set; }

        private SaveFileDialog _saveFiledialog;
        private MainViewModel _mainViewModel;
        private IWindowService _windowService;
        private static SerialPort port;
        public static byte Address { get; set; }
        public List<string> GetPortNames { get; set; }
        public string GetAddress { get; set; }
        public string Portnum { get; set; }
        public int Baudnum { get; set; }
        public int Databits { get; set; }
        public StopBits Stopbits { get; set; }
        public Parity Parity { get; set; }

        private List<byte[]> ValueList { get; set; }
        private List<string> Keystring { get; set; }
        /// <summary>
        /// 主窗体ui显示
        /// </summary>
        /// <param name="mainViewModel"></param>
        public void SetMainViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public ConnecterPortService()
        {
            _windowService = new WindowService();
            Initializedata();
            port = new SerialPort(Portnum, Baudnum, Parity, Databits, Stopbits);
            _saveFiledialog = new SaveFileDialog();
            dict = new Dictionary<string, byte[]>();
            ValueList = new List<byte[]>();
            Keystring = new List<string>();
         
  
            AddDataFunc();
         

            _dataTimer = new Timer(3000);
            _dataTimer.Elapsed += DataTimer_Elapsed;
            _dataTimer.AutoReset = true;

        }



        /// <summary>
        /// 初始化连接所需的信息
        /// </summary>
        public void Initializedata()
        {
            GetPortNames = _windowService.PortData();
            GetAddress = _windowService.AddressId();
            slaveAddress = StrTObyte(GetAddress);
        //把连接所需要的信息转位可使用类型
        
             Portnum = GetPortNames[0];
            Baudnum = StrTOint(GetPortNames[1]);
            Databits = StrTOint(GetPortNames[2]);
            string temp = GetPortNames[3];
            Stopbits = GetStopBits(temp);
            Parity parity = GetParity(GetPortNames[4]);
        }
        //public string GetAddressData()
        //{
        //    return _windowService.AddressId();
        //}



        private bool _isConnected;
        /// <summary>
        /// 串口是否连接
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                RaisePropertyChanged(() => IsConnected);
            }
        }



        /// <summary>
        /// 打开串口
        /// </summary>
        public void OpenPort()
        {
            Initializedata();
           
            if (port.IsOpen)
            {
                port.Close();
            }
            try
            {
                port.Open();
                IsConnected = true;

                Modbus.Device.ModbusSerialMaster master = Modbus.Device.ModbusSerialMaster.CreateRtu(port);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"打开失败：{ex.Message}");
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
                IsConnected = false;
            }
        }


        private const ushort DefaultReadCount = 10;
        private byte slaveAddress; 
        private ushort startAddress = 0;
        /// <summary>
        /// 封装的读取方法，使用默认数量
        /// </summary>
        /// <param name="master"></param>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <returns></returns>
        public static ushort[] WithDefault(IModbusMaster master, byte slaveAddress, ushort startAddress)
        {
            return master.ReadHoldingRegisters(slaveAddress, startAddress, DefaultReadCount);
        }
        /// <summary>
        /// 获取实时数据
        /// </summary>
        /// <returns></returns>
        ///  private const ushort DefaultReadCount = 10; 
        public string[] ConnectGetportData()
        {
          
            try
            {
                if (port != null && port.IsOpen)
                {
                    _dataTimerconnect = new Timer(3000);
                    _dataTimerconnect.Start();
                    _dataTimerconnect.Elapsed += new ElapsedEventHandler(DataTimer_ConnectException);
                    _dataTimerconnect.AutoReset = false;

                    Modbus.Device.ModbusSerialMaster master = Modbus.Device.ModbusSerialMaster.CreateRtu(port);

                    ushort[] datas = WithDefault(master, slaveAddress, startAddress);
                    if (datas != null && datas.Length > 0)
                    {
                        _dataTimerconnect.Stop();
                        string result = $" {string.Join("   ", datas.Select(p => p.ToString()).ToArray())}";
                        return new string[] { result };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show($"未获取数据，请检查！");
            }
            return null;
        }




        private Timer _dataTimer;
        private Timer _dataTimerconnect;
        private void DataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (IsConnected && port != null && port.IsOpen)
            {
                try
                {
                   
                    TestFunc(SendData(KeyFunc()));
                }
                catch (Exception ex)
                {
                    _mainViewModel.TextLog += $"错误：{ex.Message}\n";
                }
            }
            else
            {
                _dataTimer.Stop();
                _mainViewModel.TextLog += $"异常中断，请检查！\n";
            }
        }
        /// <summary>
        /// 定时获取数据方法
        /// </summary>
        /// <param name="times"></param>
        public void ConnectTime(int times)
        {
            try
            {
                if (IsConnected && port.IsOpen)
                {
                    _dataTimer = new Timer(times * 1000);
                    _dataTimer.Elapsed += new ElapsedEventHandler(DataTimer_Elapsed);
                    _dataTimer.AutoReset = true;
                    _dataTimer.Start();
                }
                else
                {
                    System.Windows.MessageBox.Show($"串口未打开！！请检查！");
                }
            }
            catch (Exception )
            {
                System.Windows.MessageBox.Show($"未获取到数据，请检查！");
            }
        }
        public void DisConnectTimer()
        {
            if (port != null && port.IsOpen)
            {
                _dataTimer.Stop();
            }
        }


       private static int ToRead = 0;
        /// <summary>
        /// 校验和收发数据的方法(定时读取用)///继续三次套娃
        /// </summary>
        public void TestFunc(byte[] bytes)
        {
            try
            {
                byte[] frameTosend = PrepareFrameForSending(bytes);
                port.DiscardInBuffer();
                SendmessageFunc(frameTosend);
                port.Write(frameTosend, 0, frameTosend.Length);
                System.Threading.Thread.Sleep(100);

                ToRead = port.BytesToRead;
                if (ToRead > 0)
                {
                    byte[] buffer = new byte[ToRead];
                    port.Read(buffer, 0, ToRead);
                    bool isValid = ValidateReceivedFrame(buffer);
                    PickupmessageFunc(buffer);

                }
                else
                {
                  
                    port.DiscardInBuffer();
                    SendmessageFunc(frameTosend);
                    port.Write(frameTosend, 0, frameTosend.Length);
                    System.Threading.Thread.Sleep(100);
                    ToRead = port.BytesToRead;
                    if (ToRead > 0)
                    {
                        byte[] buffer = new byte[ToRead];
                        port.Read(buffer, 0, ToRead);
                        bool isValid = ValidateReceivedFrame(buffer);
                        PickupmessageFunc(buffer);
                    }
                    else
                    {
                       
                        port.DiscardInBuffer();
                        SendmessageFunc(frameTosend);
                        port.Write(frameTosend, 0, frameTosend.Length);
                        System.Threading.Thread.Sleep(100);
                        ToRead = port.BytesToRead;
                        if (ToRead > 0)
                        {
                            byte[] buffer = new byte[ToRead];
                            port.Read(buffer, 0, ToRead);
                            bool isValid = ValidateReceivedFrame(buffer);
                            PickupmessageFunc(buffer);
                        }
                        else
                        {
                            
                            System.Windows.MessageBox.Show($"未获取数据，请检查！");
                            DisConnectTimer();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _mainViewModel.TextLog += ($"【错误】：{ex.Message}\n");
            }
        }
        /// <summary>
        /// 发送数据方法
        /// </summary>
        /// <param name="message"></param>
        public void SendmessageFunc(byte[] message)
        {
            _mainViewModel.TextLog += ($"【发送】： ");
            foreach (var item in message)
            {
                _mainViewModel.TextLog += ($"{item.ToString("X2")}   ");
            }
            _mainViewModel.TextLog += ($"\n");
        }
        /// <summary>
        /// 接收数据方法
        /// </summary>
        /// <param name="message"></param>
        public void PickupmessageFunc(byte[] message)
        {
            _mainViewModel.TextLog += ($"【接收】： ");
            foreach (var item in message)
            {
                _mainViewModel.TextLog += ($"{item.ToString("X2")}   ");
            }
            _mainViewModel.TextLog += ($"\n");
        }

        //循环集合的索引
        private static int i = 0;
        private Timer _dataTimerRead;
        /// <summary>
        /// 读取全部设备参数
        /// </summary>
        public void ReadNowData()
        {
            try
            {
                if (port != null && port.IsOpen)
                {
                    _dataTimerRead = new Timer(2000);
                    _dataTimerRead.Start();
                    _dataTimerRead.Elapsed += new ElapsedEventHandler(DataTimer_ReadData);
                    _dataTimerRead.AutoReset = true;
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show($"未获取到数据，请检查！");
            }

        }
        private void DataTimer_ReadData(object sender, ElapsedEventArgs e)
        {
            
            if (i < ValueList.Count)
            {
                if (IsConnected && port != null && port.IsOpen)
                {
                    try
                    {
                        ReadnowdataFunc(SendData(ValueList[i]), Keystring[i]);
                        i++;
                    }
                    catch (Exception ex)
                    {
                        _mainViewModel.TextLog += $"错误：{ex.Message}\n";
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show($"读取仪表参数失败！请检查！", "信息提醒");
                    _dataTimerRead.Stop();
                }
            }
            else
            {
                i = 0;
                _dataTimerRead.Stop();
            }
        }



        int bytesToRead = 0;
        /// <summary>
        /// 套娃三次发送数据判断，感觉优化空间很大
        /// </summary>
        /// <param name="Messageslist"></param>
        /// <param name="Messagesname"></param>
        public void ReadnowdataFunc(byte[] Messageslist, string Messagesname)
        {
            try
            {
                byte[] command = Messageslist;
                byte[] frameTosend = PrepareFrameForSending(command);
                port.DiscardInBuffer();

                _mainViewModel.TextLog += ($"================={Messagesname} =================\n");
                SendmessageFunc(frameTosend);
                port.Write(frameTosend, 0, frameTosend.Length);
                System.Threading.Thread.Sleep(100);

                bytesToRead = port.BytesToRead;
                if (bytesToRead > 0)
                {
                    byte[] buffer = new byte[bytesToRead];
                    port.Read(buffer, 0, bytesToRead);
                    bool isValid = ValidateReceivedFrame(buffer);
                    PickupmessageFunc(buffer);
                }
                else
                {

                    SendmessageFunc(frameTosend);
                    port.Write(frameTosend, 0, frameTosend.Length);
                    System.Threading.Thread.Sleep(200);
                    bytesToRead = port.BytesToRead;
                    if (bytesToRead > 0)
                    {
                        byte[] buffer = new byte[bytesToRead];
                        port.Read(buffer, 0, bytesToRead);
                        bool isValid = ValidateReceivedFrame(buffer);
                        PickupmessageFunc(buffer);

                    }
                    else
                    {
                        SendmessageFunc(frameTosend);
                        port.Write(frameTosend, 0, frameTosend.Length);
                        System.Threading.Thread.Sleep(300);
                        if (bytesToRead > 0)
                        {
                            byte[] buffer = new byte[bytesToRead];
                            port.Read(buffer, 0, bytesToRead);
                            bool isValid = ValidateReceivedFrame(buffer);
                            PickupmessageFunc(buffer);
                        }
                        else
                        {
                            _mainViewModel.TextLog += ($"【错误】：未接收到数据\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _mainViewModel.TextLog += ($"【错误】：{ex.Message}\n");
            }
        }





        private static int number = 0;
        /// <summary>
        /// 保存文件
        /// </summary>
        public SaveFileDialog SaveFiledialog
        {
            get { return _saveFiledialog; }
            set
            {
                _saveFiledialog = value;

                RaisePropertyChanged(() => SaveFiledialog);
            }
        }
        public void SaveFileDialog()
        {
            number++;
            SaveFiledialog.Filter = "文本文件|*.txt";
            SaveFiledialog.FileName = $"数据_0{number}";
            SaveFiledialog.DefaultExt = "txt";
            SaveFiledialog.AddExtension = true;
            SaveFiledialog.RestoreDirectory = true;
            SaveFiledialog.OverwritePrompt = true;
            SaveFiledialog.CheckPathExists = true;
            // SaveFiledialog.CheckFileExists = true;
            SaveFiledialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (SaveFiledialog.ShowDialog() == true)
            {
                try
                {
                    string path = SaveFiledialog.FileName;
                    string text = _mainViewModel.TextLog;
                    System.IO.File.WriteAllText(path, text);
                    System.Windows.MessageBox.Show("保存成功！");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"保存失败：{ex.Message}");
                }
            }
        }



        /// <summary>
        /// 异常连接3秒后关闭串口
        /// </summary>
        public void DataTimer_ExceptionConnect()
        {
            if (port != null && port.IsOpen)
            {
                port.Dispose();
                IsConnected = false;

            }
        }
        private void DataTimer_ConnectException(object sender, ElapsedEventArgs e)
        {
            if (IsConnected && port != null && port.IsOpen)
            {
                _dataTimerconnect.Stop();
                DataTimer_ExceptionConnect();
                IsConnected = false;
                _mainViewModel.IsbtnEnable = true;
                _mainViewModel.Btnopenport = "打开串口";
            }
        }



        /// <summary>
        /// 停止位，转枚举类型
        /// </summary>
        /// <param name="stopbits"></param>
        /// <returns></returns>
        public static StopBits GetStopBits(string stopbits)
        {
            switch (stopbits.ToLower())
            {
                case "1":
                case "one":
                    return StopBits.One;
                case "1.5":
                case "onepointfive":
                    return StopBits.OnePointFive;
                case "2":
                case "two":
                    return StopBits.Two;
                default:
                    return StopBits.One;
            }
        }


        /// <summary>
        /// 校验位,转枚举类型
        /// </summary>
        /// <param name="parity"></param>
        /// <returns></returns>
        public static Parity GetParity(string parity)
        {
            switch (parity.ToLower())
            {
                case "odd":
                    return Parity.Odd;
                case "even":
                    return Parity.Even;
                case "none":
                    return Parity.None;
                case "mark":
                    return Parity.Mark;
                case "space":
                    return Parity.Space;
                default:
                    return Parity.None;

            }
        }

        /// <summary>
        ///波特率和数据位 字符串转int类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int StrTOint(string str)
        {
            if (int.TryParse(str, out int number))
            {
                return number;
            }
            return 0;
        }


        /// <summary>
        /// 把字符串转byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte StrTObyte(string str)
        {
            if (byte.TryParse(str, out byte result))
            {
                return result;
            }
            else
            {
                return 1;
            }
        }



        /// <summary>
        /// CRC校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CalculateCRC(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) == 0x0001)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            byte[] crcBytes = new byte[2];
            crcBytes[0] = (byte)(crc & 0xFF);
            crcBytes[1] = (byte)((crc >> 8) & 0xFF);
            return crcBytes;
        }
        /// <summary>
        /// 准备发送的帧
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] PrepareFrameForSending(byte[] data)
        {
            byte[] crc = CalculateCRC(data);
            List<byte> frame = new List<byte>(data);
            frame.AddRange(crc);
            return frame.ToArray();
        }
        /// <summary>
        /// 验证接收到的帧
        /// </summary>
        /// <param name="receivedFrame"></param>
        /// <returns></returns>
        public static bool ValidateReceivedFrame(byte[] receivedFrame)
        {
            if (receivedFrame.Length < 2)
            {
                return false;
            }
            byte[] receivedData = new byte[receivedFrame.Length - 2];
            Array.Copy(receivedFrame, 0, receivedData, 0, receivedData.Length);
            byte[] receivedCRC = new byte[2];
            Array.Copy(receivedFrame, receivedFrame.Length - 2, receivedCRC, 0, 2);

            byte[] calculatedCRC = CalculateCRC(receivedData);
            return calculatedCRC[0] == receivedCRC[0] && calculatedCRC[1] == receivedCRC[1];
        }


        

        /// <summary>
        /// 更新报文开头地址位的方法
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public byte[] SendData(byte[] bytes)
        {
            int newIntValue = StrTOint(_windowService.AddressId());
            byte newByteValue = (byte)newIntValue;

          
            byte[] newMessage = new byte[bytes.Length + 1];
            newMessage[0] = newByteValue;
            Array.Copy(bytes, 0, newMessage, 1, bytes.Length);
            return newMessage;
        }

        /// <summary>
        /// 优化1，创建键值对集合储存
        /// </summary>
        public void AddDataFunc()
        {
            byte[] value1 = { 0x03, 0x00, 0x00, 0x00, 0x13 };
            dict.Add("设备1", value1);

            byte[] value2 = { 0x03, 0x00, 0x00, 0x00, 0x10 };
            dict.Add("设备2", value2);

            byte[] value3 = { 0x03, 0x00, 0x0A, 0x00, 0x19 };
            dict.Add("设备3", value3);

            byte[] value4 = { 0x03, 0x00, 0x00, 0x00, 0x50 };
            dict.Add("设备4", value4);

            byte[] value5 = { 0x03, 0x00, 0x0A, 0x00, 0x15 };
            dict.Add("设备5", value5);

            byte[] value6 = { 0x03, 0x00, 0x00, 0x00, 0xC };
            dict.Add("设备6", value6);

            byte[] value7 = { 0x03, 0x00, 0x0A, 0x00, 0x25 };
            dict.Add("设备7", value7);

            byte[] value8= { 0x03, 0x00, 0x0A, 0x00, 0x25 };
            dict.Add("设备8", value8);

            ValueList = new List<byte[]>(dict.Values);
            Keystring = new List<string>(dict.Keys);

        }


        private string Keydata { get; set; }
        /// <summary>
        /// 获取被选中的定时读取设备
        /// </summary>
        /// <returns></returns>
        public byte[] KeyFunc()
        {
            Keydata = _windowService.Keys();
            if (dict.TryGetValue(Keydata, out byte[] result2))
            {
                return result2;
            }
            return null;
        }

      
       
    }
}