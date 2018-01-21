# Simple Commands with SQL

* ReadAllAsync
* ReadOneAsync
* NonQueryAsync
* ScalarAsync

## ReadAllAsync

Returns a _list_

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var result = await db.CreateCommand("select * from [users]")
                     .ReadAllAsync<User>();
```

## ReadOneAsync

Returns _one_ instance _or the first_ instance

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var result = await db.CreateCommand("select * from [users] where id=@id")
                     .AddParameter("@id", 1)
                     .ReadOneAsync<User>();
```

With **DbType**

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var result = await db.CreateCommand("select * from users where [id]=@id")
                     .AddParameter("@id", 1, DbType.Int16)
                     .ReadOneAsync<User>();
```

## With Mapping

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

Mapping
       .SetTable("users")
       .SetColumn("id", "Id")
       .SetColumn("firstname", "FirstName")
       .SetColumn("lastname", "LastName")
       .SetColumn("age", "Age")
       .SetColumn("email", "Email");


var result = await db.CreateCommand("select * from [users] where [id]=@id")
                     .AddParameter("@id", 1)
                     .ReadOneAsync<User>(Mapping.GetTable("users")); // pass the table mapping
```

## NonQueryAsync

Returns the _number_ of _row affected_

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var user = new User
{
    FirstName = "Alexandra",
    LastName = "Bellin",
    Age = 20,
    Email = "alex@domain.com"
};

var sql = "insert into [users] (firstname,lastname,age,email) values(@firstname,@lastname,@age,@email)";
var result = db.CreateCommand(sql)
               .AddParameter("@firstname", user.FirstName)
               .AddParameter("@lastname", user.LastName)
               .AddParameter("@age", user.Age)
               .AddParameter("@email", user.Email);
```

## ScalarAsync

Returns the _object result_

_Example:_ get the **last inserted id**

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var user = new User
{
    FirstName = "Alexandra",
    LastName = "Bellin",
    Age = 20,
    Email = "alex@domain.com"
};

// output inserted.id + scalar
var sql = "insert into [users] (firstname,lastname,age,email) output inserted.id values(@firstname,@lastname,@age,@email)";
var lastInsertedId = await db.CreateCommand(sql)
                             .AddParameter("@firstname", user.FirstName)
                             .AddParameter("@lastname", user.LastName)
                             .AddParameter("@age", user.Age)
                             .AddParameter("@email", user.Email)
                             .ScalarAsync();
```

... or With a _Guid_

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);


var sql = "insert into [UsersWithGuid] (Name,Email) output inserted.id values(@name,@email)";
var result = (Guid) await db.CreateCommand(sql)
                            .AddParameter("@name", "Marie")
                            .AddParameter("@email", "marie@domain.com")
                            .ScalarAsync();
```

## Stored procedure

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

var result = await db.CreateStoredProcedureCommand("GetUser") // procedure name
                     .AddParameter("@id", 1) 
                     .ReadOneAsync<User>();
```

## Output Parameters

For example with MySQL:

```sql
DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `get_output_age` (IN `p_id` INT, OUT `p_age` INT)  NO SQL
select age into p_age from users where id=p_id$$
DELIMITER ;
```

```cs
var command = db
               .CreateStoredProcedureCommand("get_output_age")
               .AddParameter("p_id", 2)
               .AddOutputParameter("p_age");

await command.ReadOneAsync<User>();

var parameter = command.GetParameter("p_age");
var age = parameter.Value;
```

## Functions

For example with MySQL:
```sql
DELIMITER $$
CREATE DEFINER=`root`@`localhost` FUNCTION `get_username_function` (`p_id` INT) RETURNS VARCHAR(255) CHARSET latin1 NO SQL
begin
DECLARE result varchar(255);
select username into result from users where id=p_id;
return result;
end$$
DELIMITER ;
```

```cs
var command = db.CreateCommand("select `get_username_function`(@id);")
               .AddParameter("@id", 2);

var username = await command.ScalarAsync();
```

## Nullable and String

Nullables and string with **null** are **converted** to **DBNull**


