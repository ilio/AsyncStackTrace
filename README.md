# Nuget
```
PM> Install-Package AsyncStackTrace
```
https://www.nuget.org/packages/AsyncStackTrace/
# AsyncStackTrace
Convert useless stack trace for async operations to regular stack trace 

Output before:
```
System.AggregateException: One or more errors occurred. ---> System.TimeoutException: It timed out ---> System.ArgumentException: ID missing
Result StackTrace:	
at UnitTests.AsyncStackTraceExtensionUnitTest.<InnerTask3>d__b.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at UnitTests.AsyncStackTraceExtensionUnitTest.<InnerTask2>d__8.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at UnitTests.AsyncStackTraceExtensionUnitTest.<InnerTask1>d__5.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
   at UnitTests.AsyncStackTraceExtensionUnitTest.<TestExceptionsAsync>d__2.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at AsyncStackTrace.AsyncStackTraceExtension.RethrowUnhandled(Exception e) 
   at UnitTests.AsyncStackTraceExtensionUnitTest.<TestExceptionsAsync>d__2.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
```
Output after:
```
AsyncStackTrace.AggregateExceptionEx: One or more errors occurred. ---> System.TimeoutException: It timed out ---> System.ArgumentException: ID missing
Result StackTrace:	
---> (Inner Exception #0) TimeoutException: It timed out ---> ArgumentException: ID missing
   at InnerTask3 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 69
   --- End of inner exception stack trace ---
   at InnerTask3 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 69
   at InnerTask3 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 84
   at InnerTask2 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 58
   at InnerTask1 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 53
   at TestExceptionsAsync in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 20<---
---> (Inner Exception #1) NotImplementedException: Somethings not implemented
   at InnerTask3 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 74
   at InnerTask3 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 84
   at InnerTask2 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 58
   at InnerTask1 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 53
   at TestExceptionsAsync in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 20<---
---> (Inner Exception #2) IOException: not available ---> ArgumentException: file not found1 ---> ArgumentException: file not found2
   at InnerTask4 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 92
   --- End of inner exception stack trace ---
   at InnerTask4 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 93
   --- End of inner exception stack trace ---
   at InnerTask4 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 94
   at InnerTask3 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 84
   at InnerTask2 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 58
   at InnerTask1 in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 53
   at TestExceptionsAsync in d:\Documents\Visual Studio 2013\Projects\AsyncStackTrace\UnitTests\AsyncStackTraceExtensionUnitTest.cs:line 20<---
```
How to use

Code before:
```cs
try{    
   await NestedAsyncOperation();
}catch(NullReferenceException e){
  Debug.WriteLine(e)
}
```
Code after:
```cs
try{    
   await NestedAsyncOperation().Trace();
}catch(Exception e){
   e.Catch((NullReferenceException ex) => {
     Debug.WriteLine(e);
   }).RethrowUnhandled();
}
```
#More examples
Windows Phone Silverlight 8.0 example:
https://github.com/ilio/AsyncStackTrace/blob/master/Examples/WindowsPhone/WindowsPhoneSilverlight80/MainPage.xaml.cs

Windows Phone Silverlight 8.1 example:
https://github.com/ilio/AsyncStackTrace/blob/master/Examples/WindowsPhone/WindowsPhoneSilverlight81/MainPage.xaml.cs

Windows phone 8.1 example:
https://github.com/ilio/AsyncStackTrace/blob/master/Examples/WindowsPhone/WindowsPhone81/MainPage.xaml.cs

Windows 10 (universal app) example:
https://github.com/ilio/AsyncStackTrace/blob/master/Examples/Windows/Windows10/MainPage.xaml.cs

Windows .NET 4.5 example: 
https://github.com/ilio/AsyncStackTrace/blob/master/Examples/Windows/Windows45/Program.cs
