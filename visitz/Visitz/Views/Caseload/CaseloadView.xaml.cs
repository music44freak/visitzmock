using CommunityToolkit.Mvvm.Messaging;
using Oidc;
using Visitz.Extensions;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Caseload;
using Visitz.Views.BaseClasses;
#if !MACCATALYST
using CommunityToolkit.Maui.Core.Platform;
#endif

namespace Visitz.Views.Caseload;

public partial class CaseloadView : ViewModelContentView<CaseloadViewModel>, IRecipient<ServiceStateMessage>
{
    public CaseloadView()
        : base(ServiceProvider.GetService<CaseloadViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());
    }

    protected override void Dispose(bool disposing)
    {
        /* Overriding to a no-op because class is a DI singleton */
    }

    private void Picker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        ViewModel.Lister.ApplyWithFilter();
    }

#if MACCATALYST
    private void CaseloadSearchBar_SearchButtonPressed(object? sender, EventArgs e)
#else
    private async void CaseloadSearchBar_SearchButtonPressed(object? sender, EventArgs e)
#endif
    {
        ViewModel.SearchCaseload();

#if !MACCATALYST
        await CaseloadSearchBar.HideKeyboardAsync(CancellationToken.None);
#endif
    }

    private void CaseloadSearchBar_TextChanged(object? sender, TextChangedEventArgs e)
    {
        ViewModel.SearchCaseload();
    }

    private void ListOptionsButton_Clicked(object? sender, EventArgs e)
    {
        OptionsLayout.IsVisible = !OptionsLayout.IsVisible;
    }

    private void ClearFilterButton_Clicked(object? sender, EventArgs e)
    {
        ViewModel.ActivatedFilterOption = null;
    }

    public async void Receive(ServiceStateMessage message)
    {
        if (message.FinishedError && (await OidcSession.IsAuthorizedAsync() ?? false))
        {
            await Navigator.CurrentOpenPage.DisplayErrorAlert(
                LocalizedStrings.CaseloadErrorMessage,
                message.UncaughtException?.ToString(),
                LocalizedStrings.CaseloadError
            );
        }
    }
}
