# Connection

## Connection String

Set the **Connection String**

```cs
var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\file.mdf;Integrated Security=True;Connect Timeout=30";
var providerName = "System.Data.SqlClient";

EasyDb.Default.SetConnectionStringSettings(connectionString,providerName);
```

With a **Configuration File**

```cs
EasyDb.Default.SetConnectionStringSettings(); // "Default" connection
// or
EasyDb.Default.SetConnectionStringSettings("MyConnection");
```
_Example_

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>

  <connectionStrings>
    <add name="Default"
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\mydb.mdf;Integrated Security=True;Connect Timeout=20"
         providerName="System.Data.SqlClient" />
    <add name="MyConnection"
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\mydb2.mdf;Integrated Security=True;Connect Timeout=20"
         providerName="System.Data.SqlClient" />
  </connectionStrings>

</configuration>
```

# Connection Strategy

> EasyDb use System.Data.Common (DbConnection, DbCommand, etc.).

By **default** the connection is **opened** and **closed** for **each request**.

```cs
EasyDb.Default.SetConnectionStringSettings(connectionString, providerName, ConnectionStrategy.Manual);

var result = await EasyDb.Default.CreateCommand("select * from users where [id]=@id")
                                 .AddParameter("@id", 1)
                                 .ReadOneAsync<User>();
```

**Controlling** the **connection**.

```cs
EasyDb.Default.SetConnectionStringSettings(connectionString,providerName,ConnectionStrategy.Manual);
```
.. Or
```cs
EasyDb.Default.SetConnectionStrategy(ConnectionStrategy.Manual); // "Default" or "Manual"
```
_Then_
```cs
EasyDb.Default.SetConnectionStringSettings(connectionString, providerName, ConnectionStrategy.Manual);

await EasyDb.Default.OpenAsync();

var result = await EasyDb.Default.CreateCommand("select * from users where [id]=@id")
                                 .AddParameter("@id", 1)
                                 .ReadOneAsync<User>();

EasyDb.Default.Close();
```

## Error handler

```cs
var db = new EasyDb();

db.OnError += (sender, e) =>
{
  //... intercepted
};

db.SetConnectionStringSettings("invalid", "invalid"); // exception ...
```

## Create an EasyDb instance or use Default Static instance

Use the default **Static** instance

```cs
EasyDb.Default.SetConnectionStringSettings("MyConnection");

var user = await EasyDb.Default
                       .CreateCommand("select * from [users] where [id]=@id")
                       .AddParameter("@id", 1)
                       .ReadOneAsync<User>();
```

... Or **create an instance**

```cs
IEasyDb db = new EasyDb();

db.SetConnectionStringSettings("MyConnection");

var user = await db.CreateCommand("select * from [users] where [id]=@id")
                   .AddParameter("@id", 1)
                   .ReadOneAsync<User>();
```

## Other usefull Connection methods

* OpenAsync
* Close
* IsOpen
* IsClosed
* GetState
