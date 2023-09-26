using CSLibrary;
using CSLibrary.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSApplication
{
    public class MainModule
    {
        public static LoggerConsole LoggerInternal;

        public async Task Main()
        {
            LoggerInternal = new LoggerConsole();

            AppConfig.Instance.Initialize();

            var t = Task.Run(() =>
            {
                PortWorker worker = new PortWorker();
                worker.OpenInputPort();

            });

            Console.Read();
            await t;
        }
    }
}
