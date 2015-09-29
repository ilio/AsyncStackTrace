using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsyncStackTrace;

namespace Windows45
{
    class Program
    {
        static void Main()
        {
            
            do
            {
                switch (DisplayMenu())
                {
                    case 1:
                        ThrowStandardException().Wait();
                        break;
                    case 2:
                        ThrowAsyncStackTraceAsyncOnly().Wait();
                        break;
                    case 3:
                        ThrowAsyncStackTraceFull().Wait();
                        break;
                    case 4:
                        ThrowAsyncStackTraceOut().Wait();
                        break;
                    default:
                        Console.WriteLine("Inalid input. 1-5 expected.");
                        break;
                    case 5:
                        return;
                }

            } while (true);
        }

        private static async Task ThrowStandardException()
        {
            try
            {
                await Task.Delay(0);
                var nestedTask = Task.Factory.StartNew(() =>
                {
                    // create nested task throws exception
                    throw new ArgumentException("Example exception");
                });
                await nestedTask;
            }
            catch (ArgumentException ee)
            {
                Debug.WriteLine("Standard exception:\r\n" + ee);
                Console.WriteLine("Standard exception:\r\n" + ee);
                /**
output:
Standard exception:
System.ArgumentException: Example exception
   at Windows45.Program.<ThrowStandardException>b__0() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 47
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at Windows45.Program.<ThrowStandardException>d__2.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 49
                 */
            }
        }

        private static async Task ThrowAsyncStackTrace()
        {
            await Task.Delay(0).Trace(); // do nothing, just for example, you must add .Trace() after every awaited task.

            var nestedTask = Task.Factory.StartNew(() =>
            {
                // create nested task throws exception
                throw new ArgumentException("Example exception").Trace(); // Must trace exceptions too
            });

            await nestedTask.Trace();
        }

        private static async Task ThrowAsyncStackTraceAsyncOnly()
        {
            try
            {
                await ThrowAsyncStackTrace();
            }
            catch (Exception ee)
            {
                ee.Catch(delegate(ArgumentException exception)
                {
                    Debug.WriteLine("AsyncStackTrace exception:\r\n" + exception.GetAsyncTrace());
                    Console.WriteLine("AsyncStackTrace exception:\r\n" + exception.GetAsyncTrace());
                })
                .RethrowUnhandled();
                /*
output:
AsyncStackTrace exception:
ArgumentException: Example exception
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 68
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 71

                 */
            }
        }

        private static async Task ThrowAsyncStackTraceFull()
        {
            try
            {
                await ThrowAsyncStackTrace();
            }
            catch (Exception ee)
            {
                // extract ArgumentException from AggregateException
                ee.Catch(delegate(ArgumentException exception)
                {
                    Debug.WriteLine("AsyncStackTrace exception:\r\n" + exception.GetFullTrace());
                    Console.WriteLine("AsyncStackTrace exception:\r\n" + exception.GetFullTrace());
                    /*
output:
AsyncStackTrace exception:
System.ArgumentException: Example exception
   at Windows45.Program.<ThrowAsyncStackTrace>b__6() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 68
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__0.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\AsyncStackTrace\AsyncStackTraceExtension.cs:line 100
--- Async stack trace:
	   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 68
	   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 71
                     */
                })
                .RethrowUnhandled();

            }
        }

        private static async Task ThrowAsyncStackTraceOut()
        {
            try
            {
                await ThrowAsyncStackTrace();
            }
            catch (Exception ee)
            {
                // ee is AggregateException contains ArgumentException
                Debug.WriteLine("AsyncStackTrace exception:\r\n" + ee);
                Console.WriteLine("AsyncStackTrace exception:\r\n" + ee);
                /*
output:
AsyncStackTrace exception:
AsyncStackTrace.AggregateExceptionEx: Example exception ---> System.ArgumentException: Example exception
   at Windows45.Program.<ThrowAsyncStackTrace>b__6() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 68
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__0.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\AsyncStackTrace\AsyncStackTraceExtension.cs:line 100
   --- End of inner exception stack trace ---
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__0.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\AsyncStackTrace\AsyncStackTraceExtension.cs:line 104
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at Windows45.Program.<ThrowAsyncStackTrace>d__8.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 71
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at Windows45.Program.<ThrowAsyncStackTraceOut>d__16.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 121
---> (Inner Exception #0) System.ArgumentException: Example exception
   at Windows45.Program.<ThrowAsyncStackTrace>b__6() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 68
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__0.MoveNext() in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\AsyncStackTrace\AsyncStackTraceExtension.cs:line 100<---
----->
---> (Inner Exception #0) ArgumentException: Example exception
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 68
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\Windows\Windows45\Program.cs:line 71<---
<-----
                 */
            }
        }

        static public int DisplayMenu()
        {
            Console.WriteLine("Print stack trace for async task:");
            Console.WriteLine();
            Console.WriteLine("1. Standard");
            Console.WriteLine("2. Async trace only");
            Console.WriteLine("3. Full stack trace (standard + async)");
            Console.WriteLine("4. Stack for unhandled exception");
            Console.WriteLine("5. Exit");
            Console.WriteLine();
            Console.WriteLine("Press 1-5");
            var result = Console.ReadLine();
            int value;
            Int32.TryParse(result, out value);
            return value;
        }
    }
}
