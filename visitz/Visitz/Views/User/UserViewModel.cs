using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oidc;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Settings;
using Visitz.Views.BaseClasses;
using VisitzModel.Messaging;

namespace Visitz.Views.User;

public partial class UserViewModel : VisitzViewModel
{
    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public string buildNumber = AppInfo.Current.BuildString;

    [ObservableProperty]
    public string appVersion = AppInfo.Current.VersionString;

    [ObservableProperty]
    public string feedbackUrl;

    OidcSessionInfo SessionInfo;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        SessionInfo = await OidcSessionInfo.GetAsync();
        DisplayName = SessionInfo.GivenName;

        var contactInfo = new AppSettings().ContactInfo;
        FeedbackUrl = contactInfo.FeedbackSurveyUrl;

        OidcSession.SessionChanged += OidcSession_SessionChanged;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            OidcSession.SessionChanged -= OidcSession_SessionChanged;
            disposed = true;
        }
        base.Dispose(disposing);
    }

    [RelayCommand]
    static async Task OpenCollectionNotice()
    {
        var noticeView = ServiceProvider.GetService<CollectionNoticeView>();
        await Navigator.Navigation.PushModalAsync(noticeView, ViewModalSize.Fullscreen);
        CloseNavDrawer();
    }

    [RelayCommand]
    async Task OpenFeedbackUrl()
    {
        await Browser.Default.OpenAsync(
            FeedbackUrl,
            new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Hide,
                Flags = BrowserLaunchFlags.PresentAsPageSheet,
            }
        );
        CloseNavDrawer();
    }

    [RelayCommand]
    public static void TryLogout()
    {
        _ = PromptAndLogoutAsync();
    }

    static async Task PromptAndLogoutAsync()
    {
        if (await PromptLogout())
            await OidcSession.LogoutAsync();
    }

    static async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel
        );
    }

    async void OidcSession_SessionChanged(object? sender, Oidc.Events.SessionChangedEventArgs e)
    {
        if (!await OidcSession.SessionExistsAsync())
        {
            // Delay was the only thing I could do to get this working. App
            // wasn't playing nice on Windows waiting for WebViewPage to close
            // and no UI updates were firing.
            await Task.Delay(100);

            await GoToLoginScreen();
        }
    }

    static async Task GoToLoginScreen()
    {
        try
        {
            CloseNavDrawer();
            await SessionPage.TryOpenAsync(animated: false);
        }
        catch (InvalidOperationException) { }
    }

    static void CloseNavDrawer()
    {
        StrongReferenceMessenger.Default.Send(new NavDrawerMessage(isOpen: false));
    }
}
