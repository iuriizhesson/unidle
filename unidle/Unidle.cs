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

namespace unidle
{
    public partial class ServiceUnidle : ServiceBase
    {
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
