using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Visitz.Extensions;
using Visitz.Views.Entity;

namespace Visitz.Views.BaseClasses
{
    /// <summary>
    /// The base class for all the view models. Common functionality can be defined here.
    /// </summary>
    public partial class VisitzViewModel : ObservableObject, IDisposable, IAsyncInitialize
    {
        protected virtual ILogger<VisitzViewModel> Logger { get; } =
            ServiceProvider.GetService<ILogger<VisitzViewModel>>();

        bool _disposedValue;

        public Task InitTask { get; private set; }

        public virtual Task StartInitAsync()
        {
            InitTask ??= InitAsync();

            return InitTask;
        }

        protected virtual Task InitAsync()
        {
            Logger.TraceMethod(this);

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
                Logger.TraceMethod(this);

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
