using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzModel.Models.People;
using VisitzModel.Storage;

namespace Visitz.Services.People;

#nullable enable
internal class GetContactLanguagesByRangeService(Vpi vpi, LastUpdatedPrefs prefs, ServiceHandler serviceHandler)
    : VisitzApiRangeService<IcmContact>(vpi, prefs, serviceHandler)
{
    public static string MakeId()
    {
        return nameof(GetContactLanguagesByRangeService);
    }

    public static StartServiceMessage MakeStartMessage(IEnumerable<IcmContact> items)
    {
        return new()
        {
            ServiceId = MakeId(),
            ServiceType = typeof(GetContactLanguagesByRangeService),
            Payload = items,
        };
    }

    public override string GetId()
    {
        return MakeId();
    }

    protected override async Task RunInParallelAsync(ServiceHandler serviceHandler, IcmContact item)
    {
        await serviceHandler.TryRunServiceAsync(GetContactLanguagesService.MakeStartMessage(item));
    }

    protected override Exception MakePartialException(List<ApiRangeItemException<IcmContact>> exceptions)
    {
        var outString = exceptions
            .Select(ex =>
            {
                return $"• {ex.Item.ParentType} {ex.Item.ParentId} {ex.Item.Id} -> {ex.Message}";
            })
            .Aggregate((accum, item) => accum + Environment.NewLine + item);

        return new Exception(outString);
    }
}
