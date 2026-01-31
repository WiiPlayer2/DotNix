using System;
using System.Runtime.CompilerServices;

namespace DotNix.Helpers;

public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(taskFactory) { }

    public TaskAwaiter<T> GetAwaiter() { return Value.GetAwaiter(); }
}