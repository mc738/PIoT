using PIoT.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIoT.Events
{
    public class LogEventArgs 
        : EventArgs
    {
        private readonly LogItem logItem;

        public LogItem LogItem => logItem;

        public LogEventArgs(LogItem logItem)
        {
            this.logItem = logItem;
        }
    }
}
