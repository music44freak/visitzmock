using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Realms;
using Visitz.Extensions;
using Visitz.FontIcons;
using Visitz.Resources.Localization;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.SafetyAssess;

namespace Visitz.Views.Entity.SafetyAssess;

#nullable enable

public partial class SafetyAssessmentListViewModel : IcmRecordViewModel
{
    [ObservableProperty]
    public string editViewButtonText = "";

    [ObservableProperty]
    public string editViewButtonGlyph = "";

    [ObservableProperty]
    public ObservableCollection<SafetyAssessment> assessments = [];

    readonly ObservableRealmQueryMap realmQueryMap = new();

    [ObservableProperty]
    public bool isEmpty;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;

        var draftRealm = await VisitzRealms.GetSafetyAssessmentDraftRealmAsync();
        var draftQuery = AssessmentDraft.GetAllByFileNumber(draftRealm, BusinessObject.FileNumber);
        realmQueryMap.Subscribe(draftRealm, draftQuery);

        var dataRealm = await VisitzRealms.GetIcmDataRealmAsync();
        var dataQuery = SafetyAssessment
            .GetAllByFileNumber(dataRealm, BusinessObject.FileNumber)
            .OrderByDescending(sa => sa.CreatedDate);
        realmQueryMap.Subscribe(dataRealm, dataQuery);
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmQueryMap?.Dispose();

            disposed = true;
        }

        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(SafetyAssessment))
            UpdateSafetyAssessmentsList(e.Items, e.Changes);
        if (e.Type == typeof(AssessmentDraft))
            UpdateEditViewButtonText(e.Items.Any());
    }

    void UpdateSafetyAssessmentsList(IRealmCollection<IRealmObject> items, ChangeSet? changes)
    {
        if (changes == null)
        {
            foreach (var item in items)
                Assessments.Add((SafetyAssessment)item);
        }
        else
        {
            foreach (var i in changes.DeletedIndices.Reverse())
                Assessments.RemoveAt(i);

            foreach (var i in changes.InsertedIndices)
                Assessments.Add((SafetyAssessment)items.ElementAt(i));
        }

        IsEmpty = !Assessments.Any();
    }

    void UpdateEditViewButtonText(bool draftAvailable)
    {
        EditViewButtonText = draftAvailable ? LocalizedStrings.ContinueDraft : LocalizedStrings.AddNew;
        EditViewButtonGlyph = draftAvailable ? MaterialIcons.Assignment : MaterialIcons.Assignment_add;
    }

    [RelayCommand]
    public async Task OpenSafetyAssessmentView(SafetyAssessment? assessment = null)
    {
        var view = ServiceProvider.GetService<SafetyAssessmentEditView>();

        view.BusinessObject = BusinessObject;
        view.ViewModel.RowId = RowId;
        view.ViewModel.EntityType = EntityType;

        if (assessment != null)
            view.ViewAssessment(assessment);

        await Navigator.Navigation.PushModalAsync(view);
    }
}
