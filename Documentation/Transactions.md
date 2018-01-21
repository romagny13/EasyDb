# Transactions

Add a **reference** to **System.Transactions**

_Example_

```cs
EasyDb.Default.SetConnectionStringSettings(connectionString, providerName);

try
{
    using (var scope = new TransactionScope())
    {

        await EasyDb.Default.InsertInto("categories")
        .Values("name", "My category")
        .NonQueryAsync();


        await EasyDb.Default.InsertInto("categories")
        .Values("name", "My category 2")
        .NonQueryAsync();

        scope.Complete();
    }
}
catch (Exception ex)
{

}
```

.. Or with EasyDb **Error handler**

```cs
EasyDb.Default.OnError += (sender, e) =>
{

};

EasyDb.Default.SetConnectionStringSettings(connectionString, providerName);

using (var scope = new TransactionScope())
{

    await EasyDb.Default.InsertInto("categories")
    .Values("name", "My category")
    .NonQueryAsync();


    await EasyDb.Default.InsertInto("categories")
    .Values("name", "My category 2")
    .NonQueryAsync();

    scope.Complete();
}
```