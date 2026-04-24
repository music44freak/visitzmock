using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

#nullable enable

public partial class SafetyAssessmentEditView : IcmRecordContentView<SafetyAssessmentEditViewModel>
{
    // It's preferable to use lifecycle methods to determine when auto-scrolling is allowed, but MAUI's lifecycles can
    // be unreliable--so we'll use a time-delayed bool.
    // TODO: Rework this so we don't allow a scroll until we guarantee all data
    // has been loaded rather than using delays
    private bool canAutoScroll;

    private bool disposed;

    public SafetyAssessmentEditView()
        : base(ServiceProvider.GetService<SafetyAssessmentEditViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        ViewModel.SaveStateHandler.SaveStateChanged += SaveStateHandler_SaveStateChanged;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        await DelayCanAutoScroll();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            ViewModel.SaveStateHandler.SaveStateChanged -= SaveStateHandler_SaveStateChanged;
            ViewModel.SaveStateHandler?.Dispose();
            disposed = true;
        }
        base.Dispose(disposing);
    }

    private async Task DelayCanAutoScroll()
    {
        await Task.Delay(1500);
        canAutoScroll = true;
    }

    private async void SaveStateHandler_SaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        await DraftAppBar.SetDraftState(e.State);
    }

    private async void DiscardButton_Clicked(object? sender, EventArgs e)
    {
        if (await PromptDiscard())
        {
            await ViewModel.Reset();
            await Navigator.Navigation.PopModalAsync();
            SnackbarHandler.ShowText(LocalizedStrings.DiscardedSafetyAssessmentDraft);
        }
    }

    private static async Task<bool> PromptDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardSafetyAssessmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    private async void SomeChildrenPlaced_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (canAutoScroll && e.Value)
        {
            await Task.Delay(100);
            await MainScrollView.ScrollToAsync(ChildrenInCareSection.X, ChildrenInCareSection.Y, true);
        }
    }

    public void ViewAssessment(SafetyAssessment assessment)
    {
        ViewModel.IsReadOnly = true;
        ViewModel.ViewAssessment = assessment;
    }
}
