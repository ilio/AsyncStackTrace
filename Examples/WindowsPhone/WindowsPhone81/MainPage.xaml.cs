﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using AsyncStackTrace;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WindowsPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void ThrowStandardExceptionButton_Click(object sender, RoutedEventArgs e)
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
                ResultTextBlock.Text = "Standard exception:\r\n" + ee;
                Debug.WriteLine("Standard exception:\r\n" + ee);
                /**
                 * output:
Standard exception:
System.ArgumentException: Example exception
   at WindowsPhoneSilverlight81.MainPage.<ThrowStandardExceptionButton_Click>b__0()
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at WindowsPhoneSilverlight81.MainPage.<ThrowStandardExceptionButton_Click>d__2.MoveNext()
                 */
            }
        }

        private async void ThrowAsyncStackTraceAsyncOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ThrowAsyncStackTrace();
            }
            catch (Exception ee)
            {
                ee.Catch(delegate(ArgumentException exception)
                {
                    ResultTextBlock.Text = "AsyncStackTrace exception:\r\n" + exception.GetAsyncTrace();
                    Debug.WriteLine("AsyncStackTrace exception:\r\n" + exception.GetAsyncTrace());
                })
                .RethrowUnhandled();
                /*
                 * output:
AsyncStackTrace exception:
ArgumentException: Example exception
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\WindowsPhoneSilverlight81\WindowsPhoneSilverlight81\MainPage.xaml.cs:line 97
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\WindowsPhoneSilverlight81\WindowsPhoneSilverlight81\MainPage.xaml.cs:line 100

                 */
            }
        }

        private async void ThrowAsyncStackTraceFullButton_Click(object sender, RoutedEventArgs e)
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
                    ResultTextBlock.Text = "AsyncStackTrace exception:\r\n" + exception.GetFullTrace();
                    Debug.WriteLine("AsyncStackTrace exception:\r\n" + exception.GetFullTrace());
                    /*
output:
AsyncStackTrace exception:
System.ArgumentException: Example exception
   at WindowsPhoneSilverlight81.MainPage.<ThrowAsyncStackTrace>b__13()
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__c.MoveNext()
--- Async stack trace:
	   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\WindowsPhoneSilverlight81\WindowsPhoneSilverlight81\MainPage.xaml.cs:line 97
	   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\WindowsPhoneSilverlight81\WindowsPhoneSilverlight81\MainPage.xaml.cs:line 100
                     */
                })
                .RethrowUnhandled();

            }
        }

        private async void ThrowAsyncStackTraceOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await ThrowAsyncStackTrace();
            }
            catch (Exception ee)
            {
                // ee is AggregateException contains ArgumentException
                ResultTextBlock.Text = "AsyncStackTrace exception:\r\n" + ee;
                Debug.WriteLine("AsyncStackTrace exception:\r\n" + ee);
                /*
output:
AsyncStackTrace exception:
AsyncStackTrace.AggregateExceptionEx: Example exception ---> System.ArgumentException: Example exception
   at WindowsPhoneSilverlight81.MainPage.<ThrowAsyncStackTrace>b__13()
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__c.MoveNext()
   --- End of inner exception stack trace ---
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__c.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at WindowsPhoneSilverlight81.MainPage.<ThrowAsyncStackTrace>d__15.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at WindowsPhoneSilverlight81.MainPage.<ThrowAsyncStackTraceOutButton_Click>d__10.MoveNext()
---> (Inner Exception #0) System.ArgumentException: Example exception
   at WindowsPhoneSilverlight81.MainPage.<ThrowAsyncStackTrace>b__13()
   at System.Threading.Tasks.Task.InnerInvoke()
   at System.Threading.Tasks.Task.Execute()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at AsyncStackTrace.AsyncStackTraceExtension.<Trace>d__c.MoveNext()<---
----->
---> (Inner Exception #0) ArgumentException: Example exception
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\WindowsPhoneSilverlight81\WindowsPhoneSilverlight81\MainPage.xaml.cs:line 97
   at ThrowAsyncStackTrace in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\Examples\WindowsPhoneSilverlight81\WindowsPhoneSilverlight81\MainPage.xaml.cs:line 100<---
<-----
                 */
            }
        }

        private static async Task ThrowAsyncStackTrace()
        {
            await Task.Delay(0).Trace(); // do nothing, just for example, you must add .Trace() after every awaited task.

            var nestedTask = Task.Factory.StartNew(() =>
            {
                // create nested task throws exception
                throw new ArgumentException("Example exception").Trace();
            });

            await nestedTask.Trace();
        }
    }
}
