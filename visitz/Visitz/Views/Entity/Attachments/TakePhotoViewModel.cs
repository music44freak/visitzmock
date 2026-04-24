using System.Text;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Storage.Filesystem;

namespace Visitz.Views.Entity.Attachments;

public partial class TakePhotoViewModel(ICameraProvider cameraProvider) : IcmRecordViewModel
{
    public static readonly string PictureFiletype = "jpg";
    public static readonly string PictureFilenamePrepend = "Pic";

    Realm AttachmentsRealm { get; set; }

    readonly ObservableRealmQueryMap queryMap = new();

    AttachmentFiler attachmentFiler;

    [ObservableProperty]
    public IReadOnlyList<CameraInfo> cameras;

    [ObservableProperty]
    public CameraInfo selectedCamera;

    int selectedCameraIndex;

    [ObservableProperty]
    public bool waitingToProcess = true;

    [ObservableProperty]
    public bool processing;

    [ObservableProperty]
    public byte[] rollBytes;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        AttachmentsRealm = await VisitzRealms.GetAttachmentDraftsRealmAsync();
        attachmentFiler = await VisitzFiles.GetAsync(BusinessObject);

        await SetupCameras();
        SetupCameraRoll();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            AttachmentsRealm.Dispose();
            queryMap.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }

    async Task SetupCameras()
    {
        await cameraProvider.RefreshAvailableCameras(CancellationToken.None);
        Cameras = cameraProvider.AvailableCameras;

        if (Cameras.Count > 0)
            SelectedCamera = Cameras[0];
    }

    [RelayCommand]
    public void SelectNextCamera()
    {
        if (Cameras.Count > 0)
            SelectedCamera = Cameras[NextCameraIndex()];
    }

    int NextCameraIndex()
    {
        selectedCameraIndex++;
        return selectedCameraIndex %= Cameras.Count;
    }

    private void SetupCameraRoll()
    {
        queryMap.ItemsChanged += QueryMap_ItemsChanged;

        StringBuilder queryBuilder = new();
        string name = nameof(AttachmentDraft.Attachment) + "." + nameof(Attachment.Extension);

        foreach (string ext in Attachment.AllowedImageTypes)
            queryBuilder.Append($" {name} ENDSWITH '{ext}' OR");

        string filetypeQuery = queryBuilder.ToString();
        filetypeQuery = filetypeQuery[..filetypeQuery.LastIndexOf("OR")];

        queryMap.Subscribe(
            AttachmentsRealm,
            AttachmentsRealm
                .All<AttachmentDraft>()
                .Filter($"TRUEPREDICATE SORT({nameof(AttachmentDraft.DraftCreated)} DESC) LIMIT(1)")
                .Filter(filetypeQuery)
                .Where(draft => draft.RelatedEntityId == BusinessObject.FileNumber)
        );
    }

    private void QueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet Changes) e
    )
    {
        if (e.Changes == null)
        {
            if (e.Items.Any())
                RollBytes = (e.Items[0] as AttachmentDraft).Attachment.ThumbnailBinding;
        }
        else if (e.Changes.InsertedIndices.Length > 0)
            RollBytes = (e.Items[e.Changes.InsertedIndices[0]] as AttachmentDraft).Attachment.ThumbnailBinding;
    }

    public async Task SavePicture(Stream stream)
    {
        WaitingToProcess = false;

        try
        {
            string filename = attachmentFiler.MakeFilename(PictureFilenamePrepend, PictureFiletype);

            await AttachmentDraft.SaveNewPhoto(BusinessObject, attachmentFiler, AttachmentsRealm, filename, stream);
        }
        finally
        {
            WaitingToProcess = true;
        }
    }

    partial void OnWaitingToProcessChanged(bool value)
    {
        Processing = !value;
    }
}
