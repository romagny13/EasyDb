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


