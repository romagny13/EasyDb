# Mapping

> Its recommended to define the mapping at the application bootstrap

**Mapping columns** is only **required** :
* **with relations**
* or **if** the **column name** is **not equal** to the **property name** _(example: "id" and "Id", "user_id" and "UserId")_

**Create** the Mapping for a **table**

```cs
Mapping
       .SetTable("posts") // table name
       .SetPrimaryKeyColumn("id", "Id") // primary key
       .SetColumn("title", "Title") // column name => property name
       .SetForeignKeyColumn("user_id", "UserId", "users", "id"); // foreign key, property, table, primary key
```

**Ignore a column** (do not fill the property)

```cs
Mapping
      .SetTable("posts")
      .SetPrimaryKeyColumn("id", "Id",true) // ignored
      .SetColumn("title", "Title")
      .SetForeignKeyColumn("user_id", "UserId", "users", "id");
```

Set the **DbType**

```cs
Mapping
       .SetTable("posts")
       .SetPrimaryKeyColumn("id", "Id", DbType.Int16)
       .SetColumn("title", "Title")
       .SetForeignKeyColumn("user_id", "UserId", "users", "id");
```

Create an **intermediate table** (for **Many to Many relations**)

```cs
Mapping.SetIntermediateTable("users_permissions") // table name
      .SetPrimaryKeyColumn("user_id", "id", "users") // primary key, target table pk, target table name
      .SetPrimaryKeyColumn("permission_id", "id", "permissions");
````

