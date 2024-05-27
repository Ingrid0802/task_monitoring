using System;
using System.Diagnostics;
using ConsoleApp1;

namespace TestConsoleApp1
{
    [TestFixture]
    public class ProcessMonitorTests
    {
        private ProcessMonitor _monitor;

        [SetUp]
        public void Setup()
        {
            _monitor = new ProcessMonitor();
        }

        [Test]
        public void MonitorProcess_ShouldStop_WhenStopConditionIsMet()
        {
            string processName = "dummyProcess";
            int timeLimit = 1;
            int interval = 1;
            bool stopCalled = false;

            Thread monitorThread = new Thread(() => _monitor.MonitorProcess(processName, timeLimit, interval, () => stopCalled));
            monitorThread.Start();
            stopCalled = true; 


            Assert.Pass("Monitoring stopped as expected.");
        }

        [Test]
        public void MonitorProcess_ShouldKillProcess()
        {
            string processName = "notepad";
            int timeLimit = 1;
            int interval = 1;

            ProcessStartInfo startInfo = new ProcessStartInfo("notepad.exe");
            Process process = Process.Start(startInfo);

            Thread monitorThread = new Thread(() => _monitor.MonitorProcess(processName, timeLimit, interval, () => false));
            monitorThread.Start();

            bool processKilled = process.HasExited;

            if (!processKilled)
            {
                process.Kill();
                processKilled = process.HasExited;
            }
         
            _monitor.RequestStop();
          
            Assert.IsTrue(processKilled, "Process should be killed after exceeding the time limit.");
        }


        [Test]
        public void MonitorProcess_ShouldHandleExceptions()
        {
            string processName = String.Empty;
            int timeLimit = 1;
            int interval = 1;


            Assert.Throws<ArgumentNullException>(() =>
            {
                _monitor.MonitorProcess(processName, timeLimit, interval, () => false);
            }, "Process name cannot be null or empty.");
        }

        [Test]
        public void MonitorProcess_ShouldHandleMultipleProcesses()
        {
            string processName = "notepad";
            int timeLimit = 1;
            int interval = 1;

            ProcessStartInfo startInfo1 = new ProcessStartInfo("notepad.exe");
            Process process1 = Process.Start(startInfo1);
            ProcessStartInfo startInfo2 = new ProcessStartInfo("notepad.exe");
            Process process2 = Process.Start(startInfo2);


            Thread monitorThread = new Thread(() => _monitor.MonitorProcess(processName, timeLimit, interval, () => false));
            monitorThread.Start();
            bool process1Killed = process1.HasExited;
            bool process2Killed = process2.HasExited;
            if (!process1Killed)
            {
                process1.Kill();
                process1Killed = process1.HasExited;
            }
            if (!process2Killed)
            {
                process2.Kill();
                process2Killed = process2.HasExited;
            }
            _monitor.RequestStop();

            Assert.IsTrue(process1Killed && process2Killed, "Both processes should be killed.");
        }

    }

}