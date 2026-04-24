using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Maui.Toolkit.TabView;
using Visitz.Views.BaseClasses;
using Visitz.Views.Entity.Attachments;
using Visitz.Views.Entity.ChildYouthVisits;
using Visitz.Views.Entity.Details;
using Visitz.Views.Entity.FamilyMembers;
using Visitz.Views.Entity.Notes;
using Visitz.Views.Entity.SafetyAssess;
using Visitz.Views.Entity.SupportNetwork;
using Visitz.VisitzConfig;
using VisitzModel.Interfaces;
using VisitzModel.Models.EntityTypes;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityViewModel
{
    [ObservableProperty]
    public TabItemCollection tabItems = [];

    [ObservableProperty]
    public int selectedIndex;

    [ObservableProperty]
    public SfTabItem? selectedTab;

    readonly List<BaseContentView> _viewsToDispose = [];

    [ObservableProperty]
    public double tabBarHeight;

    [ObservableProperty]
    public double tabListButtonWidth;

    SfTabItem MakeTab<TContentView>()
        where TContentView : BaseContentView
    {
        TContentView view = ServiceProvider.GetService<TContentView>();

        if (view is IIcmRecordInfo info)
        {
            info.RowId = RowId;
            info.EntityType = EntityType;
        }

        if (view is IRequestedEntitySection sectionView)
            sectionView.RequestedSection = RequestedSection ?? EntitySection.Unknown;

        if (view is IFocusDraftItem focusView)
            focusView.FocusedDraftItem = FocusedDraftItem;

        _viewsToDispose.Add(view);

        return new()
        {
            Header = view.Title,
            Content = view,
            FontFamily = VisitzFonts.BcSansRegularAlias,
        };
    }

    void BuildNavList()
    {
        TabItems.Add(MakeTab<EntityDetailsView>());
        TabItems.Add(MakeTab<EntityContactsView>());
        TabItems.Add(MakeTab<EntityNotesView>());
        TabItems.Add(MakeTab<AttachmentsView>());

        if (ShouldShowSafetyAssessment())
            TabItems.Add(MakeTab<SafetyAssessmentListView>());

        if (ShouldShowChildYouthVisits())
            TabItems.Add(MakeTab<ChildYouthVisitListView>());

        if (ShouldShowSupportNetwork())
            TabItems.Add(MakeTab<SupportNetworkListView>());
    }

    void DisposeTabViews()
    {
        foreach (var item in _viewsToDispose)
            item.Dispose();

        TabItems.Clear();
    }

    SfTabItem? GetTabByType<T>()
        where T : BaseContentView
    {
        return TabItems.FirstOrDefault(t => t.Content is T);
    }

    SfTabItem? GetMappedNavItem(EntitySection? section)
    {
        return section switch
        {
            EntitySection.Family => GetTabByType<EntityContactsView>(),
            EntitySection.Notes or EntitySection.NoteEntry => GetTabByType<EntityNotesView>(),
            EntitySection.Attachments => GetTabByType<AttachmentsView>(),
            EntitySection.SafetyAssessment or EntitySection.SafetyAssessmentEntry =>
                GetTabByType<SafetyAssessmentListView>(),
            EntitySection.ChildYouthVisits or EntitySection.ChildYouthVisitsEntry =>
                GetTabByType<ChildYouthVisitListView>(),
            EntitySection.SupportNetwork => GetTabByType<SupportNetworkListView>(),
            _ => GetTabByType<EntityDetailsView>(),
        };
    }

    partial void OnSelectedIndexChanged(int value)
    {
        SelectedTab = value != -1 ? TabItems.ElementAt(value) : null;
    }

    partial void OnSelectedTabChanged(SfTabItem? value)
    {
        SelectedIndex = value != null ? TabItems.IndexOf(value) : -1;
    }

    bool ShouldShowSafetyAssessment()
    {
        return BusinessObject.EntityType == EntityType.Incident;
    }

    bool ShouldShowChildYouthVisits()
    {
        return BusinessObject.EntityType == EntityType.Case
            && BusinessObject.EntitySubtype == EntitySubtype.ChildServices;
    }

    bool ShouldShowSupportNetwork()
    {
        return BusinessObject.EntityType == EntityType.Case
            || BusinessObject.EntityType == EntityType.Incident
            || BusinessObject.EntityType == EntityType.ServiceRequest;
    }
}
