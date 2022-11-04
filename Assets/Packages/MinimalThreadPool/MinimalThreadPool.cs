using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MinimalThreadPoolSys {

    public class MinimalThreadPool : System.IDisposable {

        public event EventHandler<WorkItemEventArgs> UnhandledException;

        protected BlockingCollection<IWorkItem> queue = new BlockingCollection<IWorkItem>();
        protected List<Thread> workers = new List<Thread>();

        public MinimalThreadPool(int threads, ThreadPriority priority = ThreadPriority.BelowNormal) {
            for (var i = 0; i < threads; i++) {
                var th = new Thread(Worker);
                th.IsBackground = true;
                th.Priority = priority;
                workers.Add(th);
                th.Start();
            }

        }

        #region IDisposable
        public void Dispose() {
            queue.CompleteAdding();
            foreach (var th in workers) th.Interrupt();
        }
        #endregion

        public bool QueueUserWorkItem<TState>(System.Action<TState> callBack, TState state) {
            return queue.TryAdd(new WorkItem<TState>(callBack, state));
        }

        protected void Worker() {
            IWorkItem work = null;
            while (!queue.IsAddingCompleted) {
                try {
                    work = queue.Take();
                    work?.Execute();
                } catch (System.Exception e) {
                    UnhandledException?.Invoke(this, new WorkItemEventArgs(e, work));
                }
            }
        }

        #region declarations
        public interface IWorkItem {
            void Execute();
        }
        public class WorkItem<TState> : IWorkItem {

            public System.Action<TState> job;
            public TState state;

            public WorkItem(System.Action<TState> job, TState state) {
                this.job = job;
                this.state = state;
            }

            public void Execute() => job(state);
        }
        public class WorkItemEventArgs : EventArgs {

            public System.Exception Exception { get; set; }
            public IWorkItem Work { get; set; }

            public WorkItemEventArgs(System.Exception e, IWorkItem work) {
                this.Exception = e;
                this.Work = work;
            }
        }
        #endregion
    }
}
