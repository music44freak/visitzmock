namespace Visitz.Views.FormControls;

using Visitz.Animations;
using Visitz.Animations.Haptic;
using Visitz.Resources.Localization;
using VisitzModel.Extensions;

public partial class FormEntry : ContentView
{
    public static readonly BindableProperty FieldNameProperty = BindableProperty.Create(
        nameof(FieldName),
        typeof(string),
        typeof(FormEntry)
    );

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(FormEntry),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (boundObj, oldVal, newVal) => (boundObj as FormEntry).UpdateCharacterCount()
    );

    public static readonly BindableProperty LeadingSupportingTextProperty = BindableProperty.Create(
        nameof(LeadingSupportingText),
        typeof(string),
        typeof(FormEntry)
    );

    public static readonly BindableProperty TrailingSupportingTextProperty = BindableProperty.Create(
        nameof(TrailingSupportingText),
        typeof(string),
        typeof(FormEntry)
    );

    public static readonly BindableProperty FieldNameIsVisibleProperty = BindableProperty.Create(
        nameof(FieldNameIsVisible),
        typeof(bool),
        typeof(FormEntry),
        defaultValue: true,
        propertyChanged: (boundObj, oldVal, newVal) =>
        {
            var formEntry = (FormEntry)boundObj;
            var isVisible = (bool)newVal;

            formEntry.FieldNameRow.Height = isVisible ? GridLength.Star : 0.0;
        }
    );

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(FormEntry)
    );

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength),
        typeof(int),
        typeof(FormEntry),
        defaultValue: int.MaxValue,
        propertyChanged: (boundObj, oldVal, newVal) =>
        {
            var formEntry = (FormEntry)boundObj;
            int newLength = (int)newVal;

            formEntry.UpdateBottomRowVisibility(newLength);
            formEntry.UpdateCharacterCount();
        }
    );

    public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(FormEntry)
    );

    public string FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string LeadingSupportingText
    {
        get => (string)GetValue(LeadingSupportingTextProperty);
        set => SetValue(LeadingSupportingTextProperty, value);
    }

    public string TrailingSupportingText
    {
        get => (string)GetValue(TrailingSupportingTextProperty);
        set => SetValue(TrailingSupportingTextProperty, value);
    }

    public bool FieldNameIsVisible
    {
        get => (bool)GetValue(FieldNameIsVisibleProperty);
        set => SetValue(FieldNameIsVisibleProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public FormEntry()
    {
        InitializeComponent();
    }

    private void UpdateBottomRowVisibility(int maxLength)
    {
        SubRow.Height = maxLength == int.MaxValue ? 0.0d : GridLength.Auto;
    }

    private void UpdateCharacterCount()
    {
        TrailingSupportingText = $"{Text?.Length ?? 0}/{MaxLength}";
    }

    private void Editor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (ContainEmojis(e))
        {
            CancelTextChangedEvent(sender, e);
            var ErrorMessage = LocalizedStrings.InvalidEntry;
            _ = ShowEditorError(ErrorMessage);
            return;
        }
    }

    private static bool ContainEmojis(TextChangedEventArgs e)
    {
        return e.NewTextValue?.ContainsUnicodeSurrogatesAndOtherSymbols() ?? false;
    }

    private void CancelTextChangedEvent(object? sender, TextChangedEventArgs e)
    {
        var textBox = sender as Editor;
        textBox.Text = e.OldTextValue;
    }

    public async Task ShowEditorError(string text)
    {
        await Task.WhenAll(ShowErrorText(text), AnimateEditorError());
    }

    private async Task ShowErrorText(string text)
    {
        if (EditorError.IsVisible)
            return;

        var showAnimation = new VisibilityAnimation(true, 1000);
        LeadingSupportingText = text;
        await Task.WhenAll(showAnimation.Animate(EditorError), showAnimation.Animate(LeadingSupportingLabel));

        await Task.Delay(2000);

        var hideAnimation = new VisibilityAnimation(false, 1000);
        await Task.WhenAll(hideAnimation.Animate(EditorError), hideAnimation.Animate(LeadingSupportingLabel));
    }

    private async Task AnimateEditorError()
    {
        var vibrateErrorAnim = new ErrorVibrateAnimation();
        await vibrateErrorAnim.Animate(Editor);
    }
}
