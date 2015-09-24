using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
#if SUPPORT_ASYNC_INFO
using Windows.Foundation;
#endif

namespace AsyncStackTrace
{
    public static class AsyncStackTraceExtension
    {
#if SUPPORT_ASYNC_INFO
        public static async Task Trace(
            this IAsyncAction asyncOperation,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                await asyncOperation;
            }
            catch (Exception e)
            {
                throw AddAsyncStackTrace(e, callerMemberName, callerFilePath, callerLineNumber);
            }
        }


        public static async Task<TResult> Trace<TResult, TProgress>(
            this IAsyncOperationWithProgress<TResult, TProgress> asyncOperation,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                return await asyncOperation;
            }
            catch (Exception e)
            {
                throw AddAsyncStackTrace(e, callerMemberName, callerFilePath, callerLineNumber);
            }
        }

        public static async Task Trace<TProgress>(
            this IAsyncActionWithProgress<TProgress> asyncOperation,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                await asyncOperation;
            }
            catch (Exception e)
            {
                throw AddAsyncStackTrace(e, callerMemberName, callerFilePath, callerLineNumber);
            }
        }


        public static async Task<T> Trace<T>(
                    this IAsyncOperation<T> asyncOperation,
                    [CallerMemberName] string callerMemberName = null,
                    [CallerFilePath] string callerFilePath = null,
                    [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                return await asyncOperation;
            }
            catch (Exception e)
            {
                throw AddAsyncStackTrace(e, callerMemberName, callerFilePath, callerLineNumber);
            }
        }

#endif

        /// <summary>
        /// Store stack information for untyped task
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public static async Task Trace(
            this Task task,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                throw AddAsyncStackTrace(ex, callerMemberName, callerFilePath, callerLineNumber);
            }
        }

        /// <summary>
        /// Store stack trace for typed task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public static async Task<T> Trace<T>(
            this Task<T> task,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                throw AddAsyncStackTrace(ex, callerMemberName, callerFilePath, callerLineNumber);
            }
        }

        /// <summary>
        /// Store stack trace for exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exception"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Exception Trace<T>(this T exception,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0) where T : Exception
        {
            StoreStackTrace(exception, callerMemberName, callerFilePath, callerLineNumber);
            return exception;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Exception AddAsyncStackTrace(
            Exception ex,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber)
        {
            var exceptions = StoreStackTrace(ex, callerMemberName, callerFilePath, callerLineNumber);

            var result = new AggregateExceptionEx(
                ex.Message,
                exceptions);

            return result;
        }

        /// <summary>
        /// Store stack trace information inside the exception (ex.Data["_AsyncStackTrace"])
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        private static IEnumerable<Exception> StoreStackTrace(Exception ex, string callerMemberName, string callerFilePath,
            int callerLineNumber)
        {
            var aggregateException = ex as AggregateException;
            IEnumerable<Exception> exceptions;
            if (aggregateException != null)
            {
                var exception = aggregateException.Flatten();
                exceptions = exception.InnerExceptions;
            }
            else
            {
                exceptions = new[] { ex };
            }

            foreach (var inner in exceptions)
            {
                var trace = inner.Data["_AsyncStackTrace"] as LinkedList<string> ?? new LinkedList<string>();
                trace.AddLast(string.Format("   at {0} in {1}:line {2}", callerMemberName, callerFilePath, callerLineNumber));
                inner.Data["_AsyncStackTrace"] = trace;
            }
            return exceptions;
        }

        /// <summary>
        /// Catch only exceptions of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static Exception Catch<T>(this Exception e, Action<T> handler) where T : Exception
        {
            var aggregateException = e as AggregateException;
            if (aggregateException != null)
            {
                var innerExceptions = aggregateException.InnerExceptions;
                var unhandled = new List<Exception>(innerExceptions.Count);
                foreach (var ex in innerExceptions)
                {
                    var exception = ex as T;
                    if (exception == null)
                    {
                        unhandled.Add(ex);
                        continue;
                    }
                    handler(exception);
                }
                if (unhandled.Count > 0)
                {
                    return new AggregateExceptionEx(e.Message, unhandled);
                }
                return null;
            }

            {
                var exception = e as T;
                if (exception == null)
                {
                    return e;
                }
                handler(exception);
                return null;
            }

        }

        /// <summary>
        /// Handle unhandled exceptions
        /// </summary>
        /// <param name="e"></param>
        /// <param name="handler"></param>
        public static void Unhandled(this Exception e, Action<Exception> handler)
        {
            if (e == null)
            {
                return;
            }
            handler(e);
        }

        /// <summary>
        /// Rethrow unhandled exceptions
        /// </summary>
        /// <param name="e"></param>
        public static void RethrowUnhandled(this Exception e)
        {
            if (e == null)
            {
                return;
            }
            ExceptionDispatchInfo.Capture(e).Throw();
        }

        /// <summary>
        /// Only async stack trace
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAsyncTrace(this Exception ex)
        {
            var exception = ex as AggregateException;
            if (exception != null)
            {
                var innerExceptions = exception.InnerExceptions;
                var builder = new StringBuilder();
                for (int index = 0; index < innerExceptions.Count; index++)
                {
                    var innerException = innerExceptions[index];
                    builder.Append("ERROR " + index + ": " + innerException.GetAsyncTrace());
                }
                return builder.ToString();
            }

            var trace = ex.Data["_AsyncStackTrace"] as LinkedList<string>;
            if (trace == null)
            {
                return ex.ToString();
            }
            string prefix = ex.GetType().Name + ": " + ex.Message + "\r\n";
            return prefix + string.Join("\r\n", trace);
        }

        /// <summary>
        /// Appends async trace to end of original stack trace
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFullTrace(this Exception ex)
        {
            var trace = ex.Data["_AsyncStackTrace"] as LinkedList<string>;
            if (trace == null)
                return string.Empty;
            return string.Format("{0}\n--- Async stack trace:\n\t", ex)
                   + string.Join("\n\t", trace);
        }
    }
}
