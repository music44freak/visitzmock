using Realms;

namespace VisitzModel.Models;

#nullable enable

public partial class ObservableRealmQueryMap : IDisposable
{
    private bool disposedValue;

    Dictionary<Type, (Realm, IQueryable<IRealmObject>, IDisposable QueryToken)> Queries { get; } = [];

    public event EventHandler<(Type Type, IRealmCollection<IRealmObject> Items, ChangeSet? Changes)>? ItemsChanged;

    public (Realm Realm, IQueryable<IRealmObject> Query, IDisposable QueryToken) this[Type key] => Queries[key];

    public void Subscribe<T>(Realm realm, IQueryable<T> query)
        where T : IRealmObject
    {
        var queryToken = query.SubscribeForNotifications(Query_ItemsChanged);
        Queries[typeof(T)] = (realm, (IQueryable<IRealmObject>)query, queryToken);
    }

    public void Unsubscribe<T>()
        where T : IRealmObject
    {
        if (Queries.ContainsKey(typeof(T)))
        {
            Queries[typeof(T)].QueryToken.Dispose();
            Queries.Remove(typeof(T));
        }
    }

    public void UnsubscribeAll()
    {
        foreach (var (_, _, token) in Queries.Values)
            token.Dispose();

        Queries.Clear();
    }

    void Query_ItemsChanged<T>(IRealmCollection<T> sender, ChangeSet? changes)
        where T : IRealmObject
    {
        ItemsChanged?.Invoke(this, (typeof(T), (IRealmCollection<IRealmObject>)sender, changes));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                UnsubscribeAll();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
