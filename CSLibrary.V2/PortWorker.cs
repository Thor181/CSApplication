using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace CSLibrary.V2
{
    public class PortWorker
    {
        /// <summary>
        /// Сигнал открытия от оператора
        /// </summary>
        public const int x01 = 0x01;

        /// <summary>
        /// Сигнал открытия от оператора
        /// </summary>
        public const int x02 = 0x02;

        /// <summary>
        /// Успех
        /// </summary>
        public const int x06 = 0x06;

        /// <summary>
        /// Сумма (параметр s) с QR-кода меньше N2 и меньше N1
        /// </summary>
        public const int x07 = 0x07;

        /// <summary>
        /// Сумма (параметр s) с QR-кода меньше либо равно N2
        /// </summary>
        public const int x08 = 0x08;
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
                InputPort.ReadTimeout = _options.PortInputReadTimeoutMs;

                OutputPort = new SerialPort(_options.PortOutputName, _baudRate, _parity, _dataBits);
                OutputPort.DataReceived += PortDataReceivedInternal;
                OutputPort.ErrorReceived += PortErrorReceivedInternal;
                OutputPort.ReadTimeout = _options.PortOutputReadTimeoutMs;

                QR1Port = new SerialPort(_options.PortQR1Name, _qrBaudRate, _parity, _dataBits);
                QR1Port.DataReceived += PortDataReceivedInternal;
                QR1Port.ErrorReceived += PortErrorReceivedInternal;
                QR1Port.ReadTimeout = _options.PortQR1ReadTimeoutMs;

                QR2Port = new SerialPort(_options.PortQR2Name, _qrBaudRate, _parity, _dataBits);
                QR2Port.DataReceived += PortDataReceivedInternal;
                QR2Port.ErrorReceived += PortErrorReceivedInternal;
                QR2Port.ReadTimeout = _options.PortQR2ReadTimeoutMs;

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
            var bytes = new List<byte>(8);
            try
            {
                int currentByte = -1;

                while ((currentByte = port.ReadByte()) != -1)
                {
                    bytes.Add((byte)currentByte);
                }
            }
            catch (TimeoutException)
            {
                _logger.LogWarning($"Timeout exceed for reading data from port '{port.PortName}'");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Порт {port} не открыт", port.PortName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "При получении данных из порта {port} возникла ошибка", port.PortName);
            }

            var data = Encoding.ASCII.GetString([.. bytes]);
            Debug.WriteLine("<- Data received: " + data);
            PortDataReceived?.Invoke(port, data);
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
