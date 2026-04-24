using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.FontIcons;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class AttachmentDraftListItemUi : ObservableObject
{
    [ObservableProperty]
    public Attachment attachment;

    [ObservableProperty]
    public bool hasThumbnail;

    [ObservableProperty]
    public string fontFamily = string.Empty;

    [ObservableProperty]
    public string iconGlyph = string.Empty;

    public AttachmentDraftListItemUi(Attachment attachment)
    {
        Attachment = attachment;
        HasThumbnail = attachment.Thumbnail?.Length > 0;

        FontFamily = FluentIcons.FontConfig.FontFamily;
        IconGlyph = FluentIcons.Document_pdf_20_regular;
    }

    public AttachmentDraftListItemUi(AttachmentDraft draft)
        : this(draft.Attachment) { }
}
