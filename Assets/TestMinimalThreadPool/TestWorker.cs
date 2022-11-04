using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MinimalThreadPoolSys;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestWorker {

    private const int JOB_COUNT = 16;
    private const int SLEEP_DURATION = 100;

    // A Test behaves as an ordinary method
    [Test]
    public void TestAllJobsCompleted() {
        var pool = new MinimalThreadPool(8);
        try {
            var num_works = JOB_COUNT;
            for (var i = 0; i < num_works; i++) {
                pool.QueueUserWorkItem<int>(i => {
                    Thread.Sleep(SLEEP_DURATION);
                    Interlocked.Decrement(ref num_works);
                }, default);
            }

            Thread.Sleep(3 * SLEEP_DURATION);
            Assert.AreEqual(0, num_works);
        } finally {
            pool.Dispose();
        }

    }    // A Test behaves as an ordinary method
    [Test]
    public void TestDisposed() {
        var num_works = JOB_COUNT;
        using (var pool = new MinimalThreadPool(8)) {
            pool.UnhandledException += (obj, e) 
                => Assert.AreEqual(typeof(ThreadInterruptedException), e.Exception.GetType());

            for (var i = 0; i < num_works; i++) {
                pool.QueueUserWorkItem<int>(i => {
                    Thread.Sleep(SLEEP_DURATION);
                    Interlocked.Decrement(ref num_works);
                }, default);
            }
        }
        Assert.AreEqual(JOB_COUNT, num_works);
    }
}
