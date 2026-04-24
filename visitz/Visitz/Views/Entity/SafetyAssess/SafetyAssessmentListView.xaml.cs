using Visitz.Resources.Localization;
using Visitz.Views.BaseClasses;
using VisitzModel.Models.Navigation;

namespace Visitz.Views.Entity.SafetyAssess;

#nullable enable

public partial class SafetyAssessmentListView
    : IcmRecordContentView<SafetyAssessmentListViewModel>,
        IRequestedEntitySection
{
    public EntitySection RequestedSection { get; set; }

    public SafetyAssessmentListView()
        : base(ServiceProvider.GetService<SafetyAssessmentListViewModel>(), LocalizedStrings.SafetyAssessment)
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (RequestedSection == EntitySection.SafetyAssessmentEntry)
            await ViewModel.OpenSafetyAssessmentView();
    }
}
