using CA.Common.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CA.Common.EF
{
    /// <summary>
    /// DbContext related extension methods.
    /// </summary>
    public static class DbExtensions
    {
        /// <summary>
        /// Extension method to update many to many relationships. Should be called for already tracked entity.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity being update</typeparam>
        /// <typeparam name="TDEntity">Type of the dependent entity</typeparam>
        /// <param name="dbContext">Db Context</param>
        /// <param name="dEntitiesSet">The DbSet property of the dependent type</param>
        /// <param name="relationName"></param>
        /// <param name="idProperty"></param>
        /// <param name="entity"></param>
        /// <param name="requested"></param>
        /// <param name="clear"></param>
        /// <param name="assign"></param>
        public static void UpdateManyToMany<TEntity, TDEntity>(
            this DbContext dbContext,
            DbSet<TDEntity> dEntitiesSet,
            string relationName,
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
                // cast the ids to the dependent entity objects
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
            // reset the dependent entity collection after the dbcontext start tracking it
            clear(entity);
            // don't remove entries that exists in the request
            var localEntities = dEntitiesSet.Local.Where(o => dentities.Any(x => x.Id == o.Id)).ToList();
            var entries = dbContext.ChangeTracker.Entries()
                .Where(e => e.Metadata.Name == relationName
                && localEntities.Any(x => x.Id.ToString() == e.Property(idProperty).CurrentValue.ToString()))
                .ToList();
            entries.ForEach(e => e.State = EntityState.Unchanged);
            assign(entity, localEntities);
        }

        /// <summary>
        /// Extension method to run migrations on the application host
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="host">The <see cref="IHost"/> application instance</param>
        /// <param name="seeder">The Action performing the data seeding.</param>
        /// <returns>The <see cref="IHost"/> instance.</returns>
        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    logger.LogInformation($"Database migration started with context {typeof(TContext).Name}");

                    context.Database.Migrate();

                    seeder(context, services);

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return host;
        }
    }
}
