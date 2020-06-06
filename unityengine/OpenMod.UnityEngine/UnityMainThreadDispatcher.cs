/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.Core.Helpers;
using UnityEngine;

namespace OpenMod.UnityEngine
{
    /// Author: Pim de Witte (pimdewitte.com) and contributors, https://github.com/PimDeWitte/UnityMainThreadDispatcher
    /// Modified by: Enes Sadık Özbek (removed prefab requirement)
    /// <summary>
    /// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
    /// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
    /// </summary>
    public class UnityMainThreadDispatcher : MonoBehaviour
    {

        private static readonly Queue<Action> s_ExecutionQueue = new Queue<Action>();
        private static UnityMainThreadDispatcher s_Instance;
        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    GameObject go = new GameObject();
                    DontDestroyOnLoad(go);
                    s_Instance = go.AddComponent<UnityMainThreadDispatcher>();
                }

                return s_Instance;
            }
        }

        private void Update()
        {
            lock (s_ExecutionQueue)
            {
                while (s_ExecutionQueue.Count > 0)
                {
                    s_ExecutionQueue.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// Locks the queue and adds the IEnumerator to the queue
        /// </summary>
        /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
        public void Enqueue(IEnumerator action)
        {
            lock (s_ExecutionQueue)
            {
                s_ExecutionQueue.Enqueue(() => {
                    StartCoroutine(action);
                });
            }
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        public void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue, returning a Task which is completed when the action completes
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        /// <returns>A Task that can be awaited until the action completes</returns>
        public Task EnqueueAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }

        public void EnqueueTask(Task task)
        {
            Enqueue(() => AsyncHelper.RunSync(() => task));
        }

        private IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }

        private void OnDestroy()
        {
            s_Instance = null;
        }
    }
}