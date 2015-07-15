using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace osu_export.core
{
    public class Executer : IDisposable
    {
        private readonly Task executerTask;
        private readonly CancellationTokenSource executerTaskCanceler;
        private readonly BlockingCollection<Task<object>> toExecute;

        public Executer()
        {
            this.toExecute = new BlockingCollection<Task<object>>(new ConcurrentQueue<Task<object>>());
            this.executerTaskCanceler = new CancellationTokenSource();
            this.executerTask = Task.Factory.StartNew(this.ExecutionLoop, this.executerTaskCanceler.Token);
        }

        public void AddAction(Action action, string description = "")
        {
            var task = new Task<object>(() =>
            {
                Trace.TraceInformation("Executing task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
                action();
                Trace.TraceInformation("Task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] has finished \"" + description + "\"");
                return null;
            }, this.executerTaskCanceler.Token);
            if (!this.toExecute.TryAdd(task))
            {
                Trace.TraceError(@"Could not add action into execution queue");
                throw new InvalidOperationException(@"Could not add action into execution queue");
            }
            Trace.TraceInformation("Waiting end of task [" + task.Id + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
            task.Wait();
        }

        // Same as AddAction, but awaitable to know when it ends
        public async Task AddActionAsync(Action action, string description = "")
        {
            var task = new Task<object>(() =>
            {
                Trace.TraceInformation("Executing task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
                action();
                Trace.TraceInformation("Task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] has finished \"" + description + "\"");
                return null;
            }, this.executerTaskCanceler.Token);
            if (!this.toExecute.TryAdd(task))
            {
                Trace.TraceError(@"Could not add action into execution queue");
                throw new InvalidOperationException(@"Could not add action into execution queue");
            }
            Trace.TraceInformation("Awaiting end of task [" + task.Id + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
            await task;
        }

        public T AddFunc<T>(Func<T> func, string description = "")
        {
            var task = new Task<object>(() =>
            {
                Trace.TraceInformation("Executing task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
                var res = func();
                Trace.TraceInformation("Task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] has finished \"" + description + "\"");
                return res;
            }, this.executerTaskCanceler.Token);
            if (this.toExecute.TryAdd(task))
            {
                Trace.TraceInformation("Awaiting end of task [" + task.Id + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
                return (T)task.Result;
            }
            else
            {
                Trace.TraceError(@"Could not add action into execution queue");
                throw new InvalidOperationException(@"Could not add action into execution queue");
            }
        }

        // Same as addfunc but awaitable so caller can do process before it actually returns
        public async Task<T> AddFuncAsync<T>(Func<T> func, string description = "")
        {
            var task = new Task<object>(() =>
            {
                Trace.TraceInformation("Executing task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
                var res = func();
                Trace.TraceInformation("Task [" + Task.CurrentId + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] has finished \"" + description + "\"");
                return res;
            }, this.executerTaskCanceler.Token);
            if (this.toExecute.TryAdd(task))
            {
                Trace.TraceInformation("Awaiting end of task [" + task.Id + "] in thread : [" + Thread.CurrentThread.ManagedThreadId + "] \"" + description + "\"");
                return (T)await task; // Should wait for the end of the task and not execute the task in this thread
            }
            else
            {
                Trace.TraceError(@"Could not add action into execution queue");
                throw new InvalidOperationException(@"Could not add action into execution queue");
            }
        }

        //Cancel and dispose everything
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                try
                {
                    this.executerTaskCanceler.Cancel();
                    //this.executerTaskCanceler.Dispose();
                    //this.toExecute.Dispose();
                }
                catch (Exception e)
                {
                    Trace.TraceError("While disposing executor : " + e.ToString());
                }
            }
        }

        private void ExecutionLoop()
        {
            Trace.TraceInformation("Started execution loop task [" + Task.CurrentId + "] on thread : " + Thread.CurrentThread.ManagedThreadId);
            try
            {
                while (!(this.executerTaskCanceler.IsCancellationRequested))
                {
                    Task<object> del;
                    if (this.toExecute.TryTake(out del, Timeout.Infinite, this.executerTaskCanceler.Token))
                    {
                        del.RunSynchronously();
                    }
                }
            }
            catch(Exception ex)
            {
                Trace.TraceError("This weird exception showed up again : "  + ex.ToString());

            }

            Trace.TraceInformation("ExecutionLoop as ended");
        }
    }
}