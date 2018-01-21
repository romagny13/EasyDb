# Commands

Command creation

* **CreateSqlCommand**
* **CreateStoredProcedureCommand**
* **CreateCommand**

DbCommand extensions methods (allows to chain registration)

* **AddInParameter**
* **AddOutParameter**
* **AddParameter**

Execution 

* **SelectAllAsync<TModel>** returns a **list of Models**
* **SelectOneAsync<TModel>** returns a **Model**
* **InsertAsync<TModel>** returns the **new Id** (for model with primary key auto incremented for example) or the number of **rows affected**
* **UpdateAsync<TModel>** returns the number of **rows affected**
* **DeleteAsync<TModel>** returns the number of **rows affected**
* **CountAsync<TModel>** returns a number
* **ExecuteScalarAsync** returns an **object**
* **ExecuteNonQueryAsync** returns the number of **rows affected**

Transactions

* **ExecutePendingOperationsAsync**
* **ExecuteTransactionFactoryAsync**

> Best practice

Use Factories:
* **ISelectionAllCommandFactory<TModel>**
* **ISelectionOneCommandFactory<TModel>**
* **IInsertCommandFactory<TModel>**
* **IUpdateCommandFactory<TModel>**
* **IDeleteCommandFactory<TModel>**
* **TransactionFactory**

## Select All

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

// arguments:
// - limit
// - condition
// - sorts
var result = await db.SelectAllAsync<User>(10, Check.Op("Age", ">", 18), new string[] { "UserName DESC" });
```

Or with a command
```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

db.SetTable<User>("users")
    .SetPrimaryKeyColumn("id", p => p.Id);

using (var command = db.CreateSqlCommand("select * from [users]"))
{
    var users = await db.SelectAllAsync<User>(command);
}
```

Or with a factories

* Command factory

```cs
public class UserSelectionAllFactory : ISelectionAllCommandFactory<User>
{
    public DbCommand CreateCommand(EasyDb db)
    {
        return db.CreateSqlCommand("select * from [User]");
    }
}
```

* Model factory (data reader => model)

```cs
public class UserModelFactory : IModelFactory<User>
{
    public User CreateModel(IDataReader reader, EasyDb db)
    {
        return new User
        {
            UserName = ((string)reader["UserName"]).Trim(),
            Email = reader["Email"] == DBNull.Value ? default(string) : ((string)reader["Email"]).Trim(),
            Age = reader["Age"] == DBNull.Value ? default(int?) : (int)reader["Age"]
        };
    }
}
```

Execution

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

var result = await db.SelectAllAsync<User>(new UserSelectionAllFactory(), new UserModelFactory());
```

Or with **CheckDBNullAndConvertTo** function

```cs
public class UserModelFactory : IModelFactory<User>
{
    public User CreateModel(IDataReader reader, EasyDb db)
    {
        return new User
        {
            UserName = ((string)reader["UserName"]).Trim(),
            Email = db.CheckDBNullAndConvertTo<string>(reader["Email"])?.Trim(),
            Age = db.CheckDBNullAndConvertTo<int?>(reader["Age"])
        };
    }
}
```

## Select One

Example:

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

Post result = null;

using (var command = db.CreateSqlCommand("select a.Title, a.Content, a.UserId, b.UserName from [Post] as a,[User] as b where a.UserId=b.Id and a.Id=@id").AddInParameter("@id", 2))
{
    result = await db.SelectOneAsync<Post>(command, (reader, idb) =>
    {
        return new Post
        {
            Title = ((string)reader["Title"]).Trim(),
            Content = ((string)reader["Content"]).Trim(),
            UserId = (int)reader["UserId"],
            // populate relation object
            User = new User
            {
                UserName = ((string)reader["UserName"]).Trim()
            }
        };
    });
}
```

## Insert

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

var user = new User
{
    UserName = "Marie"
};
var newId = await db.InsertAsync<User>(user);
```

With **command factory**

```cs
public class UserInsertFactory : IInsertCommandFactory<User>
{
    public DbCommand CreateCommand(EasyDb db, User model)
    {
        return db.CreateSqlCommand("insert into [User](UserName) output inserted.id values(@username)")
                .AddInParameter("@username", model.UserName);
    }

    public void SetNewId(DbCommand command, User model, object result)
    {
        model.Id = (int)result;
    }
}
```

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

var user = new User
{
    UserName = "Marie"
};
var result = await db.InsertAsync<User>(new UserInsertFactory(), user);
```

## Update

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

var user = new User
{
    Id = 2,
    UserName = "UserName updated"
};
var rowsAffected = await db.UpdateAsync<User>(user, Check.Op("Id", user.Id));
```

## Delete

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

var rowsAffected =  await db.DeleteAsync<User>(Check.Op("Id", 3));
```

## Transactions

### Transaction Factory

Allows to create quickly transation and not have to define "BeginTransaction" or "transaction scope", open connection, etc.

Example
```cs
public class MyTransactionOperations : TransactionFactory
{
    public override async Task ExecuteAsync(EasyDb db)
    {
        try
        {
            await db.InsertAsync<User>(new User { UserName = "New user 1" });
            await db.InsertAsync<User>(new User { UserName = "New user 2" });
            await db.InsertAsync<User>(new User { UserName = "New user 3" });
        }
        catch (System.Exception ex)
        {
            // thwrow exception to rollback 
            throw ex;
        }
    }
}
```

Execution

```cs
var db = new EasyDb();

// connection + mapping ...

var succcess = await db.ExecuteTransactionFactoryAsync(new MyTransactionOperations()); // true or false
```

### Pending transaction

Allows to register actions, and execute all actions on demand

Example:
```cs
var db = new EasyDb();

// connection + mapping ...

// 3 operations
db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "New user 1" }));
db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "New user 2" }));
db.AddPendingOperation(() => db.InsertAsync<User>(new User { UserName = "New user 3" }));

// execution
var success = await db.ExecutePendingOperationsAsync(); // if success, operations are clear
```

## Stored procedure

Example:

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(Constants.ConnectionString, Constants.ProviderName);

using (var command = db
            .CreateStoredProcedureCommand<User>("GetUser")
            .AddInParameter("@id", 1))
        {
            var user = await db.SelectOneAsync<User>(command);
        }
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
var db = this.GetDb();

// connection + mapping ...

using (var command = db.CreateStoredProcedureCommand<User>("get_output_age")
            .AddInParameter("p_id", 2)
            .AddOutParameter("p_age"))
        {
            await db.SelectOneAsync<User>(command);
            var result = command.Parameters["p_age"];
        }
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
using (var command = db.CreateSqlCommand("SELECT `get_username_function`(@p0) AS `get_username_function`;")
            .AddInParameter("@p0", 2)
            .AddParameter("get_username_function", null, ParameterDirection.ReturnValue))
        {
            // Return value
            var result = await db.ExecuteScalarAsync(command);
        }
```



