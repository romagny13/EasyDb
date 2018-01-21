# Insert, Update and Delete Queries

## Insert Query

### Get the Query

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var user = new User
{
     FirstName = "Marie",
     LastName = "Bellin",
     Age = 20,
     Email ="marie@domain.com"
};

var result = db.InsertInto("users")
               .Values("firstname", user.FirstName)
               .Values("lastname", user.LastName)
               .Values("age", user.Age)
               .Values("email", user.Email)
              .GetQuery();
// insert into [users] ([firstname],[lastname],[age],[email]) output inserted.id values (@firstname,@lastname,@age,@email)
```

### Execute and get the Last Inserted Id

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var user = new User
{
    FirstName = "new firstname",
    LastName = "new lastname",
    Age = 20,
    Email = "new@domain.com"
};

var result = await db.InsertInto("users")
                     .Values("firstname", user.FirstName)
                     .Values("lastname", user.LastName)
                     .Values("age", user.Age)
                     .Values("email", user.Email)
                     .LastInsertedId<int>()
```
_For Guid replace "int" by "Guid"_


## Fetch

Returns the Model result

```cs
var db = this.GetDb();

Mapping.SetTable("users").SetPrimaryKeyColumn("id", "id");

var user = new User
{
    FirstName = "new firstname",
    LastName = "new lastname",
    Age = 20,
    Email = "new@domain.com"
};

var result = await db.InsertInto("users")
                     .Values("firstname", user.FirstName)
                     .Values("lastname", user.LastName)
                     .Values("age", user.Age)
                     .Values("email", user.Email)
                     .Fetch<User>(Mapping.GetTable("users"))
```

With **Mapping**

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

Mapping.SetTable("users")
       .SetPrimaryKeyColumn("id", "Id")
       .SetColumn("firstname","FirstName")
       .SetColumn("lastname", "LastName")
       .SetColumn("age", "Age")
       .SetColumn("email", "Email");

var user = new User
{
    FirstName = "new firstname",
    LastName = "new lastname",
    Age = 20,
    Email = "new@domain.com"
};

var result = await db.InsertInto("users")
                     .Values("firstname", user.FirstName)
                     .Values("lastname", user.LastName)
                     .Values("age", user.Age)
                     .Values("email", user.Email)
                     .Fetch<User>(Mapping.GetTable("users"));
```

## Update Query

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var user = new User
{
    Id = 1,
    FirstName = "Updated firstname",
    LastName = "Updated lastname"
};

var rowAffected = await db
                    .Update("users")
                    .Set("firstname", user.FirstName)
                    .Set("lastname", user.LastName)
                    .Set("age", user.Age)
                    .Set("email", user.Email)
                    .Where(Check.Op("id", user.Id))
                    .NonQueryAsync();
```

_We could get the Query with the function GetQuery. Example:_

```sql
update [users] set [firstname]=@firstname,[lastname]=@lastname,[age]=@age,[email]=@email where [id]=@id
```

## Delete Query

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var result = await db
                .DeleteFrom("posts")
                .Where(Check.Op("id", 2))
                .NonQueryAsync();
```

_We could get the Query with the function GetQuery_
