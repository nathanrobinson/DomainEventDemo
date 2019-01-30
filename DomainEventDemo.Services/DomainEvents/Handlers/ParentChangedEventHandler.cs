using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;

namespace DomainEventDemo.Services.DomainEvents.Handlers
{
    public class ParentChangedEventHandler : IHandle<EntityChangedEvent<Parent>>
    {
        public Task HandleAsync(EntityChangedEvent<Parent> domainEvent)
        {
            System.Diagnostics.Debug.WriteLine("ParentChangedEventHandler: got event: {0}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}