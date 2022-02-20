//using System;
//using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Runtime.InteropServices;
//using Microsoft.Win32;

namespace unidle
{
    public partial class ServiceUnidle : ServiceBase
    {
        private enum ExecutionState : uint
        {
            ES_AWAYMODE_REQUIRED =0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_USER_PRESENT = 0x00000004,
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
            //timer = new Timer
            //{
            //    Interval = 60000 // 60 seconds
            //};
            //timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            //timer.Start();
            this.CanHandleSessionChangeEvent = true;
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            //eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, ++eventId);
        }

        //private int eventId = 0;

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Service started");
            EnableUnidling();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            eventLog.WriteEntry(changeDescription.Reason.ToString());
            if ((changeDescription.Reason == SessionChangeReason.SessionLock) || (changeDescription.Reason == SessionChangeReason.SessionLogoff))
            {
                DisableUnidling();
                //timer.Enabled = false;
            }
            else if ((changeDescription.Reason == SessionChangeReason.SessionUnlock) || (changeDescription.Reason == SessionChangeReason.SessionLogon))
            {
                EnableUnidling();
                //timer.Enabled = true;
            }
        }

        protected override void OnStop()
        {
            DisableUnidling();
            eventLog.WriteEntry("Service stopped");
        }

        public void EnableUnidling()
        {
            eventLog.WriteEntry("Unidling enabled");
            SetThreadExecutionState(ExecutionState.ES_CONTINUOUS | ExecutionState.ES_DISPLAY_REQUIRED);
        }

        public void DisableUnidling()
        {
            eventLog.WriteEntry("Unidling disabled");
            SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
        }

    }
}
