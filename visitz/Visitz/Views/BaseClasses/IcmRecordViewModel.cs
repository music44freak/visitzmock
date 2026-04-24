using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Storage;
using Visitz.Views.Snackbar;
using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.BaseClasses;

#nullable enable

public partial class IcmRecordViewModel : VisitzViewModel, IIcmRecordInfo, IBusinessObjectHolder
{
    protected Realm? DataRealm { get; private set; }

    public string RowId { get; set; } = string.Empty;

    public EntityType EntityType { get; set; }

    [ObservableProperty]
    public IBusinessObject businessObject = new CaseRecord();

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        // TODO: get rid of this once we properly handle async exceptions in the lifecycle
        if (string.IsNullOrWhiteSpace(RowId) || EntityType == EntityType.Unknown)
        {
            string id = RowId.Length == 0 ? "empty" : RowId;
            string error = $"Row ID '{id}' / entity type '{EntityType}' provided—can't load {GetType().Name}";
            Logger.LogError(error);
#if DEBUG
            SnackbarHandler.ShowText(error);
#endif
            return;
        }

        DataRealm =
            await VisitzRealms.GetIcmDataRealmAsync()
            ?? throw new InvalidOperationException("Couldn't open IcmData Realm");

        BusinessObject =
            IBusinessObjectExtensions.GetByIdType(DataRealm, RowId, EntityType)
            ?? throw new InvalidOperationException(
                $"Unable to retrieve BusinessObject (id '{RowId}', type '{EntityType}')"
            );
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            DataRealm?.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }
}
