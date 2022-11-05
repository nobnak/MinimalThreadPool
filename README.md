# Minimal ThreadPool for Unity

## Features
- Minimal, Scoped and Object-oriented
- Independent from System's [ThreadPool](https://learn.microsoft.com/ja-jp/dotnet/api/system.threading.threadpoo)

## Install
- As Git UPM: https://github.com/nobnak/MinimalThreadPool.git?path=/Assets/Packages/MinimalThreadPool

## Usage
```csharp
var number_of_worker_threads = 8;
var number_of_jobs = 32;

using (var pool = new MinimalThreadPool(number_of_worker_threads)) {
    pool.UnhandledException += (obj, e) => Debug.Log($"{e.Exception}");

    for (var i = 0; i < number_of_jobs; i++) {
        pool.QueueUserWorkItem<int>(i => {
            Thread.Sleep(1000);
            Interlocked.Decrement(ref number_of_jobs);
        }, i);
    }
    
    while (number_of_jobs > 0)
      Thread.Sleep(0);
    
}
```
