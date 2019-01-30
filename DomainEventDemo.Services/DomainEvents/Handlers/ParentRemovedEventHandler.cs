using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;

namespace DomainEventDemo.Services.DomainEvents.Handlers
{
    public class ParentRemovedEventHandler : IHandle<EntityDeletedEvent<Parent>>
    {
        public Task HandleAsync(EntityDeletedEvent<Parent> domainEvent)
        {
            System.Diagnostics.Debug.WriteLine("ParentRemovedEventHandler: got event: {0}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}