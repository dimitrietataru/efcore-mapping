# Table-per-Type (TPT)


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
        modelBuilder.Entity<Cat>().ToTable("Cats");
        modelBuilder.Entity<Dog>().ToTable("Dogs");
    }
}
```

### Auto-generated SQL code
``` sql
CREATE TABLE [Animals] (
  [Id] int NOT NULL IDENTITY,
  [Species] nvarchar(max) NULL,
  CONSTRAINT [PK_Animals] PRIMARY KEY ([Id])
);

CREATE TABLE [Cats] (
  [Id] int NOT NULL,
  [EducationLevel] nvarchar(max) NULL,
  [Name] nvarchar(max) NULL,
  CONSTRAINT [PK_Cats] PRIMARY KEY ([Id]),
  CONSTRAINT [FK_Cats_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Dogs] (
  [Id] int NOT NULL,
  [FavoriteToy] nvarchar(max) NULL,
  [Name] nvarchar(max) NULL,
  CONSTRAINT [PK_Dogs] PRIMARY KEY ([Id]),
  CONSTRAINT [FK_Dogs_Animals_Id] FOREIGN KEY ([Id]) REFERENCES [Animals] ([Id]) ON DELETE NO ACTION
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
SELECT [a].[Id], [a].[Species], [c].[EducationLevel], [c].[Name]
FROM [Animals] AS [a]
INNER JOIN [Cats] AS [c] ON [a].[Id] = [c].[Id]
WHERE [c].[EducationLevel] = N'PhD'

SELECT [a].[Id], [a].[Species], [d].[FavoriteToy], [d].[Name]
FROM [Animals] AS [a]
INNER JOIN [Dogs] AS [d] ON [a].[Id] = [d].[Id]
WHERE [d].[FavoriteToy] = N'ball'
```
