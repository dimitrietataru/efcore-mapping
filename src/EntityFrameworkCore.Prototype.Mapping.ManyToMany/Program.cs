using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using First = EFCore.Mapping.M2M.Scenarios.First;

await RunFirstScenarioAsync().ConfigureAwait(false);

Console.WriteLine("Press any key to exit..");
Console.ReadKey();

static async Task RunFirstScenarioAsync()
{
    await using var context = new First.DataContext();
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    var user1 = new First.Entities.User { Name = "foo" };
    var user2 = new First.Entities.User { Name = "bar" };

    var group1 = new First.Entities.Group { Name = "fizz" };
    var group2 = new First.Entities.Group { Name = "buzz" };

    var membership1 = new First.Entities.Membership { User = user1, Group = group1 };
    var membership2 = new First.Entities.Membership { User = user1, Group = group2 };
    var membership3 = new First.Entities.Membership { User = user2, Group = group1 };

    context.AddRange(user1, user2, group1, group2, membership1, membership2, membership3);
    _ = await context.SaveChangesAsync();

    var usersInFizzGroup = await context
        .Users
        .Where(user => user.Memberships.Any(membership => membership.Group.Name.Equals("fizz")))
        .ToListAsync();

    foreach (var user in usersInFizzGroup)
    {
        Console.WriteLine($"User: { user.Name }");
    }

    Console.WriteLine("Press any key to continue..");
    Console.ReadKey();
    Console.Clear();
}
