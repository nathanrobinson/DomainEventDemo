using System;

namespace DomainEventDemo.Core.Events
{
    public class DomainEvent
    {
        public Type EntityType { get; set; }
    }
}