using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MinimalThreadPoolSys;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestWorker
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestWorkerSimplePasses()
    {
        var completed = false;
        var pool = new MinimalThreadPool(8);
        try {
            var num_works = 16;
            var events = new AutoResetEvent[num_works];
            for (var i= 0; i <events.Length; i++) {
                var ev = new AutoResetEvent(false);
                events[i] = ev;
                pool.QueueUserWorkItem<int>(i => {
                    Thread.Sleep(1000);
                    ev.Set();
                }, default);
            }

            for (var i = 0; i < events.Length; i++)
                if (!events[i].WaitOne(0))
                    Thread.Sleep(0);

            completed = true;
        }finally {
            pool.Dispose();
        }

        Assert.IsTrue(completed);

    }
}
