using CSLibrary.Log;
using System.IO.Ports;

namespace CSLibrary
{
    public class PortWorker
    {
        public SerialPort InputPort { get; private set; }
        public SerialPort OutputPort { get; private set; }
        public SerialPort QR1Port { get; private set; }
        public SerialPort QR2Port { get; private set; }

        public delegate void PortDataReceivedEventHandler(SerialPort port, string data);
        public event PortDataReceivedEventHandler PortDataReceived;

        public const int x06 = 0x06;
        public const int x31 = 0x31;
        public const int x32 = 0x32;
        public const int x33 = 0x33;
        public const int x34 = 0x34;
        public const int x41 = 0x41;
        public const int x42 = 0x42;
        public const int x43 = 0x43;

        private readonly int _baudRate = 9600;
        private readonly int _qrBaudRate = 115200;

        private readonly Parity _parity = Parity.None;
        private readonly int _dataBits = 8;

        public void OpenPorts()
        {
            try
            {
                InputPort = new SerialPort(AppConfig.Instance.PortInputName, _baudRate, _parity, _dataBits);
                InputPort.DataReceived += PortDataReceivedInternal;
                InputPort.ErrorReceived += PortErrorReceivedInternal;

                OutputPort = new SerialPort(AppConfig.Instance.PortOutputName, _baudRate, _parity, _dataBits);
                OutputPort.DataReceived += PortDataReceivedInternal;
                OutputPort.ErrorReceived += PortErrorReceivedInternal;

                QR1Port = new SerialPort(AppConfig.Instance.PortQR1Name, _qrBaudRate, _parity, _dataBits);
                QR1Port.DataReceived += PortDataReceivedInternal;
                QR1Port.ErrorReceived += PortErrorReceivedInternal;

                QR2Port = new SerialPort(AppConfig.Instance.PortQR2Name, _qrBaudRate, _parity, _dataBits);
                QR2Port.DataReceived += PortDataReceivedInternal;
                QR2Port.ErrorReceived += PortErrorReceivedInternal;

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

        private void PortDataReceivedInternal(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;
            try
            {
                Thread.Sleep(100);
                var data = port.ReadExisting();
                Logger.Instance.Log($"<- Получено ({port.PortName}): {data}", LogLevel.Info);

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

        private void PortErrorReceivedInternal(object sender, SerialErrorReceivedEventArgs e)
        {
            if (sender is SerialPort port)
                Logger.Instance.Log($"От порта {port.PortName} получена ошибка | {e.EventType}", LogLevel.Error);
            else
                Logger.Instance.Log($"Непредвиденная ошибка | {e.EventType}", LogLevel.Error);
        }

        public void SendHexResponse(SerialPort serialPort, byte data)
        {
            try
            {
                serialPort.Write(new byte[] { data }, 0, 1);
                Logger.Instance.Log($"-> Отправлено ({serialPort.PortName}): 0x{Convert.ToHexString(new byte[] { data })}", LogLevel.Info);
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
