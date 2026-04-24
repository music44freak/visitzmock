using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using Visitz.Views.Snackbar;
using VisitzModel.Extensions;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentDraftsListViewModel : IcmRecordViewModel
{
    Realm? attachmentsRealm;

    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    ObservableCollection<AttachmentDraftListItemUi> attachmentDrafts = [];

    [ObservableProperty]
    public bool isLoading = true;

    [ObservableProperty]
    public bool isEmpty;

    public readonly TaskCompletionSource attachmentsLoadedTcs = new();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        attachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();

        realmQuery.Subscribe(
            attachmentsRealm,
            attachmentsRealm.All<AttachmentDraft>().Where(draft => draft.RelatedEntityId == BusinessObject.FileNumber)
        );

        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();
            attachmentsRealm?.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQuery_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        IsLoading = false;
        IsEmpty = !realmQuery[typeof(AttachmentDraft)].Query.Any();

        if (e.Changes == null)
        {
            foreach (var item in e.Items)
                AttachmentDrafts.Add(new AttachmentDraftListItemUi((AttachmentDraft)item));

            attachmentsLoadedTcs.TrySetResult();
        }
        else
        {
            foreach (int deleted in e.Changes.DeletedIndices.Reverse())
                AttachmentDrafts.RemoveAt(deleted);

            foreach (int modified in e.Changes.ModifiedIndices)
                AttachmentDrafts[modified] = new AttachmentDraftListItemUi((AttachmentDraft)e.Items[modified]);

            foreach (int inserted in e.Changes.InsertedIndices)
                AttachmentDrafts.Insert(inserted, new AttachmentDraftListItemUi((AttachmentDraft)e.Items[inserted]));
        }
    }

    [RelayCommand]
    public static void DeleteAttachmentDraft(AttachmentDraft draft)
    {
        _ = PromptDiscardAttachmentDraftAsync(draft);
    }

    static async Task PromptDiscardAttachmentDraftAsync(AttachmentDraft draft)
    {
        bool shouldDiscard = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.DiscardDraft,
            LocalizedStrings.DiscardAttachmentDraftDescription,
            LocalizedStrings.Discard,
            LocalizedStrings.Cancel
        );

        if (shouldDiscard)
        {
            string filename = draft.Attachment.Filename;
            await draft.Attachment.DeleteAsync();
            SnackbarHandler.ShowText(LocalizedStrings.FileDiscarded.Format(filename));
        }
    }

    [RelayCommand]
    public void PublishAttachmentDraft(AttachmentDraft draft)
    {
        _ = DoPublishAttachmentDraft(draft);
    }

    async Task DoPublishAttachmentDraft(AttachmentDraft draft)
    {
        var attachmentPublishVm = ServiceProvider.Current.GetService<AttachmentDraftPublishViewModel>();
        if (attachmentPublishVm == null)
            return;

        await attachmentPublishVm.SetPayload(BusinessObject, draft);
        await Navigator.Navigation.PushModalAsync(new PublishPage(attachmentPublishVm));
    }

    [RelayCommand]
    public void OpenAttachment(AttachmentDraft draft)
    {
        _ = DoOpenAttachment(draft);
    }

    async Task DoOpenAttachment(AttachmentDraft draft)
    {
        if (!draft.Attachment.FileExistsLocally)
            return;

        string path = draft.Attachment.RelativePath.Trim();

        ContentView view = path.EndsWith(Attachment.Pdf.Trim('.'))
            ? MakePdfDetailsView(draft.Attachment)
            : MakePhotoDetailsView(draft.Attachment);

        await Navigator.Navigation.PushAsync(view);
    }

    PhotoDetailsView MakePhotoDetailsView(Attachment attachment)
    {
        return new() { Attachment = attachment, BusinessObject = BusinessObject };
    }

    PdfDetailsView MakePdfDetailsView(Attachment attachment)
    {
        return new() { Attachment = attachment, BusinessObject = BusinessObject };
    }
}
