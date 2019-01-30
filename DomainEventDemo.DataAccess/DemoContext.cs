using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DomainEventDemo.DataAccess
{
    public class DemoContext : DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public DemoContext(DbContextOptions<DemoContext> options, IDomainEventDispatcher dispatcher) : base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<Parent> Parents { get; set; }

        // This overload will get called from SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            var events = GetChangeEvents();
            var changeCount = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (events != null && events.Length > 0)
            {
                await Task.WhenAll(events.Select(_dispatcher.DispatchAsync));
            }

            return changeCount;
        }
        
        // This overload will get called from SaveChanges()
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var events = GetChangeEvents();
            var changeCount = base.SaveChanges(acceptAllChangesOnSuccess);

            if (events != null && events.Length > 0)
            {
                Task.WaitAll(events.Select(_dispatcher.DispatchAsync).ToArray());
            }

            return changeCount;
        }

        private EntityChangedEvent[] GetChangeEvents()
        {
            var changeEntries = ChangeTracker
                .Entries()
                .Where(x => x.State != EntityState.Unchanged || x.State == EntityState.Detached)
                .ToArray();

            return changeEntries.Select(GetEntityChangedEvent).ToArray();
        }

        private EntityChangedEvent GetEntityChangedEvent(EntityEntry entityEntry)
        {
            Type typedEventType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    typedEventType = typeof(EntityAddedEvent<>).MakeGenericType(entityEntry.Metadata.ClrType);
                    break;
                case EntityState.Modified:
                    typedEventType = typeof(EntityUpdatedEvent<>).MakeGenericType(entityEntry.Metadata.ClrType);
                    break;
                case EntityState.Deleted:
                    typedEventType = typeof(EntityDeletedEvent<>).MakeGenericType(entityEntry.Metadata.ClrType);
                    break;
                default:
                    typedEventType = typeof(EntityChangedEvent<>).MakeGenericType(entityEntry.Metadata.ClrType);
                    break;
            }

            var typedEvent = (EntityChangedEvent)Activator.CreateInstance(typedEventType);

            typedEvent.EntityType = entityEntry.Metadata.ClrType;
            typedEvent.Original = entityEntry.State == EntityState.Added ? null : entityEntry.OriginalValues.ToObject();
            typedEvent.Modified = entityEntry.State == EntityState.Deleted ? null : entityEntry.CurrentValues.ToObject();

            return typedEvent;
        }
    }
}