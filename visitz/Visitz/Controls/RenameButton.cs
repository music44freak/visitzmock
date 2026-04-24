using Visitz.FontIcons;
using Visitz.Resources.Localization;

namespace Visitz.Controls;

internal class RenameButton : FontIconButton
{
    public static readonly BindableProperty RenamingProperty = BindableProperty.Create(
        nameof(Renaming),
        typeof(string),
        typeof(RenameButton),
        defaultBindingMode: BindingMode.TwoWay
    );

    public string Renaming
    {
        get => (string)GetValue(RenamingProperty);
        set => SetValue(RenamingProperty, value);
    }

    public RenameButton()
    {
        FontFamily = MaterialIcons.RoundedUnfilled.FontFamily;
        Text = MaterialIcons.Edit;
        TextColor = Colors.White;

        Clicked += RenameButton_Clicked;
    }

    private async void RenameButton_Clicked(object? sender, EventArgs e)
    {
        string newName = await Navigator.CurrentOpenPage.DisplayPromptAsync(
            LocalizedStrings.Rename,
            null,
            placeholder: Renaming,
            initialValue: Renaming
        );

        if (newName != Renaming && newName?.Trim()?.Length > 0)
            Renaming = newName;
    }
}
