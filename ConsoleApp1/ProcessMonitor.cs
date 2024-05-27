using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class ProcessMonitor
    {

        private volatile bool _shouldStop;

        public void RequestStop()
        {
            _shouldStop = true;
        }

        public void MonitorProcess(string processName, int timeLimit, int interval, Func<bool> stopCondition)
        {
            if (string.IsNullOrEmpty(processName))
            {
                throw new ArgumentNullException(nameof(processName), "Process name cannot be null or empty.");
            }

            try
            {
                Console.WriteLine($"Monitoring process: {processName}");
                Console.WriteLine($"Maximum lifetime: {timeLimit} minutes");
                Console.WriteLine($"Check interval: {interval} minutes");

                var startTime = DateTime.Now;
                Process[] processes = Process.GetProcessesByName(processName);


                while (!_shouldStop)
                {
                    if (stopCondition())
                    {
                        Console.WriteLine("Stopping monitoring.");
                        break;
                    }

                    while (processes.Length == 0)
                    {

                        processes = Process.GetProcessesByName(processName);
                        startTime = DateTime.Now;

                    }

                    Process myProcess = processes[0];
                    var procTime = DateTime.Now - myProcess.StartTime;
                    Console.WriteLine($"Process time: {Math.Floor(procTime.TotalMinutes)} minutes");

                    var elapsedTime = DateTime.Now - startTime;
                    Console.WriteLine($"Elapsed time: {Math.Floor(elapsedTime.TotalMinutes)} minutes");

                    if (elapsedTime.TotalMinutes >= interval)
                    {
                        procTime = DateTime.Now - myProcess.StartTime;
                        if (procTime.TotalMinutes >= timeLimit)
                        {
                            myProcess.Kill();
                            Console.WriteLine("Process killed.");
                            Thread.Sleep(5000);
                            processes = Process.GetProcessesByName(processName);
                            startTime = DateTime.Now;
                            continue;
                        }
                    }
                    Thread.Sleep(interval * 60 * 1000);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
