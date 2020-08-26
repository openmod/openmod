using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using SDG.Unturned;

namespace OpenMod.Unturned.Logging
{
    [OpenModInternal]
    public class OpenModConsoleInputOutput : ICommandInputOutput
    {
        public event CommandInputHandler inputCommitted;

        private readonly IAutoCompleteHandler m_AutoCompleteHandler;
        private readonly ConcurrentQueue<string> m_CommandQueue;
        private readonly ILogger m_Logger;
        private Thread m_InputThread;
        private bool m_IsAlive;
        private TextReader m_PreviousConsoleIn;

        public OpenModConsoleInputOutput(
            ILoggerFactory loggerFactory,
            IAutoCompleteHandler autoCompleteHandler)
        {
            m_CommandQueue = new ConcurrentQueue<string>();
            m_AutoCompleteHandler = autoCompleteHandler;
            m_Logger = loggerFactory.CreateLogger("SDG.Unturned");
        }

        public void initialize(CommandWindow commandWindow)
        {
            if (m_IsAlive)
            {
                return;
            }

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            
            System.Console.OutputEncoding = encoding;
            System.Console.InputEncoding = encoding;

            m_PreviousConsoleIn = System.Console.In;
            System.Console.SetIn(new StreamReader(System.Console.OpenStandardInput()));

            ReadLine.HistoryEnabled = true;
            ReadLine.AutoCompletionHandler = m_AutoCompleteHandler;

            m_IsAlive = true;
            m_InputThread = new Thread(OnInputThreadStart)
            {
                IsBackground = true
            };

            m_InputThread.Start();
        }

        public void shutdown(CommandWindow commandWindow)
        {
            if (!m_IsAlive)
            {
                return;
            }

            m_IsAlive = false;
            ReadLine.AutoCompletionHandler = null;
            ReadLine.HistoryEnabled = false;
            System.Console.SetIn(m_PreviousConsoleIn);
        }

        public void update()
        {
            while (m_CommandQueue.TryDequeue(out var command))
            {
                inputCommitted?.Invoke(command); /* notify Unturned about inputted command */
            }
        }

        private void OnInputThreadStart()
        {
            while (m_IsAlive)
            {
                if (System.Console.KeyAvailable)
                {
                    string command = ReadLine.Read();
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        m_CommandQueue.Enqueue(command); /* Enqueue command because inputCommitted is expected on main thread */
                    }
                }

                Thread.Sleep(10);
            }
        }

        public void outputInformation(string information)
        {
            m_Logger.LogInformation(information);
        }

        public void outputWarning(string warning)
        {
            m_Logger.LogWarning(warning);
        }

        public void outputError(string error)
        {
            m_Logger.LogError(error);
        }
    }
}