// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Windows.AppLifecycle;
using Oidc.WinWorkaround;
using Visitz.Platforms.Windows.Visitz;
using Visitz.Views.WebViewer;
using VisitzModel.Platforms.Windows.Logging;
using WebAuthenticator = Oidc.WinWorkaround.WebAuthenticator;

#nullable enable

namespace Visitz.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public CancellationTokenSource? AuthCancelTokenSource { get; set; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
        UnhandledException += App_UnhandledException;

        try
        {
            if (HandleInstanceRedirect())
                return;
        }
        catch (Exception ex)
        {
            var redirectEx = new Exception("Failed redirect to existing app instance", ex);
            WriteExceptionToEventViewer(redirectEx);
        }

        HandleOAuthRedirect();

        SetupMappers();
    }

    private static void SetupMappers()
    {
        // https://github.com/dotnet/maui/issues/16066#issuecomment-2058487452

        CollectionViewHandler.Mapper.AppendToMapping(
            "DisableMultiselectCheckbox",
            (handler, view) =>
            {
                handler.PlatformView.IsMultiSelectCheckBoxEnabled = false;
            }
        );
    }

    private bool HandleOAuthRedirect()
    {
        bool isOAuthRedirect = WebAuthenticator.CheckOAuthRedirectionActivation();

        if (!isOAuthRedirect)
            WebAuthenticator.PromptAuthentication += WebAuthenticator_PromptForCredentials;

        return isOAuthRedirect;
    }

    private bool HandleInstanceRedirect()
    {
        var keyInstance = AppInstance.FindOrRegisterForKey(nameof(VisitzApp));

        if (!keyInstance.IsCurrent)
        {
            var args = AppInstance.GetCurrent().GetActivatedEventArgs();
            WindowUtil.RedirectActivationTo(args, keyInstance);
            return true;
        }

        return false;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private void App_UnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        WriteExceptionToEventViewer(e.Exception);
    }

    private void WriteExceptionToEventViewer(Exception exception)
    {
        string category = GetType()?.FullName ?? typeof(App).FullName ?? "<NULL CATEGORY>";
        EventLogWriter.WriteEntry(LogLevel.Error, exception.Message, category, exception: exception);
    }

    private async void WebAuthenticator_PromptForCredentials(object? sender, InvokingAuthEventArgs e)
    {
        var webViewPage = ServiceProvider.GetService<WebViewPage>();

        webViewPage.AuthUri = e.Uri;
        webViewPage.CancelTokenSource = AuthCancelTokenSource;

        await Navigator.Navigation.PushModalAsync(webViewPage);
    }
}
