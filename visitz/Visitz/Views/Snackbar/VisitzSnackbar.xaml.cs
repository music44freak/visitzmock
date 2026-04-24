using Visitz.Views.BaseClasses;

namespace Visitz.Views.Snackbar;

public partial class VisitzSnackbar : ViewModelContentView<VisitzSnackbarViewModel>
{
    public string Message
    {
        get => ViewModel.Message;
        set => ViewModel.Message = value;
    }

    public string ActionText
    {
        get => ViewModel.ActionText;
        set => ViewModel.ActionText = value;
    }

    public Action Action
    {
        get => ViewModel.Action;
        set
        {
            ViewModel.Action = () =>
            {
                ShouldClose?.Invoke(this, EventArgs.Empty);
                value?.Invoke();
            };
        }
    }

    public event EventHandler ShouldClose;

    TimeSpan _duration;

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            _ = DelayFireShouldClose();
        }
    }

    public VisitzSnackbar()
        : base(ServiceProvider.GetService<VisitzSnackbarViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    async Task DelayFireShouldClose()
    {
        await Task.Delay(_duration, CancellationToken.None);
        ShouldClose?.Invoke(this, EventArgs.Empty);
    }
}
