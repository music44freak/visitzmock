using Visitz.Animations.Haptic;
using Visitz.Device;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Events;

namespace Visitz.Views.Entity.ChildYouthVisits;

public partial class ChildYouthVisitView : IcmRecordContentView<ChildYouthVisitViewModel>
{
    private bool _disposed;
    private bool _isKeyboardOpen;

    private SoftKeyboardOpenHandler _keyboardOpenHandler;

    public ChildYouthVisitView()
        : base(ServiceProvider.GetService<ChildYouthVisitViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
        ViewModel.SaveStateHandler.SaveStateChanged += ViewModel_DraftSaveStateChanged;

        _keyboardOpenHandler = new SoftKeyboardOpenHandler();
        _keyboardOpenHandler.KeyboardStateChanged += OnKeyboardStateChanged;
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    private void OnKeyboardStateChanged(object? sender, KeyboardStateChangedEventArgs e)
    {
        _isKeyboardOpen = e.IsKeyboardOpen;
        CheckAndApplyOrientation(_isKeyboardOpen);
    }

    private void OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
    {
        CheckAndApplyOrientation(_isKeyboardOpen);
    }

    private void CheckAndApplyOrientation(bool isKeyboardOpen)
    {
        bool hideForm =
            VisitsEditor.IsFocused
            && DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape
            && isKeyboardOpen;

        ViewModel.ShowFullForm = !hideForm;
    }

    private async void ViewModel_DraftSaveStateChanged(object? sender, DraftSaveStatusEventArgs e)
    {
        await DraftSavedIndicator.SetState(e.State);
    }

    public async Task ShowEditorError(string text)
    {
        await Task.WhenAll(ShowErrorText(text), AnimateEditorError());
    }

    private async Task ShowErrorText(string text)
    {
        if (EditorError.IsVisible)
            return;

        EditorError.Text = text;
        EditorError.Show = true;

        await Task.Delay(2000);

        EditorError.Show = false;
    }

    private async Task AnimateEditorError()
    {
        var vibrateErrorAnim = new ErrorVibrateAnimation();
        await vibrateErrorAnim.Animate(VisitsEditor);
    }

    private async void Discard_Clicked(object? sender, EventArgs e)
    {
        if (await PromptDiscard())
        {
            ViewModel.DiscardDraft();
            await Navigator.Navigation.PopModalAsync();
            SnackbarHandler.ShowText(LocalizedStrings.DiscardedVisitDraft);
        }
    }

    private static async Task<bool> PromptDiscard()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraftQuestion,
            LocalizedStrings.DiscardVisitDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );
    }

    private async void VisitsEditor_EmojiEntered(object? sender, EventArgs e)
    {
        await ShowEditorError(LocalizedStrings.InvalidEntry);
    }

    private async void VisitsEditor_SuggestedMaxLengthExceeded(object? sender, EventArgs e)
    {
        await ShowEditorError(LocalizedStrings.CharacterLimitReached);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            ViewModel.SaveStateHandler.SaveStateChanged -= ViewModel_DraftSaveStateChanged;
            ViewModel.SaveStateHandler.Dispose();
            _keyboardOpenHandler.Dispose();
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
