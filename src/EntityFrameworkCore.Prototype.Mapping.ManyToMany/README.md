# Many-to-Many (M2M)


## Scenario 1

> An user *has many* Groups. A group *has many* Users.  
> The **Membership** table is explicitly declared.

### Structure | Entities
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
```

### Configuration | DbContext
``` csharp
public class DataContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Group> Groups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("connection-string")
            .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .EnableSensitiveDataLogging();
    }
}
```

### Auto-generated SQL code
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
```

### Query
``` csharp
var usersInFizzGroup = await context
    .Users
    .Where(user => user.Memberships.Any(membership => membership.Group.Name.Equals("fizz")))
    .ToListAsync();
```

``` sql
SELECT [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE EXISTS (
  SELECT 1
  FROM [Membership] AS [m]
  LEFT JOIN [Groups] AS [g] ON [m].[GroupId] = [g].[Id]
  WHERE ([u].[Id] = [m].[UserId]) AND ([g].[Name] = N'fizz'))
```



## Scenario 2

> An user *has many* Groups. A group *has many* Users.  
> The **GroupUser** table is not declared, but it is auto-generated by EF Core.

### Structure/Entities
``` csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Group> Groups { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; }
}
```

### Configuration | DbContext
``` csharp
public class DataContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Group> Groups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("connection-string")
            .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .EnableSensitiveDataLogging();
    }
}
```

### Auto-generated SQL code
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

CREATE TABLE [GroupUser] (
  [GroupsId] int NOT NULL,
  [UsersId] int NOT NULL,
  CONSTRAINT [PK_GroupUser] PRIMARY KEY ([GroupsId], [UsersId]),
  CONSTRAINT [FK_GroupUser_Groups_GroupsId] FOREIGN KEY ([GroupsId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE,
  CONSTRAINT [FK_GroupUser_Users_UsersId] FOREIGN KEY ([UsersId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
```

### Query
``` csharp
var usersInFizzGroup = await context
    .Users
    .Where(user => user.Groups.Any(group => group.Name.Equals("fizz")))
    .ToListAsync();
```

``` sql
SELECT [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE EXISTS (
  SELECT 1
  FROM [GroupUser] AS [g]
  INNER JOIN [Groups] AS [g0] ON [g].[GroupsId] = [g0].[Id]
  WHERE ([u].[Id] = [g].[UsersId]) AND ([g0].[Name] = N'fizz'))
```



## Scenario 3

> An user *has many* Groups. A group *has many* Users.  
> The **GroupUser** table is explicitly declared. *Payload data* is stored on the GroupUser table.

### Structure/Entities
``` csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Group> Groups { get; set; }
    public ICollection<Membership> Memberships { get; set; }
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; }
    public ICollection<Membership> Memberships { get; set; }
}

public class Membership
{
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; }
    public Group Group { get; set; }
}
```

### Configuration | DbContext
``` csharp
public class DataContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Group> Groups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("connection-string")
            .LogTo(System.Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<User>()
            .HasMany(user => user.Groups)
            .WithMany(group => group.Users)
                .UsingEntity<Membership>(
                    rightBuilder => rightBuilder
                        .HasOne(membership => membership.Group)
                        .WithMany(group => group.Memberships),
                    leftBuilder => leftBuilder
                        .HasOne(membership => membership.User)
                        .WithMany(user => user.Memberships))
                .ToTable("GroupUser");
    }
}
```

### Auto-generated SQL code
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

CREATE TABLE [GroupUser] (
  [GroupId] int NOT NULL,
  [UserId] int NOT NULL,
  [JoinedAt] datetime2 NOT NULL,
  CONSTRAINT [PK_GroupUser] PRIMARY KEY ([GroupId], [UserId]),
  CONSTRAINT [FK_GroupUser_Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups] ([Id]) ON DELETE CASCADE,
  CONSTRAINT [FK_GroupUser_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
```

### Query
``` csharp
var usersInFizzGroup = await context
    .Users
    .Where(user => user.Groups.Any(group => group.Name.Equals("fizz")))
    .ToListAsync();

var usersWhoJoinedInTheLastYear = await context
    .Users
    .Where(user => user.Memberships.Any(m => m.JoinedAt > DateTime.UtcNow.AddYears(-1)))
    .ToListAsync();
```

``` sql
SELECT [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE EXISTS (
  SELECT 1
  FROM [GroupUser] AS [g]
  INNER JOIN [Groups] AS [g0] ON [g].[GroupId] = [g0].[Id]
  WHERE ([u].[Id] = [g].[UserId]) AND ([g0].[Name] = N'fizz'))

SELECT [u].[Id], [u].[Name]
FROM [Users] AS [u]
WHERE EXISTS (
  SELECT 1
  FROM [GroupUser] AS [g]
  WHERE ([u].[Id] = [g].[UserId]) AND ([g].[JoinedAt] > DATEADD(year, CAST(-1 AS int), GETUTCDATE())))
```
