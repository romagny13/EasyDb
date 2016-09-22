# EasyDb (v2)

Library for data access with .NET projects.

Installation with NuGet Package
```
PM> Install-Package EasyDb
```

## Initialization

We can create a new instance of "Db" or use Db.Default (singleton).


### With connection string and provider

Example with SQL Server file and SqlClient

```cs
Db.Default.Initialize(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\path\to\myDb.mdf;Integrated Security=True", "System.Data.SqlClient");
```

EasyDb use "System.Data.Common" abstractions and "DbConnection", "DbCommand",etc. So we can connect to what we want : SQL Server, MS Access, MySQL, etc.

Example with MySQL . After installing <a href="https://dev.mysql.com/downloads/connector/net/6.9.html">MySQL Connector for .NET</a>.

```cs
var db = new Db();
db.Initialize(@"server=localhost;user id=root;database=mydb", "MySql.Data.MySqlClient");

var user = db.CreateCommand("select * from users where id=@id")
                .AddParameter("@id", 1)
                .ReadOneMapTo<MySQLUser>();
```         

Example with MS Access

```cs
var db = new Db();
db.Initialize(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\Documents\Visual Studio 2015\Projects\EasyDb\EasyDbTests\NorthWind.mdb", "System.Data.OleDb");

var category = db.CreateCommand("select * from Categories where id=@id")
                .AddParameter("@id",1)
                .ReadOneMapTo<Category>();
``` 

### With configuration file

Example with "App.config" (Wpf or Windows forms project). 
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
    
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\path\to\myDb.mdf;Integrated Security=True" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
</configuration>
```
Note : In "Web.config" for Asp.Net application.

With "DefaultConnection" 

```cs
db.InitializeWithConfigurationFile();
```
Or with other connection
```cs
db.InitializeWithConfigurationFile("MyConnection");
```

## Connection strategy
 Default is "Automatic". Connection is opened before each request and closed after.
 
 We can define the strategy to "Manual" (last parameter of initializations methods)
 
```cs
Db.Default.Initialize(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\path\to\myDb.mdf;Integrated Security=True", "System.Data.SqlClient",ConnectionStrategyType.Manual);
```

... and control when open or close the connection

 ```cs
Db.Default.Open(); // or OpenAsync

// do requests

Db.Default.Close();
```
 
## Read All

With sql query and function that intercept data reader to map data to object

```cs
var users = Db.Default.CreateCommand("select * from [dbo].[Users]")
    .ReadAll((reader) =>
    {
        return new User
        {
            Id = (int)reader["Id"],
            UserName = (string)reader["Name"],
            Email = reader["Email"] == DBNull.Value ? null : (string)reader["Email"],
            Age = reader["Age"] == DBNull.Value ? null : (int?)reader["Age"],
            Birth = reader["Birth"] == DBNull.Value ? null : (DateTime?)reader["Birth"]
        };
    });
```

### Map

EasyDb can resolve by reflection the properties
 ```cs
 var users = Db.Default.CreateCommand("select * from [dbo].[Users]").ReadAllMapTo<User>();
```

For properties that not correspond to table column name, we can use "MapAttribute" on properties

Example column "Name" is "UserName" for property. 

```cs
public class User
{
    public int Id { get; set; }
        
    // column name is required with Map Attribute
    [MapAttribute(ColumnName = "Name")]
    public string UserName { get; set; }
        
    public string Email { get; set; }
    public int? Age { get; set; }
    public DateTime? Birth { get; set; }
}
```
Could simplify to
```cs
[Map(ColumnName = "Name")]
public string UserName { get; set; }
```

### Async

All operations have sync and Async signatures.

Example

 ```cs
 var users = await Db.Default.CreateCommand("select * from [dbo].[Users]").ReadAllMapToAsync<User>();
```

## Read One

Add parameter to command

 ```cs
var user = Db.Default.CreateCommand("select * from [dbo].[Users] where Id=@id")
    .AddParameter("@id", 10)
    .ReadOneMapTo<User>();
 ```
Read one with key as Guid
```cs
var user = db.CreateCommand("select * from [dbo].[UsersWithGuid] where Id=@id")
    .AddParameter("@id", "39544cae-1342-4ab4-9d3c-921ffd93ac62")
    .ReadOneMapTo<UserWithGuid>();
```

The class
```cs
public class UserWithGuid
{
    public Guid Id { get; set; }
    
    [MapAttribute(ColumnName = "Name")]
    public string UserName { get; set; }
    // etc.
}
```

## Scalar
 
Get an integer value. 
 
Example count 
```cs
// execute scalar return an object
var result = Db.Default.CreateCommand("select count(*) from [dbo].[Users]").Scalar();

// use try parse to get the int
int count;
if(int.TryParse(result.ToString(),out count))
{
   
}
```

### Insert data and get the last inserted Key (auto incremented)

With "output INSERTED..."
```cs
var result = Db.Default.CreateCommand("insert into [dbo].[Users](Name) output INSERTED.Id values(@name)")
        .AddParameter("@name", "Marie")
        .Scalar();
// get key     
int key;
if(int.TryParse(result.ToString(),out key))
{

}
```
Or with "SCOPE_IDENTITY" 
```cs
var result = Db.Default.CreateCommand("insert into [dbo].[Users](Name) values(@name);SELECT SCOPE_IDENTITY()")
        .AddParameter("@name", "Marie")
        .Scalar();
```

### Get last key generated for Guid (C#)

Example : create a table with a primary key as "uniqueidentifier" and with default "NEWID()" (SQL Server)

```sql
CREATE TABLE [dbo].[UsersWithGuid]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID ( ), 
    [Name] NCHAR(10) NOT NULL, 
    [Email] NCHAR(20) NULL 
)
```

Then, insert data and get last guid inserted
```cs
var newGuid = db.CreateCommand("insert into [dbo].[UsersWithGuid](Name,Email) output INSERTED.Id values(@name,@email)")
    .AddParameter("@name", "Marie")
    .AddParameter("@email", "marie@mail.com")
    .Scalar();
```


## NonQuery
 
For insert, update, ... data
 
Example insert

```cs
var result = await Db.Default.CreateCommand("insert into [dbo].[Users](Name,Email,Age,Birth) values(@name,@email,@age,@birth)")
    .AddParameter("@name", "Marie")
    .AddParameter("@email", "marie@mail.com")
    .AddParameter("@age", 20)
    .AddParameter("@birth", DBNull.Value)
    .NonQueryAsync();
```

## Stored procedure

Example create a stored procedure with SQL Server database

```sql
CREATE PROCEDURE [dbo].[GetUser]
	@id int
AS
	SELECT * from [dbo].[Users] where Id=@id;
RETURN 0
```

Next, create and execute a command 
```cs
var user = Db.Default.CreateStoredProcedureCommand("GetUser")
        .AddParameter("@id", 11)
        .ReadOneMapTo<User>();
```
## Relations

Example  zero one and one one relations

- zero one : the table has a foreign key nullable
- one one : the table has a foreign key not nullable 

```cs
public class BaseTable
{
    public int Id { get; set; }
    public string Name { get; set; }

    // zero one : use a nullable
    [Map(ColumnName = "ZeroOneFKey")]
    public int? ZeroOneId { get; set; }
    public ZeroOne ZeroOne { get; set; }

    // one one
    [Map(ColumnName ="OneOneFKey")]
    public int OneOneId { get; set; }
    public OneOne OneOne { get; set; }

    // for many to many, we have a list
    public List<ManyTable> ManyList { get; set; }
}
public class ZeroOne
{
    public int Id { get; set; }
    public string Name { get; set; }
}
// etc.
```

Get objects for foreign keys
```cs
var db = new Db();
db.Initialize(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=c:\path\to\myDb.mdf;Integrated Security=True", "System.Data.SqlClient");

var result = await db.CreateCommand("select * from [dbo].[BaseTable] where Id=@id")
    .AddParameter("@id", 5)
    .ReadOneMapToAsync<BaseTable>();

// zero one
if (result.ZeroOneId.HasValue)
{
    var zeroOne = await db.CreateCommand("Select * from [dbo].[ZeroOne] where Id=@id")
        .AddParameter("@id", result.ZeroOneId)
        .ReadOneMapToAsync<ZeroOne>();
    result.ZeroOne = zeroOne;
}

// one one
var oneOne = await db.CreateCommand("Select * from [dbo].[OneOne] where Id=@id")
    .AddParameter("@id", result.OneOneId)
    .ReadOneMapToAsync<OneOne>();
result.OneOne = oneOne;
```


## Transactions

Use TransactionScope for example

```cs
try
{
    using (var scope = new TransactionScope())
    {
        var result = Db.Default.CreateCommand("delete from [dbo].[Users] where Id=@id")
                                .AddParameter("@id", 1)
                                .NonQuery();

        var result2 = Db.Default.CreateCommand("delete from [dbo].[Users] where Id=@id")
                                .AddParameter("@id", 2)
                                .NonQuery();

        scope.Complete();
    }
}
catch (Exception ex)
{ }
```
