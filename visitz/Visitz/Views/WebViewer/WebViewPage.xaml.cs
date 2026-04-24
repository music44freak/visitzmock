using Visitz.Views.BaseClasses;

namespace Visitz.Views.WebViewer;

public partial class WebViewPage : VisitzPage
{
    new WebViewModel ViewModel => base.ViewModel as WebViewModel;

    public Uri AuthUri
    {
        get => ViewModel.AuthUri;
        set => ViewModel.AuthUri = value;
    }

    public CancellationTokenSource? CancelTokenSource { get; set; }

    public WebViewPage()
        : base(ServiceProvider.GetService<WebViewModel>())
    {
        InitializeComponent();
        Setup();
    }

    partial void Setup();
}
