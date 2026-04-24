using Visitz.Views.BaseClasses;
using Visitz.Views.TagViews;
using Visitz.VisualStates;

namespace Visitz.Views.SegmentedButtons;

public partial class SegmentedButtonsView : BaseContentView
{
    public static readonly BindableProperty OptionsProperty = BindableProperty.Create(
        nameof(Options),
        typeof(IEnumerable<SegmentedOptions>),
        typeof(SegmentedButtonsView)
    );

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SegmentedButtonsView)
    );

    public static readonly BindableProperty ActivatedBackgroundColorProperty = BindableProperty.Create(
        nameof(ActivatedBackgroundColor),
        typeof(Color),
        typeof(SegmentedButtonsView)
    );

    public static readonly BindableProperty ActivatedTextColorProperty = BindableProperty.Create(
        nameof(ActivatedTextColor),
        typeof(Color),
        typeof(SegmentedButtonsView)
    );

    public static readonly BindableProperty ItemPaddingProperty = BindableProperty.Create(
        nameof(ItemPadding),
        typeof(Thickness),
        typeof(TagView),
        defaultValue: new Thickness(10.0)
    );

    public static readonly BindableProperty TapGestureCannotDeactivateItemProperty = BindableProperty.Create(
        nameof(TapGestureCannotDeactivateItem),
        typeof(bool),
        typeof(SegmentedButtonsView)
    );

    public static readonly BindableProperty ActivatedOptionProperty = BindableProperty.Create(
        nameof(ActivatedOption),
        typeof(SegmentedOptions),
        typeof(SegmentedButtonsView),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bound, oldVal, newVal) =>
        {
            var segmentedView = (SegmentedButtonsView)bound;
            var oldOption = (SegmentedOptions)oldVal;
            var newOption = (SegmentedOptions)newVal;

            if (oldOption != null)
                if (segmentedView.GetPairedTagView(oldOption) is ActivatableTagView oldTagView)
                    oldTagView.IsActive = false;

            if (segmentedView.GetPairedTagView(newOption) is ActivatableTagView tagView)
                tagView.IsActive = true;
        }
    );

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(SegmentedButtonsView)
    );

    public IEnumerable<SegmentedOptions> Options
    {
        get => (IEnumerable<SegmentedOptions>)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public Color ActivatedBackgroundColor
    {
        get => (Color)GetValue(ActivatedBackgroundColorProperty);
        set => SetValue(ActivatedBackgroundColorProperty, value);
    }

    public Color ActivatedTextColor
    {
        get => (Color)GetValue(ActivatedTextColorProperty);
        set => SetValue(ActivatedTextColorProperty, value);
    }

    public Thickness ItemPadding
    {
        get => (Thickness)GetValue(ItemPaddingProperty);
        set => SetValue(ItemPaddingProperty, value);
    }

    public bool TapGestureCannotDeactivateItem
    {
        get => (bool)GetValue(TapGestureCannotDeactivateItemProperty);
        set => SetValue(TapGestureCannotDeactivateItemProperty, value);
    }

    public SegmentedOptions ActivatedOption
    {
        get => (SegmentedOptions)GetValue(ActivatedOptionProperty);
        set => SetValue(ActivatedOptionProperty, value);
    }

    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public event EventHandler<ItemActivatedEventArgs> ItemActivated;

    public event EventHandler<ItemDeactivatedEventArgs> ItemDeactivated;

    private ActivatableTagView lastTagActivated;

    public SegmentedButtonsView()
    {
        InitializeComponent();
    }

    private SegmentedOptions GetPairedOptions(ActivatableTagView tagView)
    {
        int tagIndex = Items.Children.IndexOf(tagView);
        return Options.ElementAt(tagIndex);
    }

    private ActivatableTagView GetPairedTagView(SegmentedOptions option)
    {
        if (option == null)
            return null;

        int optionIndex = -1;

        for (int i = 0; i < Options.Count(); i++)
        {
            if (Options.ElementAt(i) == option)
            {
                optionIndex = i;
                break;
            }
        }

        return optionIndex == -1 ? null : (ActivatableTagView)Items.Children[optionIndex];
    }

    private void ActivatableTagView_ActiveStateChanged(object? sender, IActiveState.ActiveChangedEventArgs e)
    {
        if (e.IsActive)
            HandleSingleActivation((ActivatableTagView)sender);
        else
            HandleSingleDeactivation((ActivatableTagView)sender);
    }

    private void HandleSingleActivation(ActivatableTagView tagView)
    {
        var activatedOptions = GetPairedOptions(tagView);

        if (lastTagActivated?.IsActive ?? false)
            if (GetPairedOptions(lastTagActivated) != activatedOptions)
                lastTagActivated.SetIsActiveSilently(false);

        ActivatedOption = activatedOptions;
        ItemActivated?.Invoke(this, new ItemActivatedEventArgs(activatedOptions));
        lastTagActivated = tagView;
    }

    private void HandleSingleDeactivation(ActivatableTagView tagView)
    {
        int tagIndex = Items.Children.IndexOf(tagView);
        var pairedOptions = Options.ElementAt(tagIndex);

        var args = new ItemDeactivatedEventArgs(Options.ElementAt(tagIndex));

        if (ActivatedOption == pairedOptions)
            ActivatedOption = null;

        ItemDeactivated?.Invoke(this, args);
    }

    public bool ActivatableTagView_ShouldCancelTapEvent(ActivatableTagView sender, TappedEventArgs _)
    {
        // Assuming we only support single-activation: make sure the user cannot deactivate the activated item.
        // This way we can force that one item must be active.
        return TapGestureCannotDeactivateItem && sender.IsActive;
    }
}
