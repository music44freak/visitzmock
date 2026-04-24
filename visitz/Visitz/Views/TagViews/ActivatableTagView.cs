using Visitz.VisualStates;

namespace Visitz.Views.TagViews;

public partial class ActivatableTagView : TagView, IActiveState
{
    public event EventHandler<IActiveState.ActiveChangedEventArgs> ActiveStateChanged;

    public event CancelTapEventDelegate ShouldCancelTapEvent;

    public delegate bool CancelTapEventDelegate(ActivatableTagView sender, TappedEventArgs e);

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            UpdateVisualState();
            ActiveStateChanged?.Invoke(this, new IActiveState.ActiveChangedEventArgs(IsActive));
        }
    }

    public ActivatableTagView()
        : base()
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += Tapped;
        GestureRecognizers.Add(tap);
    }

    private void Tapped(object? sender, TappedEventArgs e)
    {
        if (ShouldCancelTapEvent?.Invoke(this, e) ?? false)
            return;

        IsActive = !IsActive;
    }

    public void SetIsActiveSilently(bool isActive)
    {
        this.isActive = isActive;
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, (this as IActiveState).GetActiveState());
    }
}
