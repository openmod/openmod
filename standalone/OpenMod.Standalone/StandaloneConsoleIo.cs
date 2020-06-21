using System;
using System.Diagnostics;
using System.Drawing;

namespace OpenMod.Standalone
{
    public static class StandaloneConsoleIo
    {
        public delegate void CommandExecute(string commandLine);
        public static event CommandExecute OnCommandExecute;
        private static volatile bool s_IsRunning;

        public static void StartListening()
        {
            s_IsRunning = true;
            do
            {
                var line = Console.ReadLine().Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.Equals("exit", StringComparison.OrdinalIgnoreCase))
                        break;

                    try
                    {
                        OnCommandExecute?.Invoke(line);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString(), Color.DarkRed);
                        Debugger.Break();
                    }
                }


                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.White;
            } while (s_IsRunning);
        }

        public static void StopListening()
        {
            s_IsRunning = false;
        }
    }
}