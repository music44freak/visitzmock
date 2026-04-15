using Realms;
using Realms.Schema;
using VisitzModel.Models;
using VisitzModel.Models.Attachments;
using VisitzModel.Models.CallDetails;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.InPersonVisits;
using VisitzModel.Models.Notes;
using VisitzModel.Models.People;
using VisitzModel.Models.SafetyAssess;
using VisitzModel.Storage.Migrations;

namespace VisitzModel.Storage;

public class IcmData(byte[] encryptionKey) : VisitzRealmBase(Name, CurrentVersion, encryptionKey)
{
    public static readonly string Name = "icmDataCopies.realm";
    public static readonly ulong CurrentVersion = Version2_8_0;

    protected override RealmSchema MakeRealmSchema()
    {
        return new[]
        {
            typeof(CaseloadItem),
            typeof(FamilyMember),
            typeof(CaseRecord),
            typeof(IncidentRecord),
            typeof(MemoRecord),
            typeof(ServiceRequestRecord),
            typeof(NoteItem),
            typeof(PersonVisit),
            typeof(IcmContact),
            typeof(SupportNetworkItem),
            typeof(Attachment),
            typeof(AttachmentDraft),
            typeof(BoLocalState),
            typeof(SafetyAssessment),
            typeof(FactorInfluence),
            typeof(ProtectiveCapacity),
            typeof(SafetyDecisions),
            typeof(SafetyFactors),
            typeof(SafetyInterventions),
            typeof(IncidentConcerns),
            typeof(AdditionalInformation),
            typeof(CallInformation),
            typeof(ContactMedicalBehavioral),
            typeof(ContactLanguage),
        };
    }

    protected override void MigrateRealm(Migration migration, ulong oldSchemaVersion)
    {
        IcmDataMigrations.MigrateRealm(migration, oldSchemaVersion);
    }
}
