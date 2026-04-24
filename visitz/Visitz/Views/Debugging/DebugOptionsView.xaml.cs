using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Debugging;

public partial class DebugOptionsView : ViewModelContentView<DebugOptionsViewModel>
{
    public DebugOptionsView()
        : base(ServiceProvider.GetService<DebugOptionsViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    private async void ShowDocumentsButton_Clicked(object? sender, EventArgs e)
    {
        var popup = new Popup()
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
            Content = new ScrollView()
            {
                Orientation = ScrollOrientation.Both,
                Content = new Label()
                {
                    HorizontalOptions = LayoutOptions.Start,
                    Text = DebugOptions.ListDocumentsFiles(),
                },
            },
        };

        await Navigator.CurrentOpenPage.ShowPopupAsync(popup);
    }
}
