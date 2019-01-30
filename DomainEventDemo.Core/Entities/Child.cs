namespace DomainEventDemo.Core.Entities
{
    public class Child
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int FriendCount { get; set; }
        public string Description { get; set; }
        public int GradeLevel { get; set; }
        public virtual Parent Parent { get; set; }
    }
}