using CommunityToolkit.Mvvm.Messaging.Messages;
using VisitzModel.Models.Caseload;
using VisitzModel.Models.Drafts;
using VisitzModel.Models.Navigation;

namespace VisitzModel.Messaging;

#nullable enable

public class BusinessObjectSelectedMessage(
    IBusinessObject value,
    EntitySection? section = null,
    IDraftItem? draftItem = null
) : ValueChangedMessage<IBusinessObject>(value)
{
    public EntitySection? Section { get; set; } = section;

    public IDraftItem? DraftItem { get; set; } = draftItem;
}
