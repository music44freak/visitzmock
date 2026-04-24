namespace Visitz.Views.Navigation;

#nullable enable

internal partial class ContentViewNavigationStack : ContentView
{
    const uint _animationLength = 250;

    const double _minimumOpacity = 0.2d;

    static readonly Easing _fadeEasing = Easing.Linear;

    static readonly Easing _translateEasing = Easing.CubicInOut;

    readonly Stack<ContentView> _viewStack = new();

    readonly AbsoluteLayout _layout = [];

    public ContentViewNavigationStack()
    {
        Content = _layout;
    }

    public async Task PushAsync(ContentView newView)
    {
        Task currentViewAnimationTask =
            _viewStack.TryPeek(out ContentView? currentView) && currentView != null
                ? currentView.FadeToAsync(_minimumOpacity, _animationLength, _fadeEasing)
                : Task.CompletedTask;

        _viewStack.Push(newView);
        _layout.Add(newView);

        AbsoluteLayout.SetLayoutBounds(newView, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(newView, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.All);

        newView.TranslationX = X + Width;

        await Task.WhenAll(
            currentViewAnimationTask,
            newView.TranslateToAsync(X, Y, _animationLength, _translateEasing)
        );

        currentView?.Opacity = 1.0d;
    }

    public async Task<ContentView?> PopAsync()
    {
        if (!_viewStack.TryPop(out ContentView? view) || view == null)
            return null;

        Task underViewAnimationTask = Task.CompletedTask;
        if (_viewStack.TryPeek(out ContentView? underView) && underView != null)
        {
            underView.Opacity = _minimumOpacity;
            underViewAnimationTask = underView.FadeToAsync(1.0d, _animationLength, _fadeEasing);
        }

        await Task.WhenAll(
            underViewAnimationTask,
            view.TranslateToAsync(X + view.Width, Y, _animationLength, _translateEasing)
        );

        _layout.Remove(view);

        if (view is IDisposable disposable)
            disposable.Dispose();

        return view;
    }
}
