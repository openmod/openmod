using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Rocket.Core.Utils
{
    public class TaskDispatcher : MonoBehaviour
    {
        private static int numThreads;
        private static bool awake = false;

        private static List<Action> actions = new List<Action>();
        private static List<DelayedQueueItem> delayed = new List<DelayedQueueItem>();

        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        public static void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (delayed)
                {
                    delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (actions)
                {
                    actions.Add(action);
                }
            }
        }

        public static Thread RunAsync(Action a)
        {
            while (numThreads >= 8)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch(Exception ex)
            {
                Logging.Logger.LogException(ex,"Error while running action");
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
        }

        private void Awake()
        {
            awake = true;
        }

        private void FixedUpdate()
        {
            if (!awake) return;

            List<Action> currentActions = new List<Action>();
            lock (actions)
            {
                currentActions.AddRange(actions);
                actions.Clear();
            }
            foreach (var a in currentActions)
            {
                a();
            }

            List<DelayedQueueItem> currentDelayed = new List<DelayedQueueItem>();
            lock (delayed)
            {
                currentDelayed.AddRange(delayed.Where(d => d.time <= Time.time));
                foreach (var item in currentDelayed)
                    delayed.Remove(item);
            }
            foreach (DelayedQueueItem item in currentDelayed)
            {
                item.action();
            }

        }
    }
}
