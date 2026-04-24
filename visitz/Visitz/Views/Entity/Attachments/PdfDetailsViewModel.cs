using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Visitz.Resources.Localization;
using VisitzModel.Extensions;
using VisitzModel.Models.Attachments;

namespace Visitz.Views.Entity.Attachments;

#nullable enable

public partial class PdfDetailsViewModel : AttachmentDetailsViewModel
{
    static readonly string EmbedHtmlPath = Path.Join("PDF", "pdf-embed.html");

    [ObservableProperty]
    public WebViewSource? source;

    protected override string LoadErrorText => LocalizedStrings.PdfContentMissing;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (Assembly.GetEntryAssembly() is Assembly entry)
            Source = GetEmbedPath(entry);
        else
        {
            ErrorText = LocalizedStrings.UnableToLoadPdf;

            ServiceProvider
                .GetService<ILogger<PdfDetailsViewModel>>()
                .LogError("{ErrorText} -> Couldn't load entry assembly", ErrorText);

            return;
        }
    }

    static string? GetEmbedPath(Assembly entry)
    {
        return Path.Join(Path.GetDirectoryName(entry.Location), EmbedHtmlPath);
    }

    public async Task<string> MakeBase64Pdf()
    {
        if (Attachment != null && Filer != null)
        {
            Stream stream = await Filer.GetAppDataFileAsync(Attachment.RelativePath);
            return Convert.ToBase64String(await stream.AsBytesAsync());
        }
        else
            return "";
    }
}
