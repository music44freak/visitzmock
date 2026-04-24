using Visitz.Extensions;
using Visitz.Resources.Localization;

namespace Visitz.Views.Snackbar;

internal class SnackbarHandler
{
    public static readonly int DefaultTextOnlyDurationSeconds = 3;
    public static readonly int DefaultActionDurationSeconds = 5;
    public static readonly int DefaultErrorDurationSeconds = 7;

    public static void ShowText(string snackPrompt, TimeSpan? duration = null)
    {
        if (Navigator.CurrentOpenPage is ISnackbarPresenter presenter)
            presenter.SetSnackbar(
                new VisitzSnackbar()
                {
                    Message = snackPrompt,
                    ActionText = null,
                    Action = null,
                    Duration = duration ?? TimeSpan.FromSeconds(DefaultTextOnlyDurationSeconds),
                }
            );
    }

    public static void ShowTextWithDetails(
        string snackPrompt,
        string messageTitle,
        string fullMessage,
        TimeSpan? duration = null
    )
    {
        if (Navigator.CurrentOpenPage is ISnackbarPresenter presenter)
            presenter.SetSnackbar(
                new VisitzSnackbar()
                {
                    Message = snackPrompt,
                    ActionText = LocalizedStrings.MoreInfo,
                    Action = async () => await ShowDialogMessage(messageTitle, fullMessage),
                    Duration = duration ?? TimeSpan.FromSeconds(DefaultActionDurationSeconds),
                }
            );
    }

    public static void ShowError(Exception ex, TimeSpan? duration = null)
    {
        if (Navigator.CurrentOpenPage is ISnackbarPresenter presenter)
            presenter.SetSnackbar(
                new VisitzSnackbar()
                {
                    Message = ex.Message,
                    ActionText = LocalizedStrings.MoreInfo,
                    Action = async () => await Navigator.CurrentOpenPage.DisplayErrorAlert(ex),
                    Duration = duration ?? TimeSpan.FromSeconds(DefaultErrorDurationSeconds),
                }
            );
    }

    static async Task ShowDialogMessage(string title, string message)
    {
        await Navigator.CurrentOpenPage.DisplayAlertAsync(title, message, LocalizedStrings.Ok);
    }
}
