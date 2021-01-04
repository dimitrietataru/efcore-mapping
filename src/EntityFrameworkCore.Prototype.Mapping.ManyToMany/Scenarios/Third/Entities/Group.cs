using System.Collections.Generic;

namespace EFCore.Mapping.M2M.Scenarios.Third.Entities
{
    public sealed class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}
