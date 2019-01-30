using System.Threading.Tasks;

namespace DomainEventDemo.Core.Interfaces
{
    public interface IObjectCloner
    {
        T Clone<T>(T source);
    }
}