using System;

namespace DomainEventDemo.Core.Events
{
    public class EntityChangedEvent : DomainEvent
    {
        public object Original { get; set; }
        public object Modified { get; set; }
        public ActionType ChangeType { get; set; }
    }

    public class EntityChangedEvent<T> : EntityChangedEvent
    {
        public T OriginalEntity
        {
            get => (T)Original;
            set => Original = value;
        }
        public T ModifiedEntity
        {
            get => (T)Modified;
            set => Modified = value;
        }
    }

    public class EntityAddedEvent<T> : EntityChangedEvent<T>
    {
        public EntityAddedEvent()
        {
            ChangeType = ActionType.Add;
        }
    }

    public class EntityUpdatedEvent<T> : EntityChangedEvent<T>
    {
        public EntityUpdatedEvent()
        {
            ChangeType = ActionType.Change;
        }
    }

    public class EntityDeletedEvent<T> : EntityChangedEvent<T>
    {
        public EntityDeletedEvent()
        {
            ChangeType = ActionType.Remove;
        }
    }
}