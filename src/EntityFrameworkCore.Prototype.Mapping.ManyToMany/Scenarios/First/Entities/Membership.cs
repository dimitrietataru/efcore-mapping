namespace EFCore.Mapping.M2M.Scenarios.First.Entities
{
    public sealed class Membership
    {
        public int Id { get; set; }

        public User User { get; set; }
        public Group Group { get; set; }
    }
}
