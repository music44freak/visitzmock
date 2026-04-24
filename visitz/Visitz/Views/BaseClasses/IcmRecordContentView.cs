using VisitzModel.Interfaces;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.EntityTypes;

namespace Visitz.Views.BaseClasses;

#nullable enable

public partial class IcmRecordContentView<TViewModel>(TViewModel viewModel, string title = "")
    : ViewModelContentView<TViewModel>(viewModel, title),
        IIcmRecordInfo,
        IBusinessObjectHolder
    where TViewModel : IcmRecordViewModel
{
    public string RowId
    {
        get => ViewModel.RowId;
        set => ViewModel.RowId = value;
    }

    public EntityType EntityType
    {
        get => ViewModel.EntityType;
        set => ViewModel.EntityType = value;
    }

    public IBusinessObject BusinessObject
    {
        get => ViewModel.BusinessObject;
        set => ViewModel.BusinessObject = value;
    }
}
