using EFCore.Mapping.TPH.Scenario;
using EFCore.Mapping.TPH.Scenario.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

await RunScenarioAsync();

Console.WriteLine("Press any key to exit..");
Console.ReadKey();

static async Task RunScenarioAsync()
{
    await using var context = new DataContext();
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    var animal1 = new Animal { Species = "foo" };
    var animal2 = new Animal { Species = "bar" };

    var cat1 = new Cat { Species = "feline", Name = "fizz", EducationLevel = "PhD" };
    var cat2 = new Cat { Species = "feline", Name = "buzz", EducationLevel = "N/A" };

    var dog1 = new Dog { Species = "german", Name = "lorem", FavoriteToy = "ball" };
    var dog2 = new Dog { Species = "germal", Name = "ipsum", FavoriteToy = "N/A" };

    context.AddRange(animal1, animal2, cat1, cat2, dog1, dog2);
    _ = await context.SaveChangesAsync();

    var catsWithPhd = await context
        .Set<Cat>()
        .Where(cat => cat.EducationLevel.Equals("PhD"))
        .ToListAsync();

    foreach (var cat in catsWithPhd)
    {
        Console.WriteLine($"Cat: { cat.Name }");
    }

    var dogsWhoLikeBalls = await context
        .Set<Dog>()
        .Where(dog => dog.FavoriteToy.Equals("ball"))
        .ToListAsync();

    foreach (var dogs in dogsWhoLikeBalls)
    {
        Console.WriteLine($"Dog: { dogs.Name }");
    }

    await context.DisposeAsync();

    Console.WriteLine("Press any key to continue..");
    Console.ReadKey();
    Console.Clear();
}
