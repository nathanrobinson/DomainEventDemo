using System.Collections;
using System.Collections.Generic;

namespace DomainEventDemo.Core.Entities
{
    public class Parent
    {
        public Parent()
        {
            Children = new HashSet<Child>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int JobCount { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Child> Children { get; set; }
    }
}