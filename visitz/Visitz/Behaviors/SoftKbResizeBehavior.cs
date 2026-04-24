namespace Visitz.Behaviors;

public partial class SoftKbResizeBehavior : Behavior<View>
{
    View View { get; set; }

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);

        View = bindable;
        View.Loaded += View_Loaded;
        View.Unloaded += View_Unloaded;
    }

    private void View_Loaded(object? sender, EventArgs e)
    {
        Attach();
    }

    private void View_Unloaded(object? sender, EventArgs e)
    {
        View.Loaded -= View_Loaded;
        View.Unloaded -= View_Unloaded;

        Detach();
    }

    partial void Attach();

    partial void Detach();
}
