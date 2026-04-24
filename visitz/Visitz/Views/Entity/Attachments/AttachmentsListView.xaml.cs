using Visitz.Views.BaseClasses;

namespace Visitz.Views.Entity.Attachments;

public partial class AttachmentsListView : IcmRecordContentView<AttachmentsListViewModel>
{
    public AttachmentsListView()
        : base(ServiceProvider.GetService<AttachmentsListViewModel>())
    {
        InitializeComponent();
        BindingContext = ViewModel;
    }
}
