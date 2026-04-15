using Visitz.Services.Base;
using Visitz.Services.Messages;
using VisitzApi;
using VisitzApi.Models.People;
using VisitzApi.Requests;
using VisitzModel.Interfaces;
using VisitzModel.Models.People;
using VisitzModel.Storage;

#nullable enable

namespace Visitz.Services.People;

internal class SubmitSupportNetworkService(Vpi vpi, LastUpdatedPrefs prefs) : VisitzApiService(vpi, prefs)
{
    SupportNetworkItem SupportNetworkItem => (SupportNetworkItem)Payload;

    public static string MakeId(string supportNetworkId)
    {
        return $"{nameof(SubmitSupportNetworkService)}|{supportNetworkId}";
    }

    public static string MakeId(SupportNetworkItem supportNetwork)
    {
        return MakeId(supportNetwork.ParentId);
    }

    public static StartServiceMessage MakeStartMessage(SupportNetworkItem supportNetwork)
    {
        return new()
        {
            Payload = supportNetwork,
            ServiceId = MakeId(supportNetwork),
            ServiceType = typeof(SubmitSupportNetworkService),
        };
    }

    public override string GetId()
    {
        return MakeId(SupportNetworkItem);
    }

    protected override async Task RunApiServiceAsync()
    {
        await Vpi.SubmitSupportNetworkItemAsync(
            (ApiRecordType)SupportNetworkItem.ParentType,
            SupportNetworkItem.ParentId,
            ((IApiJson<SubmitSupportNetworkJson>)SupportNetworkItem).ToApiJson("yyyy-MM-dd")
        );

        ResultCode = Result.Successful;
    }
}
