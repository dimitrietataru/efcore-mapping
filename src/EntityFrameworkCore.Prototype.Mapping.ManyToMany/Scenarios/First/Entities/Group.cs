using System.Collections.Generic;

namespace EFCore.Mapping.M2M.Scenarios.First.Entities
{
    public sealed class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}
