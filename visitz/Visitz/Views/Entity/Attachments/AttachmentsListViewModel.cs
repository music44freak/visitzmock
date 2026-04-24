using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Realms;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Attachments;
using Visitz.Views.BaseClasses;
using Visitz.Views.Snackbar;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Caseload;
using VisitzModel.Storage;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentsListViewModel : IcmRecordViewModel
{
    private bool _disposed;

    readonly ObservableRealmQueryMap realmQuery = new();

    [ObservableProperty]
    public ObservableCollection<AttachmentsListItemUi> attachmentsList = [];

    [ObservableProperty]
    public bool isEmpty;

    public UserIgnoredContentPrefs? UserIgnoredContentPrefs { get; set; }

    public AttachmentsListViewModel()
    {
        UserIgnoredContentPrefs = new UserIgnoredContentPrefs(Preferences.Default);
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        realmQuery.ItemsChanged += RealmQuery_ItemsChanged;

        realmQuery.Subscribe(
            DataRealm,
            Attachment.GetOrderedAttachments(DataRealm, BusinessObject.EntityType, BusinessObject.Id)
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            realmQuery.ItemsChanged -= RealmQuery_ItemsChanged;
            realmQuery.Dispose();

            foreach (var item in AttachmentsList)
                item.Dispose();

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQuery_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(Attachment))
            UpdateAttachmentsList(e.Items, e.Changes);
    }

    private void UpdateAttachmentsList(IRealmCollection<IRealmObject> items, ChangeSet? changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                AttachmentsList.Add(MakeItemUi((Attachment)item));
        }
        else
        {
            foreach (int deleted in changes.DeletedIndices.Reverse())
            {
                AttachmentsList.ElementAt(deleted).Dispose();
                AttachmentsList.RemoveAt(deleted);
            }

            foreach (int inserted in changes.InsertedIndices)
                AttachmentsList.Insert(inserted, MakeItemUi((Attachment)items[inserted]));
        }

        IsEmpty = !AttachmentsList.Any();
    }

    AttachmentsListItemUi MakeItemUi(Attachment attachment)
    {
        return new AttachmentsListItemUi(BusinessObject.EntityType, BusinessObject.Id, attachment);
    }

    [RelayCommand]
    public void DeleteDownloadedAttachmentFromDevice(AttachmentsListItemUi item)
    {
        _ = PromptRemoveAttachmentAsync(item);
    }

    public async Task PromptRemoveAttachmentAsync(AttachmentsListItemUi item)
    {
        bool shouldRemove = await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.RemoveAttachmentFromDevice,
            LocalizedStrings.RemoveAttachmentDescription,
            LocalizedStrings.Remove,
            LocalizedStrings.Cancel
        );

        if (shouldRemove)
        {
            item.Attachment.RemoveFileFromDevice();
            UserIgnoredContentPrefs?.SetUserIgnoredContent(item.Attachment.Id, true);
            string removedText = string.Format(LocalizedStrings.RemovedAttachmentFromDevice, item.Attachment.Filename);
            SnackbarHandler.ShowText(removedText);
        }
    }

    [RelayCommand]
    public void DownloadAttachmentForDevice(AttachmentsListItemUi listItem)
    {
        if (BusinessObject is IBusinessObject item)
        {
            var recordServiceInfo = new RecordServiceInfo(item);
            var attachmentId = listItem.Attachment.Id;
            var force = true;

            var tuple = (recordServiceInfo, attachmentId, force);
            var msg = GetAttachmentContentService.MakeStartMessage(tuple);
            WeakReferenceMessenger.Default.Send(msg);
            UserIgnoredContentPrefs?.SetUserIgnoredContent(attachmentId, false);
        }
        else
            throw new InvalidOperationException(nameof(IBusinessObject));
    }

    [RelayCommand]
    public async Task OpenAttachment(AttachmentsListItemUi listItem)
    {
        if (!listItem.Attachment.FileExistsLocally)
            return;

        string path = listItem.Attachment.RelativePath.Trim();

        ContentView view = path.EndsWith(Attachment.Pdf.Trim('.'))
            ? MakePdfDetailsView(listItem.Attachment)
            : MakePhotoDetailsView(listItem.Attachment);

        await Navigator.Navigation.PushAsync(view);
    }

    PhotoDetailsView MakePhotoDetailsView(Attachment attachment)
    {
        return new()
        {
            Attachment = attachment,
            BusinessObject = BusinessObject,
            IsDownloadedAttachment = true,
        };
    }

    PdfDetailsView MakePdfDetailsView(Attachment attachment)
    {
        return new()
        {
            Attachment = attachment,
            BusinessObject = BusinessObject,
            IsDownloadedAttachment = true,
        };
    }
}
