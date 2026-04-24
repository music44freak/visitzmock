using Realms;
using VisitzApi.Models.SafetyAssess;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;

namespace VisitzModel.Models.SafetyAssess;

#nullable enable

public partial class FactorInfluence : IRealmObject, IApiJson<SubmitFactorInfluenceJson>
{
    public bool AgeUptoFive { get; set; }

    public bool MedicalMentalDisorder { get; set; }

    public bool NotReadilyAccessible { get; set; }

    public bool DiminishedMental { get; set; }

    public bool DiminishedPhysical { get; set; }

    public static FactorInfluence FromApiJson(GetSafetyAsessmentJson entity)
    {
        return new FactorInfluence()
        {
            AgeUptoFive = entity.FactorInfluence1.ParseWordTruthiness(),
            MedicalMentalDisorder = entity.FactorInfluence2.ParseWordTruthiness(),
            NotReadilyAccessible = entity.FactorInfluence3.ParseWordTruthiness(),
            DiminishedMental = entity.FactorInfluence4.ParseWordTruthiness(),
            DiminishedPhysical = entity.FactorInfluence5.ParseWordTruthiness(),
        };
    }

    public SubmitFactorInfluenceJson ToApiJson(string _ = "s")
    {
        return new SubmitFactorInfluenceJson()
        {
            AgeUptoFive = AgeUptoFive.AsTruthyChar(),
            MedicalMentalDisorder = MedicalMentalDisorder.AsTruthyChar(),
            NotReadilyAccessible = NotReadilyAccessible.AsTruthyChar(),
            DiminishedMental = DiminishedMental.AsTruthyChar(),
            DiminishedPhysical = DiminishedPhysical.AsTruthyChar(),
        };
    }
}
