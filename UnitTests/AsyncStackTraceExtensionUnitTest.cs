using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AsyncStackTrace;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestExceptionsAsync()
        {

            try
            {
                await Level1(() => InnerTask1().Trace());
            }
            catch (Exception e)
            {
                e.Catch((IOException ex) =>
                {
                    Debug.WriteLine("--------------------------------");
                    Debug.WriteLine("Full IOException => " + ex.GetFullTrace());
                    Debug.WriteLine("--------------------------------");
                    Debug.WriteLine("Async IOException => " + ex.GetAsyncTrace());
                })
                .Catch((TimeoutException ex) =>
                {
                    Debug.WriteLine("--------------------------------");
                    Debug.WriteLine("TimeoutException => " + ex.GetAsyncTrace());
                })
                .Catch((NotImplementedException ex) =>
                {
                    Debug.WriteLine("--------------------------------");
                    Debug.WriteLine("NotImplementedException => " + ex.GetAsyncTrace());
                })
                .Catch((AggregateException ex) =>
                {
                    Debug.WriteLine("--------------------------------");
                    Debug.WriteLine("rest => " + ex.GetAsyncTrace());
                })
                .RethrowUnhandled();

            }
        }

        private async Task InnerTask1()
        {
            await InnerTask2().Trace();
        }

        private async Task InnerTask2()
        {
            await InnerTask3().Trace();
        }

        private async Task InnerTask3()
        {
            // create a collection container to hold exceptions
            List<Exception> exceptions = new List<Exception>();

            // do some stuff here ........

            // we have an exception with an innerexception, so add it to the list
            exceptions.Add(new TimeoutException("It timed out", new ArgumentException("ID missing").Trace()).Trace());

            // do more stuff .....

            // Another exception, add to list
            exceptions.Add(new NotImplementedException("Somethings not implemented").Trace());
            try
            {
                InnerTask4();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            // all done, now create the AggregateException and throw it
            var aggEx = new AggregateException(exceptions).Trace();
            throw aggEx;

        }

        private void InnerTask4()
        {
            throw new IOException("not available", new ArgumentException("file not found1", new ArgumentException("file not found2")
                    .Trace())
                    .Trace())
                    .Trace();
        }

        private T Level3<T>(Func<T> action)
        {
            return action();
        }
        private T Level2<T>(Func<T> action)
        {
            return Level3(action);
        }
        private T Level1<T>(Func<T> action)
        {
            return Level2(action);
        }
    }
}
