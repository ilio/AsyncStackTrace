using System;
using System.Collections.Generic;
using System.Globalization;

namespace AsyncStackTrace
{
    public sealed class AggregateExceptionEx : AggregateException
    {
        public AggregateExceptionEx(
            string message,
            IEnumerable<Exception> exceptions)
            : base(message, exceptions)
        {
        }

        public override string ToString()
        {
            var text = base.ToString();

            return text + "----->" + StackTrace + "<-----";
        }

        private static string RestoreStackTrace(Exception e)
        {
            var innerException = e.InnerException;
            string iex = "";
            if (innerException != null)
            {
                iex = " --->" +
                      " " + RestoreStackTrace(innerException) + Environment.NewLine +
                      "   " + "--- End of inner exception stack trace ---";
            }
            var trace = e.Data["_AsyncStackTrace"] as LinkedList<string>;
            string prefix = e.GetType().Name + ": " + e.Message + iex + "\r\n";
            if (trace == null)
            {
                return prefix + e.StackTrace;
            }
            var traceLines = string.Join("\r\n", trace);
            return prefix + traceLines;

        }
        public override string StackTrace
        {
            get
            {
                var text = "";
                for (int i = 0; i < InnerExceptions.Count; i++)
                {
                    text = String.Format(
                        CultureInfo.InvariantCulture,
                        "{0}{1}---> (Inner Exception #{2}) {3}{4}{5}",
                        text, Environment.NewLine, i, RestoreStackTrace(InnerExceptions[i]), "<---", Environment.NewLine);
                }
                return text;
            }
        }
    }
}