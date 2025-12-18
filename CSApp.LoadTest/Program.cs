using System.IO.Ports;

namespace CSApp.LoadTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to go...");
            Console.ReadLine();
            SerialSender.SendHexToSerialPort("COM2", 9200, "3632443232363841", 500, 500);
        }
    }

    public class SerialSender
    {
        /// <summary>
        /// Отправляет HEX-данные указанное количество раз в заданный последовательный порт.
        /// </summary>
        /// <param name="portName">Имя COM-порта (например, "COM3")</param>
        /// <param name="baudRate">Скорость передачи данных (например, 9600)</param>
        /// <param name="hexString">HEX-строка без пробелов (например, "3632443232363841")</param>
        /// <param name="repeatCount">Количество повторений отправки</param>
        /// <param name="delayMs">Задержка между отправками в миллисекундах (опционально)</param>
        public static void SendHexToSerialPort(string portName, int baudRate, string hexString, int repeatCount, int delayMs = 0)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                throw new ArgumentException("HEX-строка не может быть пустой.", nameof(hexString));

            if (repeatCount <= 0)
                throw new ArgumentException("Количество повторений должно быть больше нуля.", nameof(repeatCount));

            // Убираем все не-HEX символы (пробелы, табуляции и т.п.)
            hexString = System.Text.RegularExpressions.Regex.Replace(hexString, @"[^0-9A-Fa-f]", "");

            if (hexString.Length % 2 != 0)
                throw new ArgumentException("HEX-строка должна содержать чётное количество символов.", nameof(hexString));

            byte[] data = new byte[hexString.Length / 2];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            using (SerialPort port = new SerialPort(portName, baudRate))
            {
                try
                {
                    port.Open();
                    
                    for (int i = 0; i < repeatCount; i++)
                    {
                        Console.WriteLine($"[{i+1}/{repeatCount}]");
                        port.Write(data, 0, data.Length);
                        if (delayMs > 0)
                            Thread.Sleep(delayMs);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при работе с портом {portName}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
