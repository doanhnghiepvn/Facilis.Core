using System.Collections.Generic;
using System.Linq;

namespace Facilis.Core.Abstractions
{
    public interface IEntitiesWithId<T> where T : IEntityWithId
    {
        IQueryable<T> Entities { get; }

        T FindById(string id);

        T[] FindByIds(params string[] ids);

        T[] FindByIds(IEnumerable<string> ids);

        T Add(T entity);

        T[] Add(T[] entities);

        T[] Add(IEnumerable<T> entities);

        T Update(T entity);

        T[] Update(T[] entities);

        T[] Update(IEnumerable<T> entities);

        void AddNoSave(params T[] entities);

        void AddNoSave(IEnumerable<T> entities);

        void UpdateNoSave(params T[] entities);

        void UpdateNoSave(IEnumerable<T> entities);

        void Save();
    }
}