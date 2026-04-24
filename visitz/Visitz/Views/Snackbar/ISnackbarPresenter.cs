namespace Visitz.Views.Snackbar;

internal interface ISnackbarPresenter
{
    void SetSnackbar(VisitzSnackbar snackbar);

    void Snackbar_ShouldClose(object? sender, EventArgs e);
}
