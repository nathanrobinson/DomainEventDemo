using System.Threading.Tasks;
using DomainEventDemo.Core.Events;

namespace DomainEventDemo.Core.Interfaces
{
    public interface IHandle<in T> where T : DomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}