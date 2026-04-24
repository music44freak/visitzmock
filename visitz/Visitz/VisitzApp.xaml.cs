using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Oidc;
using Oidc.Events;
using Visitz.Services;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.AppLock;
using Visitz.Views.Debugging;
using Visitz.Views.Root;

namespace Visitz;

public partial class VisitzApp : Application, IRecipient<AppLockMessage>
{
    public ServiceHandler ServiceHandler { get; private set; }

    private readonly ILogger<VisitzApp> _logger;

    public VisitzApp(ILogger<VisitzApp> logger)
    {
        _logger = logger;

        OidcSession.SessionChanged += OidcSession_SessionChanged;

        InitializeComponent();

        TryStartDebugSensor();

        StrongReferenceMessenger.Default.RegisterAll(this);
    }

    protected override void OnStart()
    {
        base.OnStart();

        ServiceHandler = ServiceProvider.Current.GetService<ServiceHandler>();

        TryClearLogs();
        CleanupStaleRecords();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        return new VisitzWindow(new NavigationPage(new RootPage()));
    }

    private static void TryStartDebugSensor()
    {
        DebugOptions.TryStartShakeDetector(actionOnShake: async () => await DebugOptionsPage.TryOpen());
    }

    private void OidcSession_SessionChanged(object? sender, SessionChangedEventArgs e)
    {
        if (e is LogoutChangedEventArgs args && args.Success)
            _ = ClearIcmData();
    }

    private static async Task ClearIcmData()
    {
        await (await VisitzRealms.GetIcmDataAsync()).ClearAllData();
    }

    private void TryClearLogs()
    {
        try
        {
            _ = ClearRealmLogs.ClearLogData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private static void CleanupStaleRecords()
    {
        WeakReferenceMessenger.Default.Send(RecordCleanupService.MakeStartMessage());
    }

    public void Receive(AppLockMessage message)
    {
        if (message.Value == AppLockStatus.Closed)
            WeakReferenceMessenger.Default.Send(AutoRefreshService.MakeStartMessage());
    }
}
