using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using UnityEngine;

namespace Rocket.Unturned.Utils
{
    internal class AutomaticSaveWatchdog : MonoBehaviour
    {
        private void Update()
        {
            checkTimer();
        }

        private DateTime? nextSaveTime = null;
        public static AutomaticSaveWatchdog Instance;
        private int interval = 30;

        private void Start()
        {
            Instance = this;
            if (U.Settings.Instance.AutomaticSave.Enabled)
            {
                if(U.Settings.Instance.AutomaticSave.Interval < interval)
                {
                    Core.Logging.Logger.LogError("AutomaticSave interval must be atleast 30 seconds, changed to 30 seconds");
                }
                else
                {
                    interval = U.Settings.Instance.AutomaticSave.Interval;
                }
                Core.Logging.Logger.Log(String.Format("This server will automatically save every {0} seconds", interval));
                restartTimer();
            }
        }

        private void restartTimer ()
        {
            nextSaveTime = DateTime.Now.AddSeconds(interval);
        }

        private void checkTimer()
        {
            try
            {
                if (nextSaveTime != null)
                {
                    if (nextSaveTime.Value < DateTime.Now)
                    {
                        Core.Logging.Logger.Log("Saving server");
                        restartTimer();
                        SaveManager.save();
                    }
                }
            }
            catch (Exception er)
            {
                Core.Logging.Logger.LogException(er);
            }
        }
    }
}
