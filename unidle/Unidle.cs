using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;

namespace unidle
{
    public partial class ServiceUnidle : ServiceBase
    {
        private enum ExecutionState : uint
        {
            EsAwaymodeRequired = 0x00000040,
            EsContinuous = 0x80000000,
            EsDisplayRequired = 0x00000002,
            EsSystemRequired = 0x00000001,
            EsUserPresent = 0x00000004,
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        public ServiceUnidle()
        {
            InitializeComponent();
            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("UnidleSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource("UnidleSource", "UnidleLog");
            }
            eventLog.Source = "UnidleSource";
            eventLog.Log = "UnidleLog";
            Timer timer = new Timer
            {
                Interval = 60000 // 60 seconds
            };
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
            CanHandleSessionChangeEvent = true;
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, ++eventId);
        }

        private int eventId = 0;

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("In OnStart.");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop.");
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            EventLog.WriteEntry("SimpleService.OnSessionChange", DateTime.Now.ToLongTimeString() +
                " - Session change notice received: " +
                changeDescription.Reason.ToString() + "  Session ID: " +
                changeDescription.SessionId.ToString());
        }
    }
}
