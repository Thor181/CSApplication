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

        public const int x31 = 0x31;
        public const int x32 = 0x32;
        public const int x33 = 0x33;

        private readonly int _baudRate = 9600;
        private readonly Parity _parity = Parity.None;
        private readonly int _dataBits = 8;

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
            try
            {
                serialPort.Open();
                Logger.Instance.Log($"Порт {serialPort.PortName} открыт", LogLevel.Success);
            }
            catch (Exception e)
            {
                Logger.Instance.Log($"При открытии порта {serialPort.PortName} возникла ошибка", LogLevel.Error, e);
            }
        }

        private void InputPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;
            try
            {
                var data = port.ReadExisting();
                Logger.Instance.Log($"Получено ({port.PortName}): {data}", LogLevel.Info);

                PortDataReceived?.Invoke(port, data);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Instance.Log($"Порт {port.PortName} не открыт", LogLevel.Error, ex);
                OpenPort(port);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log("Ошибка", LogLevel.Error, ex);
            }
        }

        private void InputPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            if (sender is SerialPort port)
                Logger.Instance.Log($"От порта {port.PortName} получена ошибка | {e.EventType}", LogLevel.Error);
            else
                Logger.Instance.Log($"Непредвиденная ошибка | {e.EventType}", LogLevel.Error);
        }

        public void SendResponse(SerialPort serialPort, byte data)
        {
            try
            {
                serialPort.Write(new byte[] { data }, 0, 1);
                Logger.Instance.Log($"Отправлено ({serialPort.PortName}): {data}", LogLevel.Info);
            }
            catch (InvalidOperationException e)
            {
                Logger.Instance.Log($"Порт {serialPort.PortName} не открыт", LogLevel.Error, e);
                OpenPort(serialPort);
            }
            catch (Exception e)
            {
                Logger.Instance.Log($"При отправке ответа на порт {serialPort.PortName} возникла ошибка", LogLevel.Error, e);
            }

        }
    }
}
