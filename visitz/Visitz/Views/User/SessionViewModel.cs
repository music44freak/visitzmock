using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Views.AppLock;
using Visitz.Views.BaseClasses;
using Visitz.Views.Debugging;
using DisplayOptions = Visitz.Views.FeaturedBackgroundUnderlay.DisplayOptions;
#if WINDOWS
using Visitz.WinUI;
#endif

namespace Visitz.Views.User;

public partial class SessionViewModel(ILogger<SessionViewModel> logger)
    : VisitzViewModel,
        IRecipient<ServiceStateMessage>,
        IRecipient<AppLockMessage>
{
    [ObservableProperty]
    public string buildNumber = AppInfo.Current.BuildString;

    [ObservableProperty]
    public string appVersion = AppInfo.Current.VersionString;

    [ObservableProperty]
    public DisplayOptions bgDisplayOptions = DisplayOptions.Clear;

    [ObservableProperty]
    public bool showLoginLayout;

    [ObservableProperty]
    public string displayName;

    [ObservableProperty]
    public bool showAuthStatusLayout;

    [ObservableProperty]
    public string authStatus;

    [ObservableProperty]
    public bool showAuthStatus;

    [ObservableProperty]
    public bool isAuthorized;

    [ObservableProperty]
    public bool isUnauthorized;

    [ObservableProperty]
    public bool tryingAuthorization;

    [ObservableProperty]
    public bool showButtons;

    [ObservableProperty]
    public bool showUnknown;

    [ObservableProperty]
    public bool staleSession;

    public Action AuthorizationSuccess { get; set; }

    private OidcSessionInfo SessionInfo;

    protected override ILogger<VisitzViewModel> Logger { get; } = logger;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        SessionInfo = await OidcSessionInfo.GetAsync();
        DisplayName = SessionInfo.GivenName;

        if (await ApplyLayoutByStatus() is (bool, _) sessionStatus)
        {
#if IOS
            // Having issues with lifecycle timings on iOS and this delay solves
            // it wonderfully. Not ideal but it works.
            await Task.Delay(100);
#endif
            if (!AppLockPage.IsOpen && sessionStatus.SessionExists && NetworkHelper.InternetAvailable)
                // If AppLockPage is open, it will auto prompt to authenticate.
                // This will cause an error if VisitzApiService needs to prompt
                // user for login, and the user will be stuck at a blank screen
                // in this page.
                await DownloadCaseloadAndSubscribeAsync();
        }

        StrongReferenceMessenger.Default.Register<AppLockMessage>(this);
        OidcSession.SessionChanged += OidcSession_SessionChanged;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            OidcSession.SessionChanged -= OidcSession_SessionChanged;
            StrongReferenceMessenger.Default.UnregisterAll(this);
            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void SetUiOptions(
        bool showLoginLayout = false,
        bool? showAuthStatusLayout = null,
        bool? showAuthStatus = null,
        bool isAuthorized = false,
        bool isUnauthorized = false,
        bool tryingAuthorization = false,
        bool? showButtons = null,
        bool showUnknown = false,
        bool staleSession = false
    )
    {
        ShowLoginLayout = showLoginLayout;
        IsAuthorized = isAuthorized;
        IsUnauthorized = isUnauthorized;
        TryingAuthorization = tryingAuthorization;
        ShowButtons = showButtons ?? (showLoginLayout ? false : !IsAuthorized || IsUnauthorized);
        ShowUnknown = showUnknown;
        StaleSession = staleSession;

        BgDisplayOptions = showLoginLayout ? DisplayOptions.Clear : DisplayOptions.TextReadable;

        if (tryingAuthorization)
            AuthStatus = LocalizedStrings.CheckingIcmProfile;
        else if (staleSession)
            AuthStatus = LocalizedStrings.StaleSessionDesc;
        else if (isUnauthorized)
            AuthStatus = LocalizedStrings.LoginSuccessButUnauth;
        else if (!isAuthorized)
            AuthStatus = LocalizedStrings.NeedToConfirm;
        else
            AuthStatus = string.Empty;

        if (showLoginLayout)
        {
            ShowAuthStatusLayout = false;
            ShowAuthStatus = false;
        }
        else
        {
            ShowAuthStatusLayout = showAuthStatusLayout ?? AuthStatus?.Length > 0;
            ShowAuthStatus = showAuthStatus ?? AuthStatus?.Length > 0;
        }
    }

    private async Task<bool?> ApplyAuthStatusLayout(bool? showUnknown = null, bool? isAuthorized = null)
    {
        bool? authorized = isAuthorized ?? await OidcSession.IsAuthorizedAsync();

        SetUiOptions(
            showAuthStatusLayout: true,
            showAuthStatus: true,
            isAuthorized: authorized ?? false,
            isUnauthorized: !authorized ?? false,
            showUnknown: showUnknown ?? authorized == null,
            showButtons: true
        );

        return authorized;
    }

    private async void OidcSession_SessionChanged(object? sender, SessionChangedEventArgs e)
    {
        SessionInfo = sender as OidcSessionInfo;
        DisplayName = SessionInfo.GivenName;
        await ApplyLayoutByStatus();
    }

    private async Task<(bool SessionExists, bool? IsAuthorized)> ApplyLayoutByStatus()
    {
        if (await OidcSession.SessionExistsAsync())
        {
            bool? isAuthorized = await OidcSession.IsAuthorizedAsync();

            if (isAuthorized is true)
            {
                if (await OidcSession.IsSessionStale(DebugOptions.StaleThresholdMinutes) ?? false)
                    SetUiOptions(showButtons: true, staleSession: true);
            }
            else if (isAuthorized is false)
                await ApplyAuthStatusLayout(isAuthorized: false);
            else
                // "Idle" layout
                SetUiOptions(showButtons: true);

            return (true, isAuthorized);
        }
        else
        {
            SetUiOptions(showLoginLayout: true);
            return (false, null);
        }
    }

    [RelayCommand]
    public void Login()
    {
        _ = LoginAsync();
    }

    public async Task LoginAsync()
    {
        try
        {
            var cancelToken = new CancellationTokenSource();
#if WINDOWS
            (MauiWinUIApplication.Current as App).AuthCancelTokenSource = cancelToken;
#endif
            await OidcSession.LoginAsync(messageIfUnavailable: LocalizedStrings.NoInternet, cancelToken.Token);

            await ApplyAuthStatusLayout(showUnknown: false);

            await DownloadCaseloadAndSubscribeAsync();
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
                Logger.LogError(ex, ex.Message);
        }
    }

    [RelayCommand]
    public void TryLogout()
    {
        _ = PromptAndLogoutAsync();
    }

    private async Task PromptAndLogoutAsync()
    {
        if (await PromptLogout())
        {
            SetUiOptions(showLoginLayout: true);
            await OidcSession.LogoutAsync();
        }
    }

    private static async Task<bool> PromptLogout()
    {
        return await Navigator.CurrentOpenPage.DisplayAlertAsync(
            LocalizedStrings.LogoutAndClearData,
            LocalizedStrings.LogoutAndClearDataDesc,
            LocalizedStrings.Logout,
            LocalizedStrings.Cancel
        );
    }

    [RelayCommand]
    public async Task DownloadCaseloadAndSubscribeAsync()
    {
        if (!NetworkHelper.InternetAvailable)
        {
            await Navigator.CurrentOpenPage.DisplayAlertAsync(
                LocalizedStrings.NoInternet,
                LocalizedStrings.ConnectBeforeRetry,
                LocalizedStrings.Ok
            );
            return;
        }

        WeakReferenceMessenger.Default.Register<ServiceStateMessage, string>(this, GetCaseloadService.MakeId());

        var msg = GetAllDataForOfflineService.MakeStartMessage(forceDownload: true);
        WeakReferenceMessenger.Default.Send(msg);

        // extra SetUiOptions call before Receive() so "unauthorized" UI
        // doesn't flash
        SetUiOptions(showAuthStatusLayout: true, showAuthStatus: true, tryingAuthorization: true);
    }

    public void Receive(ServiceStateMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (message.Status == VisitzService.State.Running)
            {
                SetUiOptions(showAuthStatusLayout: true, showAuthStatus: true, tryingAuthorization: true);
            }
            else
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);

                if (message.Result == VisitzService.Result.Successful)
                    AuthorizationSuccess();
                else
                {
                    SetUiOptions(showAuthStatusLayout: true, showAuthStatus: true, isUnauthorized: true);
                }
            }
        });
    }

    public void Receive(AppLockMessage message)
    {
        if (message.Value == AppLockStatus.Closed && NetworkHelper.InternetAvailable)
            _ = DownloadCaseloadAndSubscribeAsync();
    }
}
