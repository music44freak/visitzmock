using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Views.Entity;

namespace Visitz.Views.BaseClasses;

public abstract class BaseContentView : ContentView, IDisposable, IAsyncInitialize
{
    private bool _disposedValue;

    protected ILogger Logger { get; }

    public Task InitTask { get; private set; }

    public string Title { get; protected set; }

    public BaseContentView(string title = "")
    {
        Logger = MakeLogger();
        Title = title;
    }

    protected virtual ILogger<BaseContentView> MakeLogger()
    {
        return ServiceProvider.GetService<ILogger<BaseContentView>>();
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);

        if (args.AttachingToHandler())
            InitTask ??= InitAsync();
        else if (args.DetachingFromHandler())
            Dispose();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual Task InitAsync()
    {
        Logger.TraceMethod(this);

        return Task.CompletedTask;
    }

    public async Task StartInitAsync()
    {
        await (InitTask ??= InitAsync());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            Logger.TraceMethod(this);

        _disposedValue = true;
    }
}
