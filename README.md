# EasyDb (version 4.x)

> EasyDb is a Library for **.NET** Projects.

* **Micro ORM**
* **Mapping** explicit or by Data Annotations
* **Simple commands** with **SQL**
* **Factories** for a better project structure
* **QueryService** SqlClient, OleDb, MySqlClient with with [MySQLConnector for .NET](https://dev.mysql.com/downloads/connector/net/)
* **No dependencies**. _Do not require to update the Model._

## Installation

With [NuGet](https://www.nuget.org/packages/EasyDb/)

```
PM> Install-Package EasyDb
```

## Documentation

* [Wiki](https://github.com/romagny13/EasyDb/wiki)


<p align="center">
  <img src="http://res.cloudinary.com/romagny13/image/upload/v1475077310/easyDb_logo_lrcq7m.png"/>
</p>


## Examples

```cs
var db = new EasyDb();
db.SetConnectionStringSettings("Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MyDb;Integrated Security=True", "System.Data.SqlClient");

db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;

// arguments: limit, condition, sorts
var users = await db.SelectAllAsync<User>(10, Check.Op("Age", ">", 18), new string[] { "UserName DESC" });
```

With **factory**

```cs
var db = new EasyDb();
db.SetConnectionStringSettings("Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=MyDb;Integrated Security=True", "System.Data.SqlClient");

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
