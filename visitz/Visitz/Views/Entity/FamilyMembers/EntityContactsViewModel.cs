using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Realms;
using Visitz.Views.BaseClasses;
using VisitzModel.Models;
using VisitzModel.Models.People;

namespace Visitz.Views.Entity.FamilyMembers;

#nullable enable

public partial class EntityContactsViewModel : IcmRecordViewModel
{
    readonly ObservableRealmQueryMap realmQueryMap = new();

    [ObservableProperty]
    public ObservableCollection<ContactItemViewModel> contactViewModels = [];

    [ObservableProperty]
    public bool isEmpty;

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        if (DataRealm == null)
            return;

        realmQueryMap.ItemsChanged += RealmQueryMap_ItemsChanged;
        realmQueryMap.Subscribe(DataRealm, IcmContact.GetByParentObject(DataRealm, BusinessObject));
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            realmQueryMap.ItemsChanged -= RealmQueryMap_ItemsChanged;
            realmQueryMap.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }

    private void RealmQueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        var comparer = new IcmContactRelationshipComparer();

        if (e.Changes == null)
        {
            var ordered = e.Items.Cast<IcmContact>().ToList().Order(comparer);

            foreach (var contact in ordered)
                ContactViewModels.Add(new ContactItemViewModel(contact));
        }
        else
        {
            // We can't rely on the indices provided by realm because we're
            // modifying the collection order outside the original query. So
            // we need to do another full query to see differences.

            List<IcmContact> contactsCopy = ContactViewModels.Select(vm => vm.Contact).ToList();
            var savedContacts = IcmContact.GetByParentObject(DataRealm, BusinessObject).ToList();

            var removed = contactsCopy.Except(savedContacts);
            var added = savedContacts.Except(contactsCopy);
            var removeVms = ContactViewModels.Where(vm => removed.Contains(vm.Contact)).ToList();

            foreach (var vm in removeVms)
            {
                contactsCopy.Remove(vm.Contact);
                ContactViewModels.Remove(vm);
            }

            foreach (IcmContact contact in added)
            {
                int index = contactsCopy.BinarySearch(contact, comparer);
                if (index < 0)
                    index = ~index;

                contactsCopy.Insert(index, contact);
                ContactViewModels.Insert(index, new ContactItemViewModel(contact));
            }
        }

        IsEmpty = !ContactViewModels.Any();
    }
}
