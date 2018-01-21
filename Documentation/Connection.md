# Connection

> EasyDb use System.Data.Common (DbConnection, DbCommand, etc.).

* Connection
  * Connection Strategy
* providers :
  * System.Data.SqlClient : Sql Server
  * System.Data.OleDb : Access
  * MySql.Data.MySqlClient (MySQL with [MySQLConnector for .NET](https://dev.mysql.com/downloads/connector/net/))

* Query Services :
  * SqlQueryService (System.Data.SqlClient)
  * OleDbQueryService (System.Data.OleDb)
  * MySqlQueryService (MySql.Data.MySqlClient)

## Connection String

Set the **Connection String**

Examples:

Sql Server

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=WpfEFDb;Integrated Security=True","System.Data.SqlClient");
```

OleDb

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\NorthWind.mdb","System.Data.OleDb");
```

MySql

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(@"server=localhost;database=testdb;uid=root", "MySql.Data.MySqlClient");
```

With a **Configuration File**

```cs
db.SetConnectionStringSettings(); // "DefaultConnection"
// or
db.SetConnectionStringSettings("MyConnection");
```
_Example_

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>

  <connectionStrings>
    <add name="DefaultConnection"
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\mydb.mdf;Integrated Security=True;Connect Timeout=20"
         providerName="System.Data.SqlClient" />
    <add name="MyConnection"
         connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\mydb2.mdf;Integrated Security=True;Connect Timeout=20"
         providerName="System.Data.SqlClient" />
  </connectionStrings>

</configuration>
```

## Connection Strategy

* **Auto** (by default): the connection is **opened** and **closed** after **command execution**.
* **Manual**

It's possible to change this behavior (usefull for example for **transactions**)

```cs
db.SetConnectionStringSettings(connectionString, providerName, ConnectionStrategy.Manual;
```
Or 
```cs
db.SetConnectionStrategy(ConnectionStrategy.Manual);
//
db.SetConnectionStrategy(ConnectionStrategy.Auto);
```

## Interceptor

Allows to intercept commands (executing and executed with result or exception). Usefull for example to create a **Logger Interceptor**

Implements **IDbInterceptor**

```cs
public class MyDbInterceptor : IDbInterceptor
{
    public void OnNonQueryExecuted(DbCommand command, DbInterceptionContext<int> interceptionContext)
    { }

    public void OnNonQueryExecuting(DbCommand command)
    { }

    public void OnScalarExecuted(DbCommand command, DbInterceptionContext<object> interceptionContext)
    { }

    public void OnScalarExecuting(DbCommand command)
    { }

    public void OnSelectAllExecuted<TModel>(DbCommand command, DbInterceptionContext<List<TModel>> interceptionContext)
    { }

    public void OnSelectAllExecuting(DbCommand command)
    { }

    public void OnSelectOneExecuted<TModel>(DbCommand command, DbInterceptionContext<TModel> interceptionContext)
    { }

    public void OnSelectOneExecuting(DbCommand command)
    { }
}
```

... or inherits from DbInterceptorBase and override only methods needed

```cs
public class MyDbInterceptor : DbInterceptor
{
    public override void OnSelectOneExecuted<TModel>(DbCommand command, DbInterceptionContext<TModel> interceptionContext)
    {
        
    }
}
```

## Not handle execution exceptions


```cs
 db.HandleExecutionExceptions = false;
 ```

 Note: argument exceptions are not ignored


## Query Services

# Query Services

EasyDb support by default 3 Providers:
* "_System.Data.SqlClient_" (Sql Server)
* "_System.Data.OleDb_" (Access)
* "_MySql.Data.MySqlClient_" (MySQL with [MySQLConnector for .NET](https://dev.mysql.com/downloads/connector/net/))

For the other providers we have to create a custom Query Service.

## Change/ Add a Query Service

By **default** the Query Service targets **Sql Server** and wrap tables and columns with **quotes**.

```sql
select [permissions].*
from [users_permissions],[permissions]
where [users_permissions].[permission_id]=[permissions].[id] and [users_permissions].[user_id]=@user_id
```

Create a _custom service_ for example.

```cs
public class MyCustomQueryService : QueryService
{
     public MyCustomQueryService()
      : base("`", "`")
      { }

      // override methods ...

}
```

_Change the service_

```cs
EasyDb.Default.SetQueryService("MySql.Data.MySqlClient", new MyCustomQueryService());
```




