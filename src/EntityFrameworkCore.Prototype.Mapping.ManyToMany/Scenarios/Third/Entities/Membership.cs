using System;

namespace EFCore.Mapping.M2M.Scenarios.Third.Entities
{
    public sealed class Membership
    {
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public Group Group { get; set; }
    }
}
