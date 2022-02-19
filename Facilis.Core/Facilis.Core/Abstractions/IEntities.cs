using System;
using System.Linq;
using System.Linq.Expressions;

namespace Facilis.Core.Abstractions
{
    public interface IEntities<T> : IEntitiesWithId<T>
        where T : IEntityWithId, IEntityWithStatus
    {
        IQueryable<T> WhereEnabled(Expression<Func<T, bool>> expression = null);

        IQueryable<T> WhereAll(Expression<Func<T, bool>> expression, bool expectDeleted = false);
    }
}