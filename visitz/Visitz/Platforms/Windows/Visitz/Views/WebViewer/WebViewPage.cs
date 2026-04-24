using System.Diagnostics;
using System.Globalization;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using Oidc;
using Visitz.Controls;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Settings;

namespace Visitz.Views.WebViewer;

public partial class WebViewPage
{
    const string _logoutPath = "/logout";
    const string _logoutResponse = "/logout_response";

    Uri _baseRedirectUri;
    Uri _authDomain;

    Func<Task> SessionTask { get; set; }

    partial void Setup()
    {
        var settings = new AppSettings();

        _baseRedirectUri = new(settings.Oidc.RedirectUri);
        _authDomain = new(settings.Oidc.AuthenticationDomain);

        MainWebView.Loaded += MainWebView_Loaded;
        CloseButton.Closing += CloseButton_Closing;
    }

    private static async Task<CoreWebView2> GetCoreWebView(WebView webView)
    {
        var winWebView = webView.Handler.PlatformView as WebView2;

        await winWebView.EnsureCoreWebView2Async();

        if (winWebView.CoreWebView2.Settings is CoreWebView2Settings settings)
        {
            settings.IsZoomControlEnabled = false;
#if DEBUG
            settings.AreDevToolsEnabled = true;
#else
            settings.AreDevToolsEnabled = false;
#endif
        }

        return winWebView.CoreWebView2;
    }

    private async void MainWebView_Loaded(object? sender, EventArgs e)
    {
        var webView = sender as WebView;
        var coreWebView = await GetCoreWebView(webView);

        coreWebView.NavigationStarting += (sender, args) =>
        {
            if (IsLocalRedirect(args.Uri))
            {
                SessionTask = async () => await PerformCustomSchemeRedirect(args.Uri);
            }
            else if (IsLogoutRedirect(args.Uri))
            {
                SessionTask = async () => await ForceLogout(sender);
            }
        };

        coreWebView.NavigationCompleted += async (sender, args) =>
        {
            if (SessionTask != null)
            {
                await SessionTask();
                await Navigator.Navigation.PopModalAsync();
            }
        };

        coreWebView.ProcessFailed += async (_, args) =>
        {
            await DisplayAlertAsync(
                LocalizedStrings.Error,
                args.Reason + "\n\n" + args.ProcessDescription,
                LocalizedStrings.Ok
            );

            await Navigator.Navigation.PopModalAsync();
        };

        // WORKAROUND Windows does not reliably logout currently
        // so we'll forcibly dump our local session and cookies.
        if (IsLogoutRequest(ViewModel.AuthUri))
        {
            await ForceLogout(coreWebView);
            await Navigator.Navigation.PopModalAsync();
            return;
        }

        webView.Source = ViewModel.AuthUri;
    }

    private bool IsLocalRedirect(string url)
    {
        return url.StartsWith(_baseRedirectUri.Scheme, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsLogoutRequest(Uri uri)
    {
        return uri.IsAbsoluteUri
            ? uri.AbsolutePath.EndsWith(_logoutPath, StringComparison.InvariantCultureIgnoreCase)
            : uri.LocalPath.EndsWith(_logoutPath, StringComparison.InvariantCultureIgnoreCase);
    }

    private bool IsLogoutRedirect(string url)
    {
        return url.StartsWith(_authDomain.ToString(), true, CultureInfo.InvariantCulture)
            && url.Contains(_logoutResponse, StringComparison.InvariantCultureIgnoreCase);
    }

    private async Task PerformCustomSchemeRedirect(string uri)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = uri, UseShellExecute = true });
        }
        catch (Exception ex)
        {
            await this.DisplayErrorAlert(ex);
        }
    }

    // A workaround implementation to forcibly logout the user on Windows.
    // This, along with the rest of the Windows OIDC workarounds, may not
    // be needed once https://github.com/microsoft/WindowsAppSDK/issues/441
    // is fixed.
    private async Task ForceLogout(CoreWebView2 coreWebView)
    {
        coreWebView.CookieManager.DeleteAllCookies();

        if (CancelTokenSource != null)
            await CancelTokenSource.CancelAsync();

        await OidcSession.LocalLogoutAsync();
    }

    private void CloseButton_Closing(object? sender, ClosingEventArgs e)
    {
        CancelTokenSource?.Cancel();
    }
}
