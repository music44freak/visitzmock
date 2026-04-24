using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;

namespace Visitz.Views.Drafts;

public partial class DraftsContainerViewModel : VisitzViewModel
{
    [ObservableProperty]
    public bool showMenuButton;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        StrongReferenceMessenger.Default.Register<NavPositionMessage>(this, ReceiveNavPositionMessage);
        ShowMenuButton =
            StrongReferenceMessenger.Default.Send(new GetNavPositionMessage()) == ((int)TwoPaneViewMode.Tall);
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            StrongReferenceMessenger.Default.UnregisterAll(this);

            disposed = true;
        }

        base.Dispose(disposing);
    }

    void ReceiveNavPositionMessage(object recipient, NavPositionMessage message)
    {
        ShowMenuButton = ((TwoPaneViewMode)message.Value) == TwoPaneViewMode.Tall;
    }

    [RelayCommand]
    public static void OpenNavDrawer()
    {
        StrongReferenceMessenger.Default.Send(new NavDrawerMessage(isOpen: true));
    }
}
