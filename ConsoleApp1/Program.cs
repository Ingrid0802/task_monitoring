using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: <processName> <timeLimit> <interval>");
                return;
            }

            string processName = args[0];
            int timeLimit = Int32.Parse(args[1]);
            int interval = Int32.Parse(args[2]);

            ProcessMonitor monitor = new ProcessMonitor();
            monitor.MonitorProcess(processName, timeLimit, interval, () => Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Q);
        }
    }
}