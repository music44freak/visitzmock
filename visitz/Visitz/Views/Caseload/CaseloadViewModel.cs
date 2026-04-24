using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Foldable;
using Oidc;
using Realms;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Base;
using Visitz.Services.Caseload;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using Visitz.Views.SegmentedButtons;
using VisitzModel.Events;
using VisitzModel.Extensions;
using VisitzModel.Messaging;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Storage;
using IBusinessObjectExtensions = VisitzModel.Models.Caseload.IBusinessObjectExtensions;

#nullable enable

namespace Visitz.Views.Caseload
{
    /// <summary>
    /// The business logic for the cases and incidents list rendering goes here.
    /// </summary>
    public partial class CaseloadViewModel : VisitzViewModel, IRecipient<ServiceStateMessage>
    {
#if WINDOWS
        private static readonly string PromptText = LocalizedStrings.ButtonToRefreshCaseload;
#else
        private static readonly string PromptText = LocalizedStrings.PullToRefreshCaseload;
#endif

        private static readonly string SortOptionIndexPref = "SortOptionIndexPref";

        private static readonly SegmentedOptions SortKeyPlayer = new(
            nameof(LocalizedStrings.KeyPlayer),
            LocalizedStrings.KeyPlayer,
            MaterialIcons.Person.GetUnfilledMaterialIcon()
        );

        private static readonly SegmentedOptions SortOpenDate = new(
            nameof(IBusinessObject.DisplayDate),
            LocalizedStrings.OpenDate,
            MaterialIcons.Calendar_month.GetUnfilledMaterialIcon()
        );

        private static readonly SegmentedOptions FilterCase = new(
            nameof(EntityType.Case),
            LocalizedStrings.Cases,
            MaterialIcons.Folder.GetUnfilledMaterialIcon()
        );

        private static readonly SegmentedOptions FilterIncident = new(
            nameof(EntityType.Incident),
            LocalizedStrings.Incidents,
            MaterialIcons.Description.GetUnfilledMaterialIcon()
        );

        private static readonly List<string> StartingOfficeFilterOptions =
        [
            LocalizedStrings.All,
            LocalizedStrings.MyCaseload,
        ];

        [ObservableProperty]
        public CaseloadLister? lister;

        [ObservableProperty]
        public bool isRefreshing;

        [ObservableProperty]
        public string? searchQuery;

        [ObservableProperty]
        public bool showEmptyCaseloadMessage;

        [ObservableProperty]
        public string? collectionViewPrompt;

        [ObservableProperty]
        public bool isFilterActivated;

        private Realm? Realm { get; set; }

        [ObservableProperty]
        public SegmentedOptions? activatedSortOption;

        [ObservableProperty]
        public SegmentedOptions? activatedFilterOption;

        [ObservableProperty]
        public IList<SegmentedOptions> sortOptions = [SortKeyPlayer, SortOpenDate];

        [ObservableProperty]
        public IList<SegmentedOptions> filterOptions = [FilterCase, FilterIncident];

        [ObservableProperty]
        public bool showMenuButton;

        [ObservableProperty]
        public DraftIndicatorHelper indicatorHelper = new();

        [ObservableProperty]
        public ObservableCollection<string> officeNames = [];

        [ObservableProperty]
        public string? selectedOffice;

        OidcSessionInfo? SessionInfo { get; set; }

        LastUpdatedPrefs LastUpdatedPrefs { get; set; } = ServiceProvider.GetService<LastUpdatedPrefs>();

        [ObservableProperty]
        public DateTime? lastUpdated;

        private async Task Setup()
        {
            WeakReferenceMessenger.Default.Register(this, GetAllDataForOfflineService.MakeId());

            SessionInfo = await OidcSession.GetInfoAsync();
            SetupOfficeNames();
            SessionInfo.OfficesChanged += SessionInfo_OfficesChanged;

            await SetupCaseloadList();

            int sortPrefIndex = Preferences.Default.Get(SortOptionIndexPref, 0);
            ActivatedSortOption = SortOptions.ElementAt(sortPrefIndex);

            ShowEmptyCaseloadMessage = false;
            CollectionViewPrompt = PromptText;

            DeviceDisplay.Current.MainDisplayInfoChanged += Current_MainDisplayInfoChanged;
            ShowMenuButton = DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait;

            LastUpdated = LastUpdatedPrefs.Get(GetCaseloadService.MakeId());
            LastUpdatedPrefs.LastUpdatedChanged += LastUpdatedPrefs_LastUpdatedChanged;

            StrongReferenceMessenger.Default.Register<NavPositionMessage>(this, ReceiveNavBarPositionMessage);
            ShowMenuButton =
                StrongReferenceMessenger.Default.Send(new GetNavPositionMessage()) == (int)TwoPaneViewMode.Tall;

            WeakReferenceMessenger.Default.Send(AutoRefreshService.MakeStartMessage());
        }

        private void SetupOfficeNames(HashSet<string>? newOffices = null)
        {
            if (newOffices == null)
            {
                OfficeNames.Clear();
                foreach (var starter in StartingOfficeFilterOptions)
                    OfficeNames.Add(starter);

                if (SessionInfo != null)
                    foreach (var office in SessionInfo.OfficeNames.AsEnumerable().Order())
                        OfficeNames.Add(office);

                SelectedOffice = LocalizedStrings.MyCaseload;
            }
            else
            {
                string currentSelected = SelectedOffice ?? LocalizedStrings.MyCaseload;

                UpdateSortedOfficeNames(newOffices);

                if (currentSelected != SelectedOffice)
                {
                    SelectedOffice = OfficeNames.Contains(currentSelected)
                        ? currentSelected
                        : LocalizedStrings.MyCaseload;
                }
            }
        }

        private void UpdateSortedOfficeNames(HashSet<string> newOffices)
        {
            // Skip to account for always-available options
            int offset = StartingOfficeFilterOptions.Count;

            List<string> current = OfficeNames.Skip(offset).ToList();

            foreach (var addOffice in newOffices.Except(current))
            {
                int index = current.BinarySearch(addOffice);
                if (index < 0)
                    index = ~index;
                int insertPosition = index + offset;

                if (insertPosition < OfficeNames.Count)
                {
                    current.Insert(index, addOffice);
                    OfficeNames.Insert(insertPosition, addOffice);
                }
                else
                {
                    current.Add(addOffice);
                    OfficeNames.Add(addOffice);
                }
            }

            foreach (var removeOffice in current.Except(newOffices))
                OfficeNames.Remove(removeOffice);
        }

        private void SessionInfo_OfficesChanged(object? sender, HashSet<string> offices)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SetupOfficeNames(offices);
                Lister?.ApplyWithFilter();
            });
        }

        private async Task SetupCaseloadList()
        {
            await IndicatorHelper.InitAsync();

            Realm = await VisitzRealms.GetIcmDataRealmAsync();

            if (SessionInfo == null)
                return;

            Lister = new CaseloadLister(
                Realm,
                IndicatorHelper,
                SessionInfo,
                list =>
                {
                    list = ApplySorting(list) ?? [];
                    list = ApplySearchQuery(list) ?? [];
                    list = ApplySubtypeFilter(list) ?? [];
                    list = ApplyOfficeFilter(list) ?? [];
                    return list ?? [];
                }
            );

            Lister.Records.CollectionChanged += Records_CollectionChanged;
        }

        private void Records_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ApplyCollectionViewPrompt();
        }

        private void Teardown()
        {
            DeviceDisplay.Current.MainDisplayInfoChanged -= Current_MainDisplayInfoChanged;

            WeakReferenceMessenger.Default.UnregisterAll(this);

            IndicatorHelper?.Dispose();

            Lister?.Dispose();

            Realm?.Dispose();
            Realm = null;

            SessionInfo?.OfficesChanged -= SessionInfo_OfficesChanged;
            SessionInfo = null;
        }

        protected override async Task InitAsync()
        {
            await base.InitAsync();

            await Setup();
        }

        bool disposed;

        protected override void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                Teardown();

                disposed = true;
            }
            base.Dispose(disposing);
        }

        private IEnumerable<IBusinessObject>? ApplySorting(IEnumerable<IBusinessObject>? query)
        {
            if (query == null || ActivatedSortOption == null)
                return query;

            if (ActivatedSortOption == SortOpenDate)
            {
                query = query.OrderBy(IBusinessObjectExtensions.DisplayDateTransform);
            }
            else if (ActivatedSortOption == SortKeyPlayer)
            {
                var sort = new Func<IBusinessObject, string>(item => item.DisplayName);
                query = query.OrderBy(sort);
            }

            return query;
        }

        private IEnumerable<IBusinessObject>? ApplySearchQuery(IEnumerable<IBusinessObject>? query)
        {
            if (query == null || string.IsNullOrWhiteSpace(SearchQuery))
                return query;

            string trimmedSearch = SearchQuery.Trim();

            return query.Where(item =>
            {
                return item.FileNumber.Contains(trimmedSearch, StringComparison.InvariantCultureIgnoreCase)
                    || item.DisplayName.Contains(trimmedSearch, StringComparison.InvariantCultureIgnoreCase);
            });
        }

        private IEnumerable<IBusinessObject>? ApplySubtypeFilter(IEnumerable<IBusinessObject>? query)
        {
            if (query == null || ActivatedFilterOption == null)
                return query;

            EntityType type;

            if (ActivatedFilterOption.Id == nameof(EntityType.Case))
                type = EntityType.Case;
            else if (ActivatedFilterOption.Id == nameof(EntityType.Incident))
                type = EntityType.Incident;
            else
                return query;

            return query.Where(item => item.EntityType == type);
        }

        private IEnumerable<IBusinessObject>? ApplyOfficeFilter(IEnumerable<IBusinessObject> query)
        {
            if (
                query == null
                || string.IsNullOrWhiteSpace(SelectedOffice)
                || SelectedOffice == LocalizedStrings.MyCaseload
            )
            {
                return query?.Where(bo => bo.IsAssigned(SessionInfo?.Idir ?? string.Empty));
            }
            else if (SelectedOffice == LocalizedStrings.All)
                return query;
            else
                return query.Where(bo => bo.ServiceOffice == SelectedOffice);
        }

        private void ApplyCollectionViewPrompt()
        {
            CollectionViewPrompt = !string.IsNullOrWhiteSpace(SearchQuery)
                ? LocalizedStrings.NoResultsForSearch.Format(SearchQuery)
                : PromptText;
        }

        [RelayCommand]
        public void RefreshCaseload()
        {
            WeakReferenceMessenger.Default.Send(GetAllDataForOfflineService.MakeStartMessage(forceDownload: true));
            ShowEmptyCaseloadMessage = false;
        }

        [RelayCommand]
        public static void OpenNavDrawer()
        {
            StrongReferenceMessenger.Default.Send(new NavDrawerMessage(isOpen: true));
        }

        public void SearchCaseload()
        {
            Lister?.ApplyWithFilter();
        }

        public void Receive(ServiceStateMessage message)
        {
            IsRefreshing = message.Status == VisitzService.State.Running;

            if (message.FinishedSuccess && Realm != null)
            {
                bool anyExist =
                    Realm.All<CaseRecord>().Any()
                    || Realm.All<IncidentRecord>().Any()
                    || Realm.All<MemoRecord>().Any()
                    || Realm.All<ServiceRequestRecord>().Any();

                ShowEmptyCaseloadMessage = !anyExist;
            }
        }

        partial void OnActivatedSortOptionChanged(SegmentedOptions? value)
        {
            if (value != null)
                Preferences.Default.Set(SortOptionIndexPref, SortOptions.IndexOf(value));

            Lister?.ApplyWithFilter();
        }

        partial void OnActivatedFilterOptionChanged(SegmentedOptions? value)
        {
            Lister?.ApplyWithFilter();
            IsFilterActivated = value != null;
        }

        private void Current_MainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e)
        {
            ShowMenuButton = e.DisplayInfo.Orientation == DisplayOrientation.Portrait;
        }

        partial void OnSelectedOfficeChanged(string? value)
        {
            if (Lister != null)
                Lister.ApplyWithFilter();
        }

        void ReceiveNavBarPositionMessage(object recipient, NavPositionMessage message)
        {
            ShowMenuButton = ((TwoPaneViewMode)message.Value) == TwoPaneViewMode.Tall;
        }

        private void LastUpdatedPrefs_LastUpdatedChanged(object? sender, LastUpdatedChangedEventArgs e)
        {
            if (e.Id.Equals(GetCaseloadService.MakeId()))
                LastUpdated = (sender as LastUpdatedPrefs)?.Get(e.Id);
        }
    }
}
