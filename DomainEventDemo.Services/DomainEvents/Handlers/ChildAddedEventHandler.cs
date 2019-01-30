using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;

namespace DomainEventDemo.Services.DomainEvents.Handlers
{
    public class ChildAddedEventHandler : IHandle<EntityAddedEvent<Child>>
    {
        public Task HandleAsync(EntityAddedEvent<Child> domainEvent)
        {
            System.Diagnostics.Debug.WriteLine("ChildAddedEventHandler: got event: {0}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}