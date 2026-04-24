using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;
using Tab = Visitz.Views.Navigation.Tab;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentsView : IcmRecordContentView<AttachmentsViewModel>, IFocusDraftItem
{
    static readonly IEnumerable<string> AllowedTypes = Attachment.AllowedImageTypes.Concat(
        Attachment.AllowedDocumentTypes
    );

    Tab? DownloadedTab;

    Tab? DraftsTab;

    public IDraftItem? FocusedDraftItem
    {
        get => ViewModel.FocusedDraftItem;
        set => ViewModel.FocusedDraftItem = value;
    }

    public AttachmentsView()
        : base(ServiceProvider.GetService<AttachmentsViewModel>(), LocalizedStrings.Attachments)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        try
        {
            DownloadedTab = new Tab(
                LocalizedStrings.InIcm,
                () =>
                {
                    var listView = ServiceProvider.GetService<AttachmentsListView>();
                    listView.ViewModel.RowId = RowId;
                    listView.ViewModel.EntityType = EntityType;
                    return listView;
                }
            );

            DraftsTab = new(
                LocalizedStrings.OnMyDevice,
                () =>
                {
                    var draftsView = ServiceProvider.GetService<AttachmentDraftsListView>();
                    draftsView.ViewModel.RowId = RowId;
                    draftsView.ViewModel.EntityType = EntityType;
                    draftsView.FocusedDraftItem = FocusedDraftItem;
                    return draftsView;
                }
            );

            if (TabDisplayView != null)
                AttachmentsTabs.PairedDisplayView = TabDisplayView;

            AttachmentsTabs.Tabs = [DownloadedTab, DraftsTab];

            if (FocusedDraftItem != null)
                AttachmentsTabs.SelectedTab = DraftsTab;
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            AttachmentsTabs.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private async void AddPhotos_Clicked(object? sender, EventArgs e)
    {
        await OpenTakePhotoView();
    }

    private async void Browse_Clicked(object? sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(
            new()
            {
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>() { { DevicePlatform.WinUI, AllowedTypes } }
                ),
            }
        );

        if (result != null)
            await SaveFile(result);
    }

    private async Task SaveFile(FileResult result)
    {
        if (result == null)
            return;

        try
        {
            await ViewModel.SaveFile(result);
            // TODO: Switch to drafts tab on successful save.
            // Had some weird issues where Realm was getting disposed seemingly randomly.
            // Don't have time to debug it right now, so leaving this TODO here.
        }
        catch (Exception ex)
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(ex);
        }
    }

    private async Task OpenTakePhotoView()
    {
        await TakePhotoView.TryOpenWithPermissionsAsync(BusinessObject);
    }
}
