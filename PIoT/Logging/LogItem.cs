using System;
using System.Collections.Generic;
using System.Text;

namespace PIoT.Logging
{
    public class LogItem
    {
        private readonly string from;
        private readonly string summary;
        private readonly string details;
        private readonly LogItemTypes type;
        private readonly (string, string)[] data;
        private readonly DateTime time;

        public string From => from;

        public string Summary => summary;

        public string Details => details;

        public LogItemTypes Type => type;

        public DateTime Time => time;

        public LogItem(string from, string summary, string details, LogItemTypes type, params (string, string)[] data)
        {
            this.from = from;
            this.summary = summary;
            this.details = details;
            this.type = type;
            this.data = data;
            time = DateTime.Now;
        }

       
    }
}
