using System.Threading.Tasks;
using DomainEventDemo.Core.Events;
using DomainEventDemo.Core.Interfaces;

namespace DomainEventDemo.Services.DomainEvents.Handlers
{
    public class EntityChangedEventHandler : IHandle<EntityChangedEvent>
    {
        public Task HandleAsync(EntityChangedEvent domainEvent)
        {
            System.Diagnostics.Debug.WriteLine("EntityChangedEventHandler: got event: {0}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}