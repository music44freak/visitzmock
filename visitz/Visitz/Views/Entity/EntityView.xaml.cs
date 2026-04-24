using System.ComponentModel;
using System.Reflection;
using Syncfusion.Maui.Toolkit.Popup;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity;

#nullable enable

public partial class EntityView : IcmRecordContentView<EntityViewModel>
{
    Grid? TabsGrid => TabView.Content as Grid;

    RowDefinition? TabsRow =>
        TabsGrid is not null && TabsGrid.RowDefinitions.Count > 0 ? TabsGrid?.RowDefinitions[0] : null;

    Thickness? TabBarPadding
    {
        get
        {
            if (TabsGrid?[0] is VisualElement element)
            {
                Type type = element.GetType();

                if (
                    type.GetProperty("TabHeaderPadding", typeof(Thickness)) is PropertyInfo info
                    && info.GetValue(element) is Thickness thickness
                )
                {
                    return thickness;
                }
            }

            return null;
        }
    }

    public EntityView()
        : base(ServiceProvider.GetService<EntityViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;

        ViewModel.TabListButtonWidth = TabBarPadding?.Left ?? -1;

        if (TabsRow is RowDefinition row)
        {
            row.SizeChanged += EntityView_SizeChanged;
            ViewModel.TabBarHeight = row.Height.Value;
        }

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            TabsRow?.SizeChanged -= EntityView_SizeChanged;

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.SelectedTab) && TabPopup.IsOpen)
            TabPopup.Dismiss();
    }

    void EntityView_SizeChanged(object? sender, EventArgs e)
    {
        if (sender is RowDefinition row)
            ViewModel.TabBarHeight = row.Height.Value;
    }

    async void TabListButton_Clicked(object? sender, EventArgs e)
    {
        if (sender != null)
            TabPopup.ShowRelativeToView((View)sender, PopupRelativePosition.AlignBottom);
    }
}
