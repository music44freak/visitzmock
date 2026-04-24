using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Oidc.Network;
using Realms;
using Visitz.Extensions;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.BaseClasses.Publishing;
using VisitzModel.Extensions;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Resources.Localization;

namespace Visitz.Views.Entity.ChildYouthVisits;

#nullable enable

public partial class ChildYouthVisitViewModel : IcmRecordViewModel
{
    public static readonly string VisitTypeGroup = "VisitTypeGroup";
    public static readonly string VisitDetailGroup = "VisitDetailGroup";
    public static readonly int CharacterLimit = 4000;
    public static readonly string RemainingCharactersString = "{0}/" + CharacterLimit;

    private bool _disposed;

    Realm? DraftRealm { get; set; }

    CaseRecord? Case { get; set; }

    [ObservableProperty]
    PersonVisitDraft? draft;

    [ObservableProperty]
    public DateTime maxDate = DateTimeExtensions.LocalNow;

    [ObservableProperty]
    public string visitDescription = "";

    [ObservableProperty]
    public DateTimeOffset dateOfVisit;

    [ObservableProperty]
    public bool isUpdatingEnabled = true;

    [ObservableProperty]
    public bool hideElements = true;

    [ObservableProperty]
    public bool allowDiscard;

    [ObservableProperty]
    public bool allowPublish;

    [ObservableProperty]
    public int remainingCharacters = CharacterLimit;

    [ObservableProperty]
    public int characterCount;

    [ObservableProperty]
    public PersonVisit? personVisitItem;

    [ObservableProperty]
    public bool showFullForm = true;

    [ObservableProperty]
    public GridLength detailsRowHeight = GridLength.Star;

    public DraftSaveStateHandler SaveStateHandler { get; } = new();

    [ObservableProperty]
    public List<VisitDetailListItem> detailItems = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        DraftRealm = await VisitzRealms.GetPersonVisitDraftsRealmAsync();
        Case = DataRealm.All<CaseRecord>().Where(@case => @case.FileNumber == BusinessObject.FileNumber).First();

        if (PersonVisitItem == null && IsUpdatingEnabled)
            Draft = PersonVisitDraft.GetDraft(DraftRealm, Case.Id) ?? new(Case);

        if (PersonVisitItem == null)
        {
            string error = "Unable to load visit record";
            Logger.LogError(error);
            await Navigator.CurrentOpenPage.DisplayErrorAlert(error);
            return;
        }

        DetailItems =
        [
            new(PersonVisitDetails.Api_ExemptionChildDeclined, PersonVisitItem),
            new(PersonVisitDetails.Api_ExemptionOther, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivateInHome, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivatePlanning, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivateRelational, PersonVisitItem),
            new(PersonVisitDetails.Api_NotPrivateWithCaregiver, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitAge0_5, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitInHome, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitMedicalSupportNeeds, PersonVisitItem),
            new(PersonVisitDetails.Api_PrivateVisitNotInHome, PersonVisitItem),
        ];

        AddOtherVisitDetails();

        if (!IsUpdatingEnabled)
            DetailItems = DetailItems.Where(item => item.IsChecked).ToList();

        SaveStateHandler.Clear();
        UpdateAllowPublish();

        Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
    }

    void AddOtherVisitDetails()
    {
        if (PersonVisitItem == null)
            return;

        var knownDetails = DetailItems.Select(item => item.DetailValue).ToList();
        var otherDetails = PersonVisitItem.VisitDetails.Except(knownDetails);

        foreach (var other in otherDetails)
            DetailItems.Add(new VisitDetailListItem(other, PersonVisitItem));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;

            Draft = null;
            PersonVisitItem = null;

            SaveStateHandler.Dispose();

            DraftRealm?.Dispose();
            DraftRealm = null;

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    partial void OnPersonVisitItemChanged(PersonVisit? oldValue, PersonVisit? newValue)
    {
        if (oldValue != null)
            oldValue.PropertyChanged -= PersonVisitItem_PropertyChanged;

        if (newValue != null)
        {
            newValue.PropertyChanged += PersonVisitItem_PropertyChanged;
            UpdateAllowPublish();
        }
    }

    private async void PersonVisitItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!IsUpdatingEnabled)
            return;

        await HandleDraft();

        if (e.PropertyName != nameof(PersonVisit.VisitDescription))
            // Skip VisitDescription updates, because we want to use EditorEx's
            // character count instead of a race condition from directly reading
            // VisitDescription.Length.
            UpdateAllowPublish();
    }

    [RelayCommand]
    public async Task PublishInPersonVisit()
    {
        if (!AllowPublish)
            return;

        await Navigator.Navigation.PopModalAsync();

        var publishVm = ServiceProvider.GetService<ChildYouthVisitPublishViewModel>();
        publishVm.BusinessObject = BusinessObject;

        await Navigator.Navigation.PushAsync(new PublishPage(publishVm));
    }

    public void DiscardDraft()
    {
        string? id = Draft?.RelatedEntityId;
        Draft = null;

        if (id != null)
            DraftRealm?.Write(() => DraftRealm.DeleteByIds<PersonVisitDraft>([id]));
    }

    private void UpdateAllowPublish()
    {
        AllowPublish =
            NetworkHelper.InternetAvailable
            && PersonVisitItem?.VisitDetails.Count > 0
            && PersonVisitItem?.VisitDescription?.Length > 0
            && CharacterCount <= CharacterLimit;
    }

    TaskCompletionSource? DraftInitTcs;

    private async Task HandleDraft()
    {
        if (Draft == null)
            return;

        if (DraftInitTcs != null)
            await DraftInitTcs.Task;

        if (PersonVisitItem != null && !PersonVisitItem.IsManaged && BusinessObject != null && Case != null)
        {
            DraftInitTcs = new();

            Draft = await PersonVisitDraft.Upsert(DraftRealm, Case.Id, PersonVisitItem, BusinessObject.DisplayName);

            DraftInitTcs.TrySetResult();
        }
        else if (Draft?.IsValid ?? false)
            Draft.LastUpdatedBinding = DateTimeOffset.Now;

        _ = SaveStateHandler.Saving();
    }

    partial void OnCharacterCountChanged(int value)
    {
        RemainingCharacters = CharacterLimit - value;
        UpdateAllowPublish();
    }

    partial void OnDraftChanged(PersonVisitDraft? value)
    {
        PersonVisitItem = value?.Visit;
        AllowDiscard = value?.IsManaged ?? false;
    }

    partial void OnShowFullFormChanged(bool value)
    {
        DetailsRowHeight = value ? GridLength.Star : 0;
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        UpdateAllowPublish();
    }
}
