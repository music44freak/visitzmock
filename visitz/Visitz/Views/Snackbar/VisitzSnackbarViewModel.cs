using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Visitz.Views.BaseClasses;

namespace Visitz.Views.Snackbar;

public partial class VisitzSnackbarViewModel : VisitzViewModel
{
    [ObservableProperty]
    public string message;

    [ObservableProperty]
    public string actionText;

    [ObservableProperty]
    public Action action;

    [ObservableProperty]
    public bool actionVisible = false;

    [RelayCommand]
    public void ActionButtonSelected()
    {
        Action?.Invoke();
    }

    partial void OnActionChanged(Action value)
    {
        UpdateActionVisible();
    }

    partial void OnActionTextChanged(string value)
    {
        UpdateActionVisible();
    }

    void UpdateActionVisible()
    {
        ActionVisible = Action != null && !string.IsNullOrWhiteSpace(ActionText);
    }
}
