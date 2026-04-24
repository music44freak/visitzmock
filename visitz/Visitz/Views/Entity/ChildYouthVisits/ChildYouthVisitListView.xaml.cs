using CommunityToolkit.Mvvm.Messaging;
using Visitz.Resources.Localization;
using Visitz.Services;
using Visitz.Services.Visits;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.ChildYouthVisits;

#nullable enable

public partial class ChildYouthVisitListView
    : IcmRecordContentView<ChildYouthVisitListViewModel>,
        IRequestedEntitySection,
        IRecipient<ServiceStateMessage>
{
    bool _disposed;

    public EntitySection RequestedSection
    {
        get => ViewModel.RequestedSection;
        set => ViewModel.RequestedSection = value;
    }

    public ChildYouthVisitListView()
        : base(ServiceProvider.GetService<ChildYouthVisitListViewModel>(), LocalizedStrings.ChildYouthVisits)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        WeakReferenceMessenger.Default.Register(this, PostAndRefreshVisitService.MakeId(ViewModel.BusinessObject.Id));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);

            _disposed = true;
        }
        base.Dispose(disposing);
    }

    public void Receive(ServiceStateMessage message)
    {
        if (message.FinishedSuccess)
            VisitsCollectionView.ScrollTo(0);
    }
}
