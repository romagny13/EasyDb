# Select Query and relations

Methods:
* **Statements**: "distinct" for example
* **Top** : allow to take only a number of results
* **Where**: with condition
* **OrderBy**

_Example_

```cs
EasyDb.Default.SetConnectionStringSettings(connectionString, providerName);

Mapping.SetTable("posts");

var posts = await EasyDb.Default
                .Select<Post>(Mapping.GetTable("posts"))
                .Top(10)
                .Where(Check.Op("user_id", 1))
                .OrderBy("created_at desc", "title")
                .ReadAllAsync();
```

## Conditions (Check)

* Op (operator) 

```cs
Check.Op("id", 1) // =
Check.Op("age", ">", 18)
```

* Like

```cs
Check.Like("name", "%a")
```

* Between

```cs
Check.Between("age",18,30)
```

* IsNull

```cs
Check.IsNull("age")
```

* IsNotNull

```cs
Check.IsNotNull("age")
```

* And

```cs
Check.Op("user_id",1).And(Check.Op("category_id", 2))
```

* Or

```cs
Check.Op("user_id",1).Or(Check.Op("user_id", 2))
```

... multiple conditions

```cs
Check.Op("user_id", 1).Or(Check.Op("user_id", 2)).Or(Check.Op("user_id",3))
```

## Get the Query String

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);


Mapping.SetTable("users");

var result = db
              .Select<User>(Mapping.GetTable("users"))
              .GetQuery();
              // select * from [users]
```

With **Mapping**

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

Mapping
      .SetTable("posts")
      .SetColumn("title","Title")
      .SetColumn("content","Content")
      .SetForeignKeyColumn("user_id", "UserId", "users", "id");

var result = db.Select<Post>(Mapping.GetTable("posts"))
               .GetQuery();
// select [title],[content],[user_id] from [posts]
```

## ReadOneAsync

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);


Mapping.SetTable("users")
       .SetPrimaryKeyColumn("id", "Id")
       .SetColumn("firstname", "FirstName")
       .SetColumn("lastname", "LastName")
       .SetColumn("age", "Age")
       .SetColumn("email", "Email");

var result = await db
                .Select<User>(Mapping.GetTable("users"))
                .Where(Check.Op("id", 1))
                .ReadOneAsync();
```

## ReadAllAsync

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

Mapping.SetTable("users");

var result = await db
                .Select<UserLikeTable>(Mapping.GetTable("users"))
                .ReadAllAsync();
```

## Zero One relation

Fill an _object property_ for a _foreign key_

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

// require the foreign key
Mapping.SetTable("posts")
.SetForeignKeyColumn("category_id", "category_id", "categories", "id");

Mapping.SetTable("categories");

// => fill the Category property in post model
var result = await db
                .Select<Post>(Mapping.GetTable("posts"))
                .Where(Check.Op("id", 2))
                .SetZeroOne<Category>("Category", Mapping.GetTable("categories")) // property to fill, relation mapping
               .ReadOneAsync();
```

_With multiple relations ..._

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);


Mapping.SetTable("posts")
       .SetForeignKeyColumn("user_id", "user_id", "users", "id")
       .SetForeignKeyColumn("category_id","category_id","categories","id");
Mapping.SetTable("categories").SetForeignKeyColumn("category_id", "category_id", "categories", "id");
Mapping.SetTable("users");


var result = await db
                .Select<Post>(Mapping.GetTable("posts"))
                .Where(Check.Op("id", 1))
                .SetZeroOne<User>("User", Mapping.GetTable("users"))
                .SetZeroOne<Category>("Category", Mapping.GetTable("categories"))
                .ReadOneAsync();
```

## One to Many relation

Returns a _list_

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

// require the foreign key
Mapping.SetTable("posts")
       .SetForeignKeyColumn("user_id", "user_id", "users", "id"); // column, property, table, primary key

// require the primary key
Mapping.SetTable("users")
       .SetPrimaryKeyColumn("id", "id");

var result = await db
                .Select<User>(Mapping.GetTable("users"))
                .Where(Check.Op("id", 1))
                .SetOneToMany<Post>("PostList", Mapping.GetTable("posts")) // property to fill, relation table mapping
                .ReadOneAsync();
```

## Many to Many relation

Returns a _list_

_Example: get the permissions for a user_

```cs
var db = new EasyDb();
db.SetConnectionStringSettings(connectionString, providerName);

// required the primary key
Mapping.SetTable("users")
       .SetPrimaryKeyColumn("id", "id");

Mapping.SetTable("permissions");

// define the intermediate table mapping ("users_permissions")
Mapping.SetIntermediateTable("users_permissions")
       .SetPrimaryKeyColumn("user_id", "id", "users") // primary key (users_permissions), primary key (users), table (users)
       .SetPrimaryKeyColumn("permission_id", "id", "permissions"); // primary key (users_permissions), primary key (permissions), table (permissions)

var result = await db
                .Select<User>(Mapping.GetTable("users"))
                .Where(Check.Op("id", 1))
                // property to fill, relation mapping, intermediate table mapping
                .SetManyToMany<Permission>("PermissionList", Mapping.GetTable("permissions"), Mapping.GetIntermediateTable("users_permissions"))
                .ReadOneAsync();
```

