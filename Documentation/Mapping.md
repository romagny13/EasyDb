# Mapping

> Its recommended to define the mapping at the application startup

* Explicit 
* by **Reflection** with **Data Annotations Attributes**


Registering Tables

By default **IgnoreCase** allows to resolve properties and not have to define all properties. 
For example, if a column is named "username" and the property is "UserName", with IgnoreCase the property will be found.

```cs
var db = new EasyDb();

db.SetTable<User>("User")
      .SetPrimaryKeyColumn("Id", p => p.Id);
```

Define properties explicitly

```cs
var db = new EasyDb();

db.SetTable<User>("User")
      .setColumn("Name", p => p.UserName);
```

### Database Generated columns

By default primary key with mapping is supposed auto incremented. Change this:

```cs
var db = new EasyDb();

db.SetTable<User>("User")
      .SetPrimaryKeyColumn("Id", p => p.Id, false);
```

For a column. Example:

```cs
var db = new EasyDb();

db.SetTable<User>("User")
      .SetColumn("RowVersion", p => p.RowVersion, true);
```

### Ignore columns

With Primary key column

```cs
var db = new EasyDb();

db.SetTable<User>("User")
      .SetPrimaryKeyColumn("Id", p => p.Id, false, true); // last paramater
```

With column

```cs
var db = new EasyDb();

db.SetTable<User>("User")
      .setColumn("UserName", p => p.UserName, false, true); // last paramater
```

## Default Mapping Behavior

EasyDb could discover Mapping for tables not registered explicitly:

* By **Reflection** (default)
* **Create empty table** with primary key "Id" or "id" (for MySql) are supposed column name
* None: do not resolve

Change default behavior

```cs
var db = new EasyDb();
db.DefaultMappingBehavior = DefaultMappingBehavior.CreateEmptyTable;
```

## Mapping with Data Annotation Attributes

Attributes:
* **Table**: allows to change the table name
* **Column**: allows to change the column name
* **Key** : for primary key
* **DatabaseGenerated**: for auto increment or columns
* **NotMapped**: ignore column

Example:

```cs
[Table("Users")]
public class User
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      public string UserName { get; set; }

      [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
      public DateTime RowVersion { get; set; }

      [Column("role_id")]
      public int RoleId { get; set; }

      [NotMapped]
      public int? Age { get; set; }

      public Role Role { get; set; }
}
```

