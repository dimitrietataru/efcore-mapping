# Entity Framework Core - Mapping

## Install
``` powershell
PM> Install-Package Microsoft.EntityFrameworkCore -Version 5.0.1
PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 5.0.1
```

## Many-to-Many (M2M)
### Scenario 1
``` csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Membership> Memberships { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Membership> Memberships { get; set; }
}

public class Membership
{
    public int Id { get; set; }
    public User User { get; set; }
    public Group Group { get; set; }
}

public class DataContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Group> Groups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("..")
            .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .EnableSensitiveDataLogging();
    }
}

// Query sample
var usersInFizzGroup = await context
    .Users
    .Where(user => user.Memberships.Any(membership => membership.Group.Name.Equals("fizz")))
    .ToListAsync();
```

``` sql
CREATE TABLE [Users] (
  [Id] int NOT NULL IDENTITY,
  [Name] nvarchar(max) NULL,
  CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Groups] (
  [Id] int NOT NULL IDENTITY,
  [Name] nvarchar(max) NULL,
  CONSTRAINT [PK_Groups] PRIMARY KEY ([Id])
);

CREATE TABLE [Membership] (
  [Id] int NOT NULL IDENTITY,
  [UserId] int NULL,
  [GroupId] int NULL,
  CONSTRAINT [PK_Membership] PRIMARY KEY ([Id]),
  CONSTRAINT [FK_Membership_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE NO ACTION,
  CONSTRAINT [FK_Membership_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

SELECT [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE EXISTS (
  SELECT 1
  FROM [Membership] AS [m]
  LEFT JOIN [Groups] AS [g] ON [m].[GroupId] = [g].[Id]
  WHERE ([u].[Id] = [m].[UserId]) AND ([g].[Name] = N'group-fizz'))
```
