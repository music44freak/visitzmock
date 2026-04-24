using Visitz.Views.BaseClasses;

namespace Visitz.Views.Navigation;

#nullable enable

public partial class TabView : ViewModelContentView<TabViewModel>
{
    public static readonly BindableProperty TabsProperty = BindableProperty.Create(
        nameof(Tabs),
        typeof(IEnumerable<Tab>),
        typeof(TabView),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: TabsChanged
    );

    public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create(
        nameof(SelectedTab),
        typeof(Tab),
        typeof(TabView),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: TabChanged
    );

    internal TabViewModel PublicVm => ViewModel;

    public IEnumerable<Tab>? Tabs
    {
        get => (IEnumerable<Tab>)GetValue(TabsProperty);
        set => SetValue(TabsProperty, value);
    }

    public Tab? SelectedTab
    {
        get => (Tab?)GetValue(SelectedTabProperty);
        set => SetValue(SelectedTabProperty, value);
    }

    /// <summary>
    /// The ContentView that TabView will use to display views referenced by Tabs.
    /// </summary>
    public ContentView? PairedDisplayView
    {
        get => ViewModel.PairedDisplayView;
        set => ViewModel.PairedDisplayView = value;
    }

    public TabView()
        : base(ServiceProvider.GetService<TabViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    static void TabsChanged(object bound, object old, object @new)
    {
        var tabView = (TabView)bound;
        var tabs = (IEnumerable<Tab>)@new;

        tabView.ViewModel.Tabs = tabs;
    }

    static void TabChanged(object bound, object old, object @new)
    {
        var tabView = (TabView)bound;
        var tab = (Tab)@new;

        tabView.ViewModel.SelectedTab = tab;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            foreach (var tab in Tabs ?? [])
                tab.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }
}
