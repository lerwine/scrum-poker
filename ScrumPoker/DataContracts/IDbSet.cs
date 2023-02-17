using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface IDbSet<T> : System.Collections.Generic.IEnumerable<TEntity>, System.ComponentModel.IListSource, System.Linq.IQueryable<TEntity>
        where T : class, new()
    {
    }
}