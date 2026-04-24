namespace Visitz.Views.BaseClasses;

#nullable enable

public abstract class ViewModelContentView<TViewModel>(TViewModel viewModel, string title = "") : BaseContentView(title)
    where TViewModel : VisitzViewModel
{
    public TViewModel ViewModel { get; private set; } = viewModel;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        ArgumentNullException.ThrowIfNull(ViewModel);

        await ViewModel.StartInitAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            ViewModel.Dispose();

        base.Dispose(disposing);
    }
}
