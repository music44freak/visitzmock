using System.ComponentModel;
using Visitz.Extensions;
using Visitz.Views.Surveys;

namespace Visitz.Views.BaseClasses.Publishing;

public partial class PublishPage : VisitzPage
{
    public new PublishViewModel ViewModel => base.ViewModel as PublishViewModel;

    public PublishPage(PublishViewModel publishViewModel)
        : base(publishViewModel)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override void OnCreated()
    {
        base.OnCreated();

        ViewModel.OnCompleted += PublishPage_OnCompleted;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        string name = e.PropertyName;

        if (name == nameof(ViewModel.ShowDismissButton) || name == nameof(ViewModel.ShowRetryButton))
            AdjustDismissButtonStyles();
    }

    private void AdjustDismissButtonStyles()
    {
        bool onlyDismiss = ViewModel.ShowDismissButton && !ViewModel.ShowRetryButton;

        DismissButton.HorizontalOptions = onlyDismiss ? LayoutOptions.Center : LayoutOptions.End;

        int fullSpan = MainGrid.ColumnDefinitions.Count;
        int singleColumn = 1;
        Grid.SetColumnSpan(DismissButton, onlyDismiss ? fullSpan : singleColumn);
    }

    protected override void OnDestroyed()
    {
        ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        ViewModel.OnCompleted -= PublishPage_OnCompleted;

        base.OnDestroyed();
    }

    private async void PublishPage_OnCompleted(object? sender, EventArgs e)
    {
        await Task.WhenAll(AnimateCountdown(), Task.Delay(PublishViewModel.DismissDuration));

        await TryPopAsync();

        await FeedbackSurveyPage.TryOpen();
    }

    async Task AnimateCountdown()
    {
        DismissProgressBar.IsVisible = true;

        DismissProgressBar.Progress = 1.0d;

        await DismissProgressBar.ProgressTo(0.0d, (uint)PublishViewModel.DismissDuration, Easing.Linear);

        DismissProgressBar.IsVisible = false;
    }

    protected override bool OnBackButtonPressed()
    {
        // Prevent the user from backing out of this screen in favour of using the dismiss button.
        return false;
    }

    private async void PublishStatus_Tapped(object? sender, TappedEventArgs e)
    {
        if (ViewModel.ShowPublishErrorIcon && ViewModel.PublishErrorDetail?.Length > 0)
            await this.DisplayErrorAlert(ViewModel.PublishErrorDetail);
    }

    private async void RefreshStatus_Tapped(object? sender, TappedEventArgs e)
    {
        if (ViewModel.ShowRefreshErrorIcon && ViewModel.RefreshErrorDetail?.Length > 0)
            await this.DisplayErrorAlert(ViewModel.RefreshErrorDetail);
    }

    async Task TryPopAsync()
    {
        var nav = Navigator.Navigation;

#pragma warning disable SS004 // Implement Equals() and GetHashcode() methods for a type used in a collection.
        //Disabling SS004 because Page is a MAUI framework type and we cannot add these methods
        if (nav.NavigationStack.Contains(this))
            await nav.PopAsync();
        else if (nav.ModalStack.Contains(this))
            await nav.PopModalAsync();
#pragma warning restore SS004 // Implement Equals() and GetHashcode() methods for a type used in a collection.
    }

    private async void DismissButton_Clicked(object? sender, EventArgs e)
    {
        await TryPopAsync();
    }
}
