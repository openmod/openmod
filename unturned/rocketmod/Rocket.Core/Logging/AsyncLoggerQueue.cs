using Rocket.Core.RCON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Rocket.Core.Logging
{
    public class AsyncLoggerQueue
    {
        public static AsyncLoggerQueue Current = new AsyncLoggerQueue();

        private static readonly object logEntryQueueLock = new object();

        private Queue<LogEntry> logEntryQueue = new Queue<LogEntry>();
        private BackgroundWorker logger = new BackgroundWorker();

        private AsyncLoggerQueue()
        {
            //configure background worker
            logger.WorkerSupportsCancellation = false;
            logger.DoWork += new DoWorkEventHandler(_Logger_DoWork);
        }

        public void Enqueue(LogEntry le)
        {
            //lock during write
            lock (logEntryQueueLock)
            {
                logEntryQueue.Enqueue(le);

                //while locked check to see if the BW is running, if not start it
                if (!logger.IsBusy)
                    logger.RunWorkerAsync();
            }
        }

        private void _Logger_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                LogEntry le = null;

                bool skipEmptyCheck = false;
                lock (logEntryQueueLock)
                {
                    if (logEntryQueue.Count <= 0) //if queue is empty than BW is done
                        return;
                    else if (logEntryQueue.Count > 1) //if greater than 1 we can skip checking to see if anything has been enqueued during the logging operation
                        skipEmptyCheck = true;

                    //dequeue the LogEntry that will be written to the log
                    le = logEntryQueue.Dequeue();
                }

                processLog(le);

                if (skipEmptyCheck) //if LogEntryQueue.Count was > 1 before we wrote the last LogEntry we know to continue without double checking
                {
                    lock (logEntryQueueLock)
                    {
                        if (logEntryQueue.Count <= 0) //if queue is still empty than BW is done
                            return;
                    }
                }
            }
        }
        private void processLog(LogEntry entry) {
            StreamWriter streamWriter = File.AppendText(Path.Combine(Environment.LogsDirectory, Environment.LogFile));
            streamWriter.WriteLine("[" + DateTime.Now + "] [" + entry.Severity.ToString() + "] " + entry.Message);
            streamWriter.Close();
            if (entry.RCON && R.Settings != null && R.Settings.Instance.RCON.Enabled)
            {
                RCONServer.Broadcast(entry.Message);
            }
        }
    }
}
