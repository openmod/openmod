using Microsoft.Extensions.Logging;
using SDG.Unturned;
using System;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OpenMod.Unturned.Logging
{
    public class SerilogWindowsConsoleInputOutput : WindowsConsoleInputOutput
    {
        private readonly ILogger m_Logger;

        public SerilogWindowsConsoleInputOutput(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger("SDG.Unturned");
        }

        public override void outputInformation(string information)
        {
            CheckCursor(information.Split('\n').Length);
            m_Logger.LogInformation(information);
            System.Console.CursorTop++;
            System.Console.CursorLeft = 0;
        }

        public override void outputWarning(string warning)
        {
            CheckCursor(warning.Split('\n').Length);
            m_Logger.LogWarning(warning);
            System.Console.CursorTop++;
            System.Console.CursorLeft = 0;
        }

        public override void outputError(string error)
        {
            CheckCursor(error.Split('\n').Length);
            m_Logger.LogError(error);
            System.Console.CursorTop++;
            System.Console.CursorLeft = 0;
        }

        public void CheckCursor(int lineCount)
        {
            if (System.Console.CursorLeft != 0)
            {
                System.Console.CursorTop += lineCount;
                System.Console.CursorLeft = 0;
            }
        }

        protected override void onConsoleInputKey(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    onConsoleInputEnter();
                    return;
                case ConsoleKey.Escape:
                    onConsoleInputEscape();
                    return;
                case ConsoleKey.Backspace:
                    onConsoleInputBackspace();
                    return;
                default:
                    var character = keyInfo.KeyChar;

                    if (character == '\0') return;

                    pendingInput += character;

                    System.Console.CursorLeft = 0;
                    System.Console.Write(pendingInput + new string(' ', System.Console.BufferWidth - pendingInput.Length));
                    System.Console.CursorLeft = pendingInput.Length;
                    return;
            }
        }

        protected override void onConsoleInputEnter()
        {
            if (pendingInput.Length == 0) return;

            ClearLine();
            m_Logger.LogInformation($">{pendingInput}");
            notifyInputCommitted(pendingInput);

            pendingInput = "";
        }

        protected override void onConsoleInputBackspace()
        {
            if (pendingInput.Length == 0) return;

            pendingInput = pendingInput.Remove(pendingInput.Length - 1);
            System.Console.Write("\b \b");
        }

        public void ClearLine()
        {
            System.Console.CursorLeft = 0;
            System.Console.Write(new string(' ', System.Console.BufferWidth));
            System.Console.CursorLeft = 0;
        }
    }
}