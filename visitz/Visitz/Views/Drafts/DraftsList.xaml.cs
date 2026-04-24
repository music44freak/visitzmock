using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions.EntityTypes;
using VisitzModel.Models.Drafts;

namespace Visitz.Views.Drafts;

#nullable enable

public partial class DraftsList : ViewModelContentView<DraftsListViewModel>
{
    public DraftsList()
        : base(ServiceProvider.GetService<DraftsListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override Task InitAsync()
    {
        var init = base.InitAsync();

        ViewModel.SelectedItemRelatedMissing += ViewModel_SelectedItemRelatedMissing;

        return init;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            ViewModel.SelectedItemRelatedMissing -= ViewModel_SelectedItemRelatedMissing;
            disposed = true;
        }
        base.Dispose(disposing);
    }

    void ViewModel_SelectedItemRelatedMissing(object? sender, IDraftItem draft)
    {
        _ = DoPromptDiscardAsync(draft);
    }

    async Task DoPromptDiscardAsync(IDraftItem draft)
    {
        if (await PromptDiscardDraftAsync(draft))
            await ViewModel.DeleteDraftAsync(draft);
    }

    static async Task<bool> PromptDiscardDraftAsync(IDraftItem draft)
    {
        string message = string.Format(
            LocalizedStrings.DiscardUnlinkedDraftDesc,
            draft.RelatedEntityType.GetDisplayString(),
            !string.IsNullOrWhiteSpace(draft.DraftLocation) ? draft.DraftLocation : draft.RelatedEntityId
        );

        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardUnlinkedDraft,
            message,
            LocalizedStrings.DiscardDraft,
            LocalizedStrings.CancelAndKeepDraft
        );
    }
}
