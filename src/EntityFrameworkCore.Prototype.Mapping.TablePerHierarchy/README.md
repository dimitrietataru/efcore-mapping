# Table-per-Hierarchy (TPH)


### Structure | Entities
``` csharp
public class Animal
{
    public int Id { get; set; }
    public string Species { get; set; }
}

public class Pet : Animal
{
    public string Name { get; set; }
}

public class Cat : Pet
{
    public string EducationLevel { get; set; }
}

public class Dog : Pet
{
    public string FavoriteToy { get; set; }
}
```

### Configuration | DbContext
``` csharp
public class DataContext : DbContext
{
    public virtual DbSet<Animal> Animals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("connection-string")
            .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cat>();
        modelBuilder.Entity<Dog>();
    }
}
```

### Auto-generated SQL code
``` sql
CREATE TABLE [Animals] (
  [Id] int NOT NULL IDENTITY,
  [Species] nvarchar(max) NULL,
  [Discriminator] nvarchar(max) NOT NULL, /* EF magic */
  [EducationLevel] nvarchar(max) NULL,
  [Name] nvarchar(max) NULL,
  [FavoriteToy] nvarchar(max) NULL,
  CONSTRAINT [PK_Animals] PRIMARY KEY ([Id])
);
```

### Query
``` csharp
var catsWithPhd = await context
    .Set<Cat>()
    .Where(cat => cat.EducationLevel.Equals("PhD"))
    .ToListAsync();

var dogsWhoLikeBalls = await context
    .Set<Dog>()
    .Where(dog => dog.FavoriteToy.Equals("ball"))
    .ToListAsync();
```

``` sql
SELECT [a].[Id], [a].[Discriminator], [a].[Species], [a].[EducationLevel], [a].[Name]
FROM [Animals] AS [a]
WHERE ([a].[Discriminator] = N'Cat') AND ([a].[EducationLevel] = N'PhD')

SELECT [a].[Id], [a].[Discriminator], [a].[Species], [a].[FavoriteToy], [a].[Name]
FROM [Animals] AS [a]
WHERE ([a].[Discriminator] = N'Dog') AND ([a].[FavoriteToy] = N'ball')
```
