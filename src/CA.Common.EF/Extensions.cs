using CA.Common.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Common.EF
{
    public static class DbExtensions
    {
        /// <summary>
        /// Extension method to update many to many relationships. Should be called for already tracked entity.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity being update</typeparam>
        /// <typeparam name="TDEntity">Type of the dependant entity</typeparam>
        /// <param name="dbContext">Db Context</param>
        /// <param name="dEntitiesSet">The DbSet property of the dependant type</param>
        /// <param name="realationName"></param>
        /// <param name="idProperty"></param>
        /// <param name="entity"></param>
        /// <param name="requested"></param>
        /// <param name="clear"></param>
        /// <param name="assign"></param>
        public static void UpdateManyToMany<TEntity, TDEntity>(
            this DbContext dbContext,
            DbSet<TDEntity> dEntitiesSet,
            string realationName,
            string idProperty,
            ref TEntity entity,
            IEnumerable<int> requested,
            Action<TEntity> clear,
            Action<TEntity, ICollection<TDEntity>> assign)
            where TEntity : EntityBase<int>, new()
            where TDEntity : EntityBase<int>, new()
        {
            ICollection<TDEntity> dentities = default;
            if (requested is not null)
            {
                // cast the ids to the dependant entity objects
                dentities = requested.Distinct().Select(x => new TDEntity { Id = x }).ToList();
                foreach (var ent in dentities)
                {
                    // attach the entities if they are not already attached
                    if (!dEntitiesSet.Local.Any(x => x.Id == ent.Id))
                    {
                        dbContext.Attach(ent);
                    }
                }
            }
            // reset the dependant entity collection after the dbcontext start tracking it
            clear(entity);
            // don't remove entries that exists in the request
            var localEntities = dEntitiesSet.Local.Where(o => dentities.Any(x => x.Id == o.Id)).ToList();
            var entries = dbContext.ChangeTracker.Entries()
                .Where(e => e.Metadata.Name == realationName
                && localEntities.Any(x => x.Id.ToString() == e.Property(idProperty).CurrentValue.ToString()))
                .ToList();
            entries.ForEach(e => e.State = EntityState.Unchanged);
            assign(entity, localEntities);
        }
    }
}
