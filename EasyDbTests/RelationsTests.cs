using EasyDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDbTests
{
    [TestClass]
    public class RelationsTests
    {
        [TestMethod]
        public void Relations_Zero_One_And_One_One()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var result = db.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
                .AddParameter("@id", 4)
                .ReadOneMapTo<BaseTable>();

            // zero one
            if (result.ZeroOneId.HasValue)
            {
                var zeroOne = db.CreateCommand("Select * from [dbo].[ZeroOne] where Id=@id")
                  .AddParameter("@id", result.ZeroOneId)
                  .ReadOneMapTo<ZeroOne>();

                result.ZeroOne = zeroOne;
            }

            // one one
            var oneOne = db.CreateCommand("Select * from [dbo].[OneOne] where Id=@id")
                .AddParameter("@id", result.OneOneId)
                .ReadOneMapTo<OneOne>();

            result.OneOne = oneOne;

            Assert.AreEqual(result.Name.TrimEnd(), "Base 1");
            Assert.IsFalse(result.ZeroOneId.HasValue);
            Assert.AreEqual(result.OneOneId,2);
            Assert.AreEqual(result.OneOne.Id, 2);
            Assert.AreEqual(result.OneOne.Name, "One 1");
        }

        [TestMethod]
        public async Task Relations_Zero_One_And_One_One_Async()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var result = await db.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
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

            Assert.AreEqual(result.Name.TrimEnd(), "Base 2");
            Assert.IsTrue(result.ZeroOneId.HasValue && result.ZeroOneId == 1);
            Assert.AreEqual(result.ZeroOne.Id, 1);
            Assert.AreEqual(result.ZeroOne.Name, "Zero 1");
            Assert.AreEqual(result.OneOneId, 3);
            Assert.AreEqual(result.OneOne.Id, 3);
            Assert.AreEqual(result.OneOne.Name, "One 2");
        }

        [TestMethod]
        public void Relations_Many_Many()
        {
            var db = new Db();
            db.Initialize(TestConstants.SqlClientConnectionString, "System.Data.SqlClient");

            var result = db.CreateCommand("select * from [dbo].[BaseTable] where id=@id")
                .AddParameter("@id", 4)
                .ReadOneMapTo<BaseTable>();

            // Many many
            var manyManyList = db.CreateCommand("select * from [dbo].[ManyMany] where BaseTable_Id=@id")
                .AddParameter("@id", 4)
                .ReadAllMapTo<ManyMany>();

            var manyTablesList = new List<ManyTable>();
            foreach (var many in manyManyList)
            {
                var manyTable = db.CreateCommand("select * from [dbo].[ManyTable] where Id=@id")
                    .AddParameter("@id", many.ManyTable_Id)
                    .ReadOneMapTo<ManyTable>();

                manyTablesList.Add(manyTable);
            }
            result.ManyList = manyTablesList;

            Assert.AreEqual(result.ManyList[0].Id, 1);
            Assert.AreEqual(result.ManyList[0].Name, "Many 1");
            Assert.AreEqual(result.ManyList[1].Id, 2);
            Assert.AreEqual(result.ManyList[1].Name, "Many 2");
        }

    }

    public class BaseTable
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Map(ColumnName = "ZeroOneFKey")]
        public int? ZeroOneId { get; set; }
        public ZeroOne ZeroOne { get; set; }

        [Map(ColumnName ="OneOneFKey")]
        public int OneOneId { get; set; }
        public OneOne OneOne { get; set; }

        public List<ManyTable> ManyList { get; set; }
    }

    public class ZeroOne
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OneOne
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ManyMany
    {
        public int BaseTable_Id { get; set; }
        public int ManyTable_Id { get; set; }
    }
    public class ManyTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
