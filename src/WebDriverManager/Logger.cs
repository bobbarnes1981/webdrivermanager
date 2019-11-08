using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDriverManager
{
    public interface ILogger
    {
        void Trace(string format, params object[] parameters);
        void Warn(string format, params object[] parameters);
        void Debug(string format, params object[] parameters);
        void Info(string format, params object[] parameters);
        void Error(string format, params object[] parameters);
        bool IsDebugEnabled();
        bool IsTraceEnabled();
    }
    public class Logger : ILogger
    {
        public static ILogger GetLogger()
        {
            return new Logger();
        }
        private void internalWrite(string level, string format, params object[] parameters)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("[{0}] {1}", level, string.Format(format, parameters)));
        }
        public void Trace(string format, params object[] parameters)
        {
            internalWrite("Trace", format, parameters);
        }
        public void Warn(string format, params object[] parameters)
        {
            internalWrite("Warn", format, parameters);
        }
        public void Debug(string format, params object[] parameters)
        {
            internalWrite("Debug", format, parameters);
        }
        public void Info(string format, params object[] parameters)
        {
            internalWrite("Info", format, parameters);
        }
        public void Error(string format, params object[] parameters)
        {
            internalWrite("Error", format, parameters);
        }
        public bool IsDebugEnabled()
        {
            return true;
        }
        public bool IsTraceEnabled()
        {
            return true;
        }
    }
}
