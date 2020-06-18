using Rocket.Core.Logging;
using System;
using UnityEngine;

namespace Rocket.Core.Utils
{
    internal class AutomaticShutdownWatchdog : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (started)
            {
                checkTimerRestart();
            }
        }

        private DateTime? shutdownTime = null;
        private bool shutdown = false;
        public static AutomaticShutdownWatchdog Instance;
        private bool started = false;
        private DateTime startedTime = DateTime.Now;

        private void Start()
        {
            Instance = this;
            if (R.Settings.Instance.AutomaticShutdown.Enabled)
            {
                shutdownTime = startedTime.ToUniversalTime().AddSeconds(R.Settings.Instance.AutomaticShutdown.Interval);
                Logging.Logger.Log("This server will automatically shutdown in " + R.Settings.Instance.AutomaticShutdown.Interval + " seconds (" + shutdownTime.ToString() + " UTC)");
            }
            started = true;
        }

        private void checkTimerRestart()
        {
            try
            {
                if (shutdownTime != null)
                {
                    if ((shutdownTime.Value - DateTime.UtcNow).TotalSeconds < 0 && !shutdown)
                    {
                        shutdown = true;
                        R.Implementation.Shutdown();
                    }
                }
            }
            catch (Exception er)
            {
                Logging.Logger.LogException(er);
            }
        }
    }
}
