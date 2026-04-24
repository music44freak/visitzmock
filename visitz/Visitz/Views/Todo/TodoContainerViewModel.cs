using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;

namespace Visitz.Views.Todo;

public partial class TodoContainerViewModel : VisitzViewModel
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

    [RelayCommand]
    public static void OpenNavDrawer()
    {
        StrongReferenceMessenger.Default.Send(new NavDrawerMessage(isOpen: true));
    }

    void ReceiveNavPositionMessage(object recipient, NavPositionMessage message)
    {
        ShowMenuButton = ((TwoPaneViewMode)message.Value) == TwoPaneViewMode.Tall;
    }
}
