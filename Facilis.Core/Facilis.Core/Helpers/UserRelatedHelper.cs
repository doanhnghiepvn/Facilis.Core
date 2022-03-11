using Facilis.Core.Abstractions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Facilis.Core.Helpers
{
    public static class UserRelatedHelper
    {
        public static IQueryable<T> WhereEnabledByUserId<T>(
            this IEntities<T> entities,
            string userId,
            Expression<Func<T, bool>> expression = null
        ) where T : IEntityWithId, IEntityWithStatus, IUserRelatedEntity
        {
            return entities
                .WhereEnabled(expression)
                .Where(entity => entity.UserId == userId);
        }
    }
}