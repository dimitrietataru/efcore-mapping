using System.Collections.Generic;

namespace EFCore.Mapping.M2M.Scenarios.Third.Entities
{
    public sealed class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }
}
