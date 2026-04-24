using Visitz.FontIcons;
#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration;
#endif

namespace Visitz.Controls;

internal class CloseButton : FontIconButton
{
    public event EventHandler<ClosingEventArgs> Closing;

    public CloseButton()
        : base()
    {
        FontFamily = MaterialIcons.RoundedUnfilled.FontFamily;
        Text = MaterialIcons.Close;
        TextColor = Colors.White;

        Clicked += CloseButton_Clicked;
    }

    private async void CloseButton_Clicked(object? sender, EventArgs e)
    {
        var closingEventArgs = new ClosingEventArgs();
        Closing?.Invoke(this, closingEventArgs);

        if (closingEventArgs.Cancel)
            return;

        if (Navigator.CurrentOpenModal != null)
            await Navigator.Navigation.PopModalAsync();
        else
            await Navigator.Navigation.PopAsync();
    }
}
