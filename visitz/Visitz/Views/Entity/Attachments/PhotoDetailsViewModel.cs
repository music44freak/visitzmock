using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Resources.Localization;
using VisitzModel.Interfaces;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

public partial class PhotoDetailsViewModel : AttachmentDetailsViewModel, IBusinessObjectHolder
{
    [ObservableProperty]
    public ImageSource detailImage;

    protected override string LoadErrorText => LocalizedStrings.ImageContentMissing;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        DetailImage = ImageSource.FromStream(GetPhoto);
    }

    async Task<Stream> GetPhoto(CancellationToken token)
    {
        return await Filer.GetAppDataFileAsync(Attachment.RelativePath, token);
    }
}
