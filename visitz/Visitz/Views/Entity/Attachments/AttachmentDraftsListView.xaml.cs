using Visitz.Views.BaseClasses;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentDraftsListView : IcmRecordContentView<AttachmentDraftsListViewModel>, IFocusDraftItem
{
    public IDraftItem? FocusedDraftItem { get; set; }

    readonly TaskCompletionSource loadingTcs = new();

    public AttachmentDraftsListView()
        : base(ServiceProvider.GetService<AttachmentDraftsListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        Loaded += AttachmentDraftsListView_Loaded;
    }

    private void AttachmentDraftsListView_Loaded(object? sender, EventArgs e)
    {
        loadingTcs.TrySetResult();
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        await Task.WhenAll(loadingTcs.Task, ViewModel.attachmentsLoadedTcs.Task);

        TryNavigateToFocusDraft();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            Loaded -= AttachmentDraftsListView_Loaded;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    void TryNavigateToFocusDraft()
    {
        if (FocusedDraftItem == null)
            return;

        var draftItem = ViewModel.AttachmentDrafts.FirstOrDefault(draftItem =>
            draftItem.Attachment.Draft.Preview == FocusedDraftItem.Preview
            && draftItem.Attachment.Draft.LastUpdated == FocusedDraftItem.LastUpdated
        );

        if (draftItem == null)
            return;

        ScrollToDraft(draftItem);
        ViewModel.OpenAttachment(draftItem.Attachment.Draft);

        FocusedDraftItem = null;
    }

    void ScrollToDraft(AttachmentDraftListItemUi draft)
    {
        DraftsCollection.ScrollTo(draft, position: ScrollToPosition.Center, animate: false);
    }
}
