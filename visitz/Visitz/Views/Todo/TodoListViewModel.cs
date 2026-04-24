using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Realms;
using Visitz.Storage;
using Visitz.Views.BaseClasses;
using VisitzModel.Extensions;
using VisitzModel.Interfaces;
using VisitzModel.Models;
using VisitzModel.Models.InPersonVisits;

namespace Visitz.Views.Todo;

#nullable enable

public partial class TodoListViewModel : VisitzViewModel
{
    [ObservableProperty]
    public bool showEmptyView = true;

    Realm? DataRealm { get; set; }

    readonly ObservableRealmQueryMap _queryMap = new();

    public ObservableCollection<ITodoItem> TodoItems { get; } = [];

    readonly ObservableCollection<TodoVisitItemViewModel> _personVisits = [];

    protected override async Task InitAsync()
    {
        await base.InitAsync();

        TodoItems.CollectionChanged += TodoItems_CollectionChanged;

        await SetupQueries();
    }

    async Task SetupQueries()
    {
        _queryMap.ItemsChanged += QueryMap_ItemsChanged;

        DataRealm = await VisitzRealms.GetIcmDataRealmAsync();

        _personVisits.CollectionChanged += SupportingList_CollectionChanged;
        _queryMap.Subscribe(DataRealm, DataRealm.All<PersonVisit>());
    }

    private void QueryMap_ItemsChanged(
        object? sender,
        (Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes) e
    )
    {
        if (e.Type == typeof(PersonVisit) && DataRealm != null)
        {
            var dbVisits = PersonVisit.GetUpcomingVisits(DataRealm).ToList();
            var loadedVisits = _personVisits;

            List<PersonVisit> addVisits = dbVisits
                .Except(loadedVisits.Select(vm => vm.Visit))
                .OfType<PersonVisit>()
                .ToList();
            var remVisits = loadedVisits.ExceptBy(dbVisits, vm => vm.Visit).ToList();

            UpdateSupportingList(
                addVisits,
                remVisits,
                _personVisits,
                (item) => new TodoVisitItemViewModel((PersonVisit)item)
            );
        }
    }

    /// <summary>
    /// Processes incoming results from a realm query to insert into a supporting list for the main
    /// ObservableCollection 'TodoItems'.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <param name="items"></param>
    /// <param name="changes"></param>
    /// <param name="draftsList"></param>
    /// <param name="mapper"></param>
    static void UpdateSupportingList<VM>(
        IRealmCollection<IRealmObject> items,
        ChangeSet changes,
        ObservableCollection<VM> draftsList,
        Func<IRealmObject, VM> mapper
    )
        where VM : VisitzViewModel
    {
        if (changes == null)
        {
            foreach (var realmObj in items)
                draftsList.Add(mapper(realmObj));
        }
        else
        {
            foreach (int deleteIndex in changes.DeletedIndices.Reverse())
                draftsList.RemoveAt(deleteIndex);

            foreach (int insertIndex in changes.InsertedIndices)
                draftsList.Add(mapper(items.ElementAt(insertIndex)));
        }
    }

    /// <summary>
    /// Processes explicit add/remove items into a supporting list for the main ObservableCollection 'TodoItems'.
    /// </summary>
    /// <typeparam name="VM"></typeparam>
    /// <param name="addItems"></param>
    /// <param name="removeItems"></param>
    /// <param name="draftsList"></param>
    /// <param name="mapper"></param>
    static void UpdateSupportingList<VM>(
        IEnumerable<IRealmObject> addItems,
        IEnumerable<VM> removeItems,
        ObservableCollection<VM> draftsList,
        Func<IRealmObject, VM> mapper
    )
    {
        foreach (var remove in removeItems)
            draftsList.Remove(remove);

        foreach (var add in addItems)
            draftsList.Add(mapper(add));
    }

    private void SupportingList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                ITodoItem todo = (ITodoItem)item;

                TodoItems.InsertSorted(todo, ascending: true);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems)
                TodoItems.Remove((ITodoItem)item);
        }
        else
        {
            Logger.LogInformation($"Unhandled CollectionChanged action: '{e.Action}'");
        }
    }

    private void TodoItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ShowEmptyView = sender is ObservableCollection<ITodoItem> { Count: <= 0 };
    }

    bool disposed;

    protected override void Dispose(bool disposing)
    {
        if (!disposed && disposing)
        {
            _queryMap.Dispose();

            disposed = true;
        }
        base.Dispose(disposing);
    }
}
