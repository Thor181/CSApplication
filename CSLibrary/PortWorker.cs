using CSLibrary.Log;
using System.IO.Ports;

namespace CSLibrary
{
    public class PortWorker
    {
        public SerialPort InputPort { get; set; }
        public SerialPort OutputPort { get; set; }
        public SerialPort QR1Port { get; set; }
        public SerialPort QR2Port { get; set; }

        private readonly int _baudRate = 9600;
        private readonly Parity _parity = Parity.None;
        private readonly int _dataBits = 8;

        public PortWorker()
        {
            InputPort = new SerialPort(AppConfig.Instance.PortInputName, _baudRate, _parity, _dataBits);
            InputPort.DataReceived += InputPort_DataReceived;

            OutputPort = new SerialPort(AppConfig.Instance.PortOutputName, _baudRate, _parity, _dataBits);
            OutputPort.DataReceived += InputPort_DataReceived;

            QR1Port = new SerialPort(AppConfig.Instance.PortQR1Name, _baudRate, _parity, _dataBits);
            QR1Port.DataReceived += InputPort_DataReceived;

            QR2Port = new SerialPort(AppConfig.Instance.PortQR2Name, _baudRate, _parity, _dataBits);
            QR2Port.DataReceived += InputPort_DataReceived;
        }

        public void OpenPorts()
        {
            try
            {
                OpenPort(InputPort);
                OpenPort(OutputPort);
                OpenPort(QR1Port);
                OpenPort(QR2Port);
            }
            catch (Exception e)
            {
                Logger.Instance.Log("При открытии портов возникла ошибка", LogLevel.Error, e);
                throw;
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
            var line = port.ReadExisting();
         
            Logger.Instance.Log(line, LogLevel.Info);
        }

        private void InputPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new Exception(e.EventType.ToString());
        }
    }
}
