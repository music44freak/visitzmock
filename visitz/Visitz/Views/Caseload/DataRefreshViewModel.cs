using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Oidc.Network;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Caseload;

public partial class DataRefreshViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
{
    [ObservableProperty]
    public bool superMessageVisible = false;

    [ObservableProperty]
    public string superMessage;

    [ObservableProperty]
    public StackOrientation orientation = StackOrientation.Vertical;

    [ObservableProperty]
    public bool caseloadActivity;

    protected override Task InitAsync()
    {
        base.InitAsync();

        SetConnectivityMessage();
        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

        return Task.CompletedTask;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        SetConnectivityMessage();
    }

    private void SetConnectivityMessage()
    {
        SuperMessage = NetworkHelper.InternetAvailable ? "" : LocalizedStrings.Offline;
    }

    [RelayCommand]
    public static void RefreshData()
    {
        WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage(forceDownload: true));
    }

    partial void OnSuperMessageChanged(string value)
    {
        SuperMessageVisible = !string.IsNullOrWhiteSpace(value);
    }

    public void Receive(ServiceStateMessage message)
    {
        CaseloadActivity = message.Status == VisitzService.State.Running;
    }

    [RelayCommand]
    public void HideIndicator()
    {
        CaseloadActivity = false;
    }
}
