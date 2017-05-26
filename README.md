# EasyDb (version 3.x)

> EasyDb is a Library for **.NET** Projects.

* **Micro ORM**
* **Simple commands** with **SQL**
* **Queries** with **Relations** Zero One, One to many and Many to Many
* **Mapping**
* **No dependencies**. _Do not require to update the Model._
* Code **Testable**
* **Repository** _Pattern recommended_

## Installation

```
PM> Install-Package EasyDb
```

## Documentation

* [Gitbook](https://www.gitbook.com/book/romagny13/easydb/)

## Examples

### Simple Command With SQL

```cs
var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\mydb.mdf;Integrated Security=True;Connect Timeout=30";
var providerName = "System.Data.SqlClient";

EasyDb.Default.OnError += (sender, e) =>
            {
                
            };

EasyDb.Default.SetConnectionStringSettings(connectionString, providerName);

var result =  await EasyDb.Default.CreateCommand("select * from [users] where id=@id")
                                  .AddParameter("@id",1)
                                  .ReadOneAsync<User>();
```

### Query

```cs
var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\path\to\mydb.mdf;Integrated Security=True;Connect Timeout=30";
var providerName = "System.Data.SqlClient";

EasyDb.Default.SetConnectionStringSettings(connectionString, providerName);

Mapping.SetTable("users");
Mapping.SetTable("posts")
       .SetForeignKeyColumn("user_id","user_d","users","id");


var result = await EasyDb.Default
                .Select<Post>(Mapping.GetTable("posts"))
                .SetZeroOne<User>("User", Mapping.GetTable("users"))
                .Where(Check.Op("id", 1))
                .ReadOneAsync();

```

