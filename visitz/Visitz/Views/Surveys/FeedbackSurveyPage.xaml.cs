using Visitz.Settings;
using VisitzModel;
using VisitzModel.Storage;
#if IOS
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
#endif

namespace Visitz.Views.Surveys;

public partial class FeedbackSurveyPage : ContentPage
{
    public static async Task TryOpen()
    {
        var tracker = new SurveyFeedbackTracker(Preferences.Default);

        ConsoleTrace.TraceMethod(
            typeof(FeedbackSurveyPage),
            $"\n!SurveyPrompted: '{!tracker.SurveyPrompted}'"
                + $"\nUnlockedAppEnough: '{tracker.UnlockedAppEnough}'"
                + $"\nHavePublishedAnything: '{tracker.PublishedAnything}'"
        );

        if (!tracker.SurveyPrompted && tracker.UnlockedAppEnough && tracker.PublishedAnything)
        {
            await Navigator.Navigation.PushModalAsync(new FeedbackSurveyPage());
            tracker.SetHavePromptedSurvey();
        }
    }

    public FeedbackSurveyPage()
    {
#if IOS
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
#endif

        InitializeComponent();
    }

    private async void StartSurvey_Clicked(object? sender, EventArgs e)
    {
        var feedbackUri = new Uri(new AppSettings().ContactInfo.FeedbackSurveyUrl);

        await Navigator.Navigation.PopModalAsync();

        await Browser.Default.OpenAsync(
            feedbackUri,
            new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Hide,
                Flags = BrowserLaunchFlags.PresentAsPageSheet,
            }
        );
    }
}
