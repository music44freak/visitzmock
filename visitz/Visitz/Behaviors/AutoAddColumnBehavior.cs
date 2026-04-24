namespace Visitz.Behaviors;

public partial class AutoAddColumnBehavior : Behavior<Grid>
{
    protected override void OnAttachedTo(Grid grid)
    {
        base.OnAttachedTo(grid);

        grid.ChildAdded += Bindable_ChildAdded;
    }

    protected override void OnDetachingFrom(Grid grid)
    {
        grid.ChildAdded -= Bindable_ChildAdded;

        base.OnDetachingFrom(grid);
    }

    private void Bindable_ChildAdded(object? sender, ElementEventArgs e)
    {
        Grid grid = (Grid)sender;

        grid.AddColumnDefinition(new ColumnDefinition());
        grid.SetColumn((IView)e.Element, grid.Children.Count - 1);
    }
}
