using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Visitz.Animations;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Messaging;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Root;

#nullable enable

public partial class RootPage : VisitzPage, ISnackbarPresenter
{
    VisitzSnackbar? Snackbar { get; set; }

    public RootPage()
        : base(ServiceProvider.GetService<RootViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        StrongReferenceMessenger.Default.Register<AppNavMessage>(this, ReceiveAppNavMessage);
        StrongReferenceMessenger.Default.Register<NavDrawerMessage>(this, ReceiveNavDrawerMessage);
        StrongReferenceMessenger.Default.Register<GetNavPositionMessage>(this, SendNavPosition);

        HideSoftInputOnTapped = true;
    }

    private void ReceiveAppNavMessage(object recipient, AppNavMessage message)
    {
        if (message.Value is NavItem nav)
        {
            var content =
                ServiceProvider.GetService(nav.ContentViewType) as ContentView
                ?? throw new InvalidOperationException("Requested navigation item was null");

            SetContent(content);
        }
    }

    private void ReceiveNavDrawerMessage(object _, NavDrawerMessage message)
    {
        if (NavDrawer.IsOpen != message.Value)
            NavDrawer.ToggleDrawer();
    }

    private void SetContent(IView view)
    {
        if (view is View v)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            // StackLayout with FillAndExpand has so far been the most reliable layout mechanism in MAUI, so we'll
            // suppress compiler warnings about it.

            v.HorizontalOptions = LayoutOptions.FillAndExpand;
            v.VerticalOptions = LayoutOptions.FillAndExpand;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        ContentPane.Clear();
        ContentPane.Add(view);
    }

    public void SetSnackbar(VisitzSnackbar? snackbar)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Snackbar?.ShouldClose -= Snackbar_ShouldClose;

            Snackbar = snackbar;
            SnackbarContainer.Content = Snackbar;
            SnackbarContainer.IsVisible = Snackbar != null;

            if (Snackbar != null)
            {
                Snackbar.ShouldClose += Snackbar_ShouldClose;
                _ = new VisibilityAnimation(showView: true, 150).Animate(Snackbar);
            }
        });
    }

    public void Snackbar_ShouldClose(object? sender, EventArgs e)
    {
        _ = AnimateCloseSnackbar();
    }

    private async Task AnimateCloseSnackbar()
    {
        if (Snackbar != null)
            await new VisibilityAnimation(showView: false, 150).Animate(Snackbar);

        SetSnackbar(null);
    }

    private void TwoPaneView_ModeChanged(object? sender, EventArgs e)
    {
        if (ViewModel is RootViewModel rvm && sender is TwoPaneView paneView)
        {
            rvm.UpdateOrientationVisibility(paneView.Mode);
            StrongReferenceMessenger.Default.Send(new NavPositionMessage((int)paneView.Mode));
        }
    }

    private static void SendNavPosition(object recipient, GetNavPositionMessage message)
    {
        if (recipient is RootPage root)
            message.Reply((int)root.TwoPane.Mode);
    }
}
