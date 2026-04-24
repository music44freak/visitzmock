using Oidc.Network;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Views.TagViews;

namespace Visitz.Views;

public partial class InternetInfoView : ContentView
{
    public static readonly BindableProperty ShouldShowViewProperty = BindableProperty.Create(
        nameof(ShouldShowView),
        typeof(bool),
        typeof(InternetInfoView)
    );

    public static readonly BindableProperty MessageProperty = BindableProperty.Create(
        nameof(Message),
        typeof(string),
        typeof(InternetInfoView)
    );

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
        nameof(ImageSource),
        typeof(ImageSource),
        typeof(InternetInfoView)
    );

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(TagView)
    );

    public bool ShouldShowView
    {
        get => (bool)GetValue(ShouldShowViewProperty);
        set => SetValue(ShouldShowViewProperty, value);
    }

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public InternetInfoView()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        ApplyConnectivityStyles();
    }

    protected override void OnParentChanging(ParentChangingEventArgs args)
    {
        base.OnParentChanging(args);

        if (args.AttachingToParent())
            Connectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        else if (args.DetachingFromParent())
            Connectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
    }

    private void Current_ConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        ApplyConnectivityStyles();
    }

    private void ApplyConnectivityStyles()
    {
        ShouldShowView = !NetworkHelper.InternetAvailable;

        Message = LocalizedStrings.NoInternet;
        Color = Colors.Red;
        ImageSource = MaterialIcons.Signal_disconnected.GetUnfilledMaterialIcon(Color);
    }
}
