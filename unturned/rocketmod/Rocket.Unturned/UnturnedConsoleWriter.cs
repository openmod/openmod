using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rocket.Unturned
{
    [Obsolete("Refer to usage of built-in ICommandInputOutput for handling of custom console/terminal.")]
    public class UnturnedConsoleWriter : TextWriter
    {
        private TextWriter _consoleOutput;
        private TextWriter _consoleError;

        private StreamWriter _streamWriter;

        public UnturnedConsoleWriter(StreamWriter streamWriter)
        {
            this._streamWriter = streamWriter;
            _consoleOutput = Console.Out;
            _consoleError = Console.Error;

            Console.SetOut(this);
            Console.SetError(this);
        }

        public override Encoding Encoding { get { return _consoleOutput.Encoding; } }
        public override IFormatProvider FormatProvider { get { return _consoleOutput.FormatProvider; } }
        public override string NewLine { get { return _consoleOutput.NewLine; } set { _consoleOutput.NewLine = value; } }

        public override void Close()
        {
            _consoleOutput.Close();
            _streamWriter.Close();
        }

        public override void Flush()
        {
            _consoleOutput.Flush();
            _streamWriter.Flush();
        }

        public override void Write(double value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }
        public override void Write(string value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(object value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(decimal value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(float value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(bool value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(int value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(uint value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(ulong value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(long value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(char[] buffer)
        {
            _consoleOutput.Write(buffer);
            _streamWriter.Write(buffer);

        }

        public override void Write(char value)
        {
            _consoleOutput.Write(value);
            _streamWriter.Write(value);

        }

        public override void Write(string format, params object[] arg)
        {
            _consoleOutput.Write(format, arg);
            _streamWriter.Write(format, arg);

        }

        public override void Write(string format, object arg0)
        {
            _consoleOutput.Write(format, arg0);
            _streamWriter.Write(format, arg0);

        }

        public override void Write(string format, object arg0, object arg1)
        {
            _consoleOutput.Write(format, arg0, arg1);
            _streamWriter.Write(format, arg0, arg1);

        }

        public override void Write(char[] buffer, int index, int count)
        {
            _consoleOutput.Write(buffer, index, count);
            _streamWriter.Write(buffer, index, count);

        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            _consoleOutput.Write(format, arg0, arg1, arg2);
            _streamWriter.Write(format, arg0, arg1, arg2);

        }

        public override void WriteLine()
        {
            _consoleOutput.WriteLine();
            _streamWriter.WriteLine();

        }

        public override void WriteLine(double value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(decimal value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(string value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(object value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(float value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(bool value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(uint value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(long value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(ulong value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(int value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(char[] buffer)
        {
            _consoleOutput.WriteLine(buffer);
            _streamWriter.WriteLine(buffer);

        }
        public override void WriteLine(char value)
        {
            _consoleOutput.WriteLine(value);
            _streamWriter.WriteLine(value);

        }
        public override void WriteLine(string format, params object[] arg)
        {
            _consoleOutput.WriteLine(format, arg);
            _streamWriter.WriteLine(format, arg);

        }
        public override void WriteLine(string format, object arg0)
        {
            _consoleOutput.WriteLine(format, arg0);
            _streamWriter.WriteLine(format, arg0);

        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            _consoleOutput.WriteLine(format, arg0, arg1);
            _streamWriter.WriteLine(format, arg0, arg1);

        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            _consoleOutput.WriteLine(buffer, index, count);
            _streamWriter.WriteLine(buffer, index, count);

        }
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            _consoleOutput.WriteLine(format, arg0, arg1, arg2);
            _streamWriter.WriteLine(format, arg0, arg1, arg2);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Console.SetOut(_consoleOutput);
                Console.SetError(_consoleError);
            }
        }
    }
}
