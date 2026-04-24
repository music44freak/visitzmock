using CommunityToolkit.Mvvm.ComponentModel;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.Details;

#nullable enable

public partial class EntityDetailsViewModel : IcmRecordViewModel
{
    [ObservableProperty]
    public IcmContact? keyPlayer;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        KeyPlayer = BusinessObject.GetKeyPlayer();
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            disposed = true;
        }
        base.Dispose(disposing);
    }
}
