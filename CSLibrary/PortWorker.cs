using CSLibrary.Log;
using System.IO.Ports;

namespace CSLibrary
{
    public class PortWorker
    {
        private SerialPort InputPort { get; set; }
        private SerialPort OutputPort { get; set; }
        private SerialPort QR1Port { get; set; }
        private SerialPort QR2Port { get; set; }

        public delegate void PortDataReceivedEventHandler(SerialPort port, string data);
        public event PortDataReceivedEventHandler PortDataReceived;

        private readonly int _baudRate = 9600;
        private readonly Parity _parity = Parity.None;
        private readonly int _dataBits = 8;

        public PortWorker()
        {
            
        }

        public void OpenPorts()
        {
            try
            {
                InputPort = new SerialPort(AppConfig.Instance.PortInputName, _baudRate, _parity, _dataBits);
                InputPort.DataReceived += InputPort_DataReceived;

                OutputPort = new SerialPort(AppConfig.Instance.PortOutputName, _baudRate, _parity, _dataBits);
                OutputPort.DataReceived += InputPort_DataReceived;

                QR1Port = new SerialPort(AppConfig.Instance.PortQR1Name, _baudRate, _parity, _dataBits);
                QR1Port.DataReceived += InputPort_DataReceived;

                QR2Port = new SerialPort(AppConfig.Instance.PortQR2Name, _baudRate, _parity, _dataBits);
                QR2Port.DataReceived += InputPort_DataReceived;

                OpenPort(InputPort);
                OpenPort(OutputPort);
                OpenPort(QR1Port);
                OpenPort(QR2Port);
            }
            catch (Exception e)
            {
                Logger.Instance.Log("При открытии портов возникла ошибка", LogLevel.Error, e);
            }
        }

        private void OpenPort(SerialPort serialPort)
        {
            serialPort.Open();
            Logger.Instance.Log($"Порт {serialPort.PortName} открыт", LogLevel.Success);
        }

        private void InputPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;
            var data = port.ReadExisting();
         
            Logger.Instance.Log($"Получено ({port.PortName}): {data}", LogLevel.Info);

            PortDataReceived?.Invoke(port, data);
        }

        //private void OutputPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    var port = (SerialPort)sender;
        //    var line = port.ReadExisting();

        //    Logger.Instance.Log(line, LogLevel.Info);
        //}

        //private void QR1Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    var port = (SerialPort)sender;
        //    var line = port.ReadExisting();

        //    Logger.Instance.Log(line, LogLevel.Info);
        //}

        //private void QR2Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    var port = (SerialPort)sender;
        //    var line = port.ReadExisting();

        //    Logger.Instance.Log(line, LogLevel.Info);
        //}

        //private void Port_DataReceived(string portName, SerialPort serialPort)
        //{

        //}

        private void InputPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new Exception(e.EventType.ToString());
        }
    }
}
