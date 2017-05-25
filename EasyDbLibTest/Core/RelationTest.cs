using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyDbLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyDbLibTest
{
    //[TestClass]
    //public class RelationTest
    //{

    //    private static string sqlServerConnectionString =
    //            @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\SqlServerDb.mdf;Integrated Security=True;Connect Timeout=20"
    //        ;

    //    private static string sqlServerProviderName = "System.Data.SqlClient";

    //    [TestMethod]
    //    public async Task TestRelations_ZeroOne_HasNoValue()
    //    {
    //        EasyDb.Default.SetConnectionStringSettings(sqlServerConnectionString, sqlServerProviderName);


    //        var result = await EasyDb.Default.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
    //            .AddParameter("@id", 4)
    //            .ReadOneAsync<BaseTable>();

    //        // zero one
    //        if (result.ZeroOneFKey.HasValue)
    //        {
    //            var zeroOne = await EasyDb.Default.CreateCommand("Select * from [dbo].[ZeroOne] where Id=@id")
    //                .AddParameter("@id", result.ZeroOneFKey)
    //                .ReadOneAsync<ZeroOne>();

    //            result.ZeroOne = zeroOne;
    //        }

    //        Assert.AreEqual(result.Name.TrimEnd(), "Base 1");
    //        Assert.IsFalse(result.ZeroOneFKey.HasValue);
    //    }

    //    [TestMethod]
    //    public async Task TestRelations_ZeroOne_HasValue()
    //    {
    //        EasyDb.Default.SetConnectionStringSettings(sqlServerConnectionString, sqlServerProviderName);


    //        var result = await EasyDb.Default.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
    //            .AddParameter("@id", 5)
    //            .ReadOneAsync<BaseTable>();

    //        // zero one
    //        if (result.ZeroOneFKey.HasValue)
    //        {
    //            var zeroOne = await EasyDb.Default.CreateCommand("Select * from [dbo].[ZeroOne] where Id=@id")
    //                .AddParameter("@id", result.ZeroOneFKey)
    //                .ReadOneAsync<ZeroOne>();

    //            result.ZeroOne = zeroOne;
    //        }

    //        Assert.IsTrue(result.ZeroOneFKey.HasValue);
    //        Assert.AreEqual(1,result.ZeroOneFKey.Value);
    //        Assert.AreEqual(1, result.ZeroOne.Id);
    //        Assert.AreEqual("Zero 1", result.ZeroOne.Name);
    //    }

    //    [TestMethod]
    //    public async Task TestRelations_OneOne_HasValue()
    //    {
    //        EasyDb.Default.SetConnectionStringSettings(sqlServerConnectionString, sqlServerProviderName);


    //        var result = await EasyDb.Default.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
    //            .AddParameter("@id", 4)
    //            .ReadOneAsync<BaseTable>();

    //        var oneOne = await EasyDb.Default.CreateCommand("Select * from [dbo].[OneOne] where Id=@id")
    //            .AddParameter("@id", result.OneOneFKey)
    //            .ReadOneAsync<OneOne>();

    //        result.OneOne = oneOne;

    //        Assert.AreEqual(result.Name.TrimEnd(), "Base 1");
    //        Assert.IsFalse(result.ZeroOneFKey.HasValue);
    //        Assert.AreEqual(result.OneOneFKey, 2);
    //        Assert.AreEqual(result.OneOne.Id, 2);
    //        Assert.AreEqual(result.OneOne.Name, "One 1");
    //    }

    //    [TestMethod]
    //    public async Task TestRelations_ManyMany()
    //    {
    //        EasyDb.Default.SetConnectionStringSettings(sqlServerConnectionString, sqlServerProviderName);

    //        var result = await EasyDb.Default.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
    //            .AddParameter("@id", 4)
    //            .ReadOneAsync<BaseTable>();

    //        // Many many
    //        var manyManyList = await EasyDb.Default.CreateCommand("select * from [dbo].[ManyMany] where BaseTable_Id=@id")
    //            .AddParameter("@id", 4)
    //            .ReadAllAsync<ManyMany>();

    //        var manyTablesList = new List<ManyTable>();
    //        foreach (var many in manyManyList)
    //        {
    //            var manyTable = await EasyDb.Default.CreateCommand("select * from [dbo].[ManyTable] where Id=@id")
    //                .AddParameter("@id", many.ManyTable_Id)
    //                .ReadOneAsync<ManyTable>();

    //            manyTablesList.Add(manyTable);
    //        }
    //        result.ManyList = manyTablesList;

    //        Assert.AreEqual(1,result.ManyList[0].Id);
    //        Assert.AreEqual("Many 1",result.ManyList[0].Name);
    //        Assert.AreEqual(2,result.ManyList[1].Id, 2);
    //        Assert.AreEqual("Many 2", result.ManyList[1].Name);
    //    }
    //}

   
}
