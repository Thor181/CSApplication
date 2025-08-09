using Microsoft.Extensions.Logging;
using System.IO.Ports;

namespace CSLibrary.V2
{
    public class PortWorker
    {
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

        private readonly IPortWorkerOptions _options;
        private readonly ILogger _logger;

        public SerialPort InputPort { get; private set; }
        public SerialPort OutputPort { get; private set; }
        public SerialPort QR1Port { get; private set; }
        public SerialPort QR2Port { get; private set; }

        public delegate void PortDataReceivedEventHandler(SerialPort port, string data);
        public event PortDataReceivedEventHandler PortDataReceived;

        public PortWorker(IPortWorkerOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        public void OpenPorts()
        {
            try
            {
                InputPort = new SerialPort(_options.PortInputName, _baudRate, _parity, _dataBits);
                InputPort.DataReceived += PortDataReceivedInternal;
                InputPort.ErrorReceived += PortErrorReceivedInternal;

                OutputPort = new SerialPort(_options.PortOutputName, _baudRate, _parity, _dataBits);
                OutputPort.DataReceived += PortDataReceivedInternal;
                OutputPort.ErrorReceived += PortErrorReceivedInternal;

                QR1Port = new SerialPort(_options.PortQR1Name, _qrBaudRate, _parity, _dataBits);
                QR1Port.DataReceived += PortDataReceivedInternal;
                QR1Port.ErrorReceived += PortErrorReceivedInternal;

                QR2Port = new SerialPort(_options.PortQR2Name, _qrBaudRate, _parity, _dataBits);
                QR2Port.DataReceived += PortDataReceivedInternal;
                QR2Port.ErrorReceived += PortErrorReceivedInternal;

                OpenPort(InputPort);
                OpenPort(OutputPort);
                OpenPort(QR1Port);
                OpenPort(QR2Port);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При открытии портов возникла ошибка");
            }
        }

        private void OpenPort(SerialPort serialPort)
        {
            try
            {
                serialPort.Open();
                _logger.LogInformation("Порт {port} открыт", serialPort.PortName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При открытии порта {port} возникла ошибка", serialPort.PortName);
            }
        }

        private void PortDataReceivedInternal(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;
            try
            {
                Thread.Sleep(100);
                var data = port.ReadExisting();
                _logger.LogInformation("<- Получено ({port}): {data}", port.PortName, data);

                PortDataReceived?.Invoke(port, data);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Порт {port} не открыт", port.PortName);
                OpenPort(port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "При получении данных из порта {port} возникла ошибка", port.PortName);
            }
        }

        private void PortErrorReceivedInternal(object sender, SerialErrorReceivedEventArgs e)
        {
            if (sender is SerialPort port)
                _logger.LogError("От порта {port} получена ошибка | {eventType}", port.PortName, e.EventType);
            else
                _logger.LogError("Возникла непредвиденная ошибка | {eventType}", e.EventType);
        }

        public void SendHexResponse(SerialPort serialPort, byte data)
        {
            try
            {
                byte[] dataArray = [data];
                serialPort.Write(dataArray, 0, 1);

                _logger.LogInformation("-> Отправлено ({port}): 0x{hex}", serialPort.PortName, dataArray);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Порт {port} не открыт", serialPort.PortName);
                OpenPort(serialPort);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "При отправке ответа на порт {port} возникла ошибка", serialPort.PortName);
            }
        }
    }
}
