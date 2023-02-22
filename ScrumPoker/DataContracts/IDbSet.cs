using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ScrumPoker.DataContracts
{
    public interface IDbSet<TEntity> : System.Collections.Generic.IEnumerable<TEntity>, System.ComponentModel.IListSource, System.Linq.IQueryable<TEntity>
        where TEntity : class, new()
    {
    }
}