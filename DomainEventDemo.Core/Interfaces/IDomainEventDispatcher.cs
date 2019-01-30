using System.Threading.Tasks;
using DomainEventDemo.Core.Events;

namespace DomainEventDemo.Core.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(DomainEvent domainEvent);
    }
}