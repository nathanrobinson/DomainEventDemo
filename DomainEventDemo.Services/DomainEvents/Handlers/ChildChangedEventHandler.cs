using System.Threading.Tasks;
using DomainEventDemo.Core.Entities;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;

namespace DomainEventDemo.Services.DomainEvents.Handlers
{
    public class ChildChangedEventHandler : IHandle<EntityChangedEvent<Child>>
    {
        public Task HandleAsync(EntityChangedEvent<Child> domainEvent)
        {
            System.Diagnostics.Debug.WriteLine("ChildChangedEventHandler: got event: {0}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}