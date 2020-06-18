using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Logging
{
    public class LogEntry
    {
        public ELogType Severity;
        public string Message;
        public bool RCON;
    }
}
