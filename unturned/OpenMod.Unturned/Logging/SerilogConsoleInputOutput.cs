using Microsoft.Extensions.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OpenMod.Unturned.Logging
{
    public class SerilogConsoleInputOutput : ConsoleInputOutput
    {
        private readonly ILogger m_Logger;

        private readonly List<string> m_InputHistoric;
        private int m_HistoricIndex;

        private int m_InputIndex;
        private bool m_IsInsert;
        private bool m_IsLogging;

        public SerilogConsoleInputOutput(ILoggerFactory loggerFactory)
        {
            m_InputHistoric = new List<string>();
            m_Logger = loggerFactory.CreateLogger("SDG.Unturned");
        }

        private string GetCurrentInput()
        {
            return m_HistoricIndex == m_InputHistoric.Count ? pendingInput : m_InputHistoric[m_HistoricIndex];
        }

        #region Output 

        public override void outputInformation(string information)
        {
            Log(information, ELogType.Info);
        }

        public override void outputWarning(string warning)
        {
            Log(warning, ELogType.Warn);
        }

        public override void outputError(string error)
        {
            Log(error, ELogType.Error);
        }

        private void Log(string message, ELogType logType)
        {
            m_IsLogging = true;
            ClearLine();

            switch (logType)
            {
                case ELogType.Info:
                    m_Logger.LogInformation(message);
                    break;

                case ELogType.Warn:
                    m_Logger.LogWarning(message);
                    break;

                case ELogType.Error:
                    m_Logger.LogError(message);
                    break;

                default:
                    var exception = new ArgumentOutOfRangeException(nameof(logType), logType, null);
                    m_Logger.LogError(exception.Message);
                    break;
            }

            PosLog(message);
            m_IsLogging = false;
        }

        #endregion

        #region Keys

        protected override void onConsoleInputKey(ConsoleKeyInfo keyInfo)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (keyInfo.Key)
            {
                /*case ConsoleKey.Tab:
                    //AutoComplete here maybe
                    return;*/

                case ConsoleKey.Backspace:
                    onConsoleInputBackspace();
                    return;

                case ConsoleKey.Delete:
                    OnConsoleInputDelete();
                    return;

                case ConsoleKey.Enter:
                    onConsoleInputEnter();
                    return;

                case ConsoleKey.Escape:
                    onConsoleInputEscape();
                    return;

                case ConsoleKey.DownArrow:
                    DownArrow();
                    return;

                case ConsoleKey.LeftArrow:
                    LeftArrow();
                    return;

                case ConsoleKey.RightArrow:
                    RightArrow();
                    return;

                case ConsoleKey.UpArrow:
                    UpArrow();
                    return;

                case ConsoleKey.Insert:
                    m_IsInsert = !m_IsInsert;
                    break;

                default:
                    var character = keyInfo.KeyChar;
                    if (character == '\0' || character == '\t')//Console does not work correctly with \t
                        return;

                    var input = GetCurrentInput();
                    m_HistoricIndex = m_InputHistoric.Count;
                    var haveInput = input.Length != 0;

                    if (m_IsInsert && haveInput && m_InputIndex < input.Length)
                    {
                        input = input.Remove(m_InputIndex, 1);
                    }

                    pendingInput = input.Insert(m_InputIndex, character.ToString()).TrimStart();
                    if (haveInput)
                    {
                        ClearLine();
                        if (pendingInput.Length == 0)
                        {
                            m_InputIndex = 0;
                            return;
                        }
                    }
                    else if (pendingInput.Length == 0)
                        return;

                    if (input.Length != pendingInput.Length) //Pointer fix cause by trim
                        m_InputIndex++;

                    if (m_IsLogging)
                        return;

                    Rewrite(pendingInput);
                    return;
            }
        }

        protected override void onConsoleInputBackspace()
        {
            var input = GetCurrentInput();
            if (input.Length == 0 || m_InputIndex <= 0)
                return;

            m_HistoricIndex = m_InputHistoric.Count;
            pendingInput = input.Remove(--m_InputIndex, 1);
            Rewrite(pendingInput);
        }

        private void OnConsoleInputDelete()
        {
            var input = GetCurrentInput();
            if (input.Length == 0 || m_InputIndex >= input.Length)
                return;

            m_HistoricIndex = m_InputHistoric.Count;
            pendingInput = input.Remove(m_InputIndex, 1);
            Rewrite(pendingInput);
        }

        protected override void onConsoleInputEnter()
        {
            if (m_IsLogging)
                return;

            var input = GetCurrentInput();
            m_InputIndex = 0;
            ClearLine();

            if (input.Length == 0)
            {
                System.Console.Write("> ");
                System.Console.CursorLeft = 0;
                System.Console.CursorTop++;
                return;
            }

            pendingInput = string.Empty;
            outputInformation($"> {input}");
            notifyInputCommitted(input);

            if (m_InputHistoric.Count == 0 || !m_InputHistoric.Last().Equals(input))
            {
                m_InputHistoric.Add(input);
            }

            var excess = m_InputHistoric.Count - 15;
            if (excess > 0)
                m_InputHistoric.RemoveRange(0, excess);

            m_HistoricIndex = m_InputHistoric.Count;
        }

        protected override void onConsoleInputEscape()
        {
            var input = GetCurrentInput();
            m_HistoricIndex = m_InputHistoric.Count;

            m_InputIndex = 0;
            pendingInput = string.Empty;
            if (input.Length == 0)
                return;

            ClearLine();
        }

        #endregion

        #region ConsoleLogic

        public void ClearLine()
        {
            System.Console.CursorLeft = 0;
            System.Console.Write(new string(' ', System.Console.BufferWidth));
            System.Console.CursorLeft = 0;
        }

        public void PosLog(string message)
        {
            System.Console.CursorTop += message.Split('\n').Length;

            var input = GetCurrentInput();
            Rewrite(input);
        }

        private void Rewrite(string input)
        {
            ClearLine();
            if (input.Length == 0)
                return;

            System.Console.Write($"> {input}");
            System.Console.CursorLeft = m_InputIndex + 2; // +1 to compense '> '
        }

        #endregion

        #region Arrows

        private void DownArrow()
        {
            if (m_IsLogging || m_HistoricIndex == m_InputHistoric.Count)
                return;

            m_HistoricIndex++;
            var input = GetCurrentInput();
            m_InputIndex = input.Length;
            Rewrite(input);
        }

        private void LeftArrow()
        {
            if (m_IsLogging || m_InputIndex <= 0)
                return;

            System.Console.CursorLeft = m_InputIndex--;
        }

        private void RightArrow()
        {
            var input = GetCurrentInput();
            if (m_IsLogging || input.Length == 0 || m_InputIndex >= input.Length)
                return;

            System.Console.CursorLeft = ++m_InputIndex + 1;
        }

        private void UpArrow()
        {
            if (m_IsLogging || m_InputHistoric.Count == 0 || m_HistoricIndex == 0)
                return;

            m_HistoricIndex--;
            var input = GetCurrentInput();
            m_InputIndex = input.Length;
            Rewrite(input);
        }

        #endregion
    }
}