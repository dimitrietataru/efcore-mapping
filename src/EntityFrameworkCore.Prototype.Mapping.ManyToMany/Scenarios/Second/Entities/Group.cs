using System.Collections.Generic;

namespace EFCore.Mapping.M2M.Scenarios.Second.Entities
{
    public sealed class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
