// https://devblogs.microsoft.com/dotnet/asynclazyt/
// Thanks Stephen!

namespace VisitzModel.Utilities;

/// <summary>
/// Provides support for asynchronous lazy initialization.
/// </summary>
/// <typeparam name="T"></typeparam>
public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory)
        : base(() => Task.Factory.StartNew(valueFactory)) { }

    public AsyncLazy(Func<Task<T>> taskFactory)
        : base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }
}
