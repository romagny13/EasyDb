using System.Data.Common;

namespace EasyDbLibTest
{
    public class InitDb
    {
        public static string SqlConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\EasyDbLib\EasyDbLibTest\DBTests.mdf;Integrated Security=True;Connect Timeout=30";
        public static string SqlProviderName = "System.Data.SqlClient";

        public static void CreateDbTest()
        {

            using (var connection = DbProviderFactories.GetFactory(SqlProviderName).CreateConnection())
            {

                connection.ConnectionString = SqlConnectionString;
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DROP TABLE IF EXISTS dbo.[posts];DROP TABLE IF EXISTS dbo.[categories];DROP TABLE IF EXISTS dbo.[users];;DROP TABLE IF EXISTS dbo.[UsersWithGuid];
                                  CREATE TABLE [dbo].[users] (
                                        [id]    INT         IDENTITY (1, 1) NOT NULL,
                                        [firstname] NCHAR (255) NOT NULL,
                                        [lastname]  NCHAR (255) NOT NULL,
                                        [age]   INT         NULL,
                                        [email] NCHAR (100) NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );
                                   CREATE TABLE [dbo].[categories] (
                                        [id]   INT         IDENTITY (1, 1) NOT NULL,
                                        [name] NCHAR (255) NOT NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC)
                                    );
                                    CREATE TABLE [dbo].[posts] (
                                        [id]          INT                   IDENTITY (1, 1) NOT NULL,
                                        [title]       NCHAR (255)           NOT NULL,
                                        [content]     NVARCHAR (MAX)        NOT NULL,
                                        [user_id]     INT                   NOT NULL,
                                        [category_id] INT                   NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC),
                                        FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([Id]),
                                        FOREIGN KEY ([category_id]) REFERENCES [dbo].[categories] ([id])
                                    );
                                    CREATE TABLE [dbo].[UsersWithGuid] (
                                        [Id]    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
                                        [Name]  NCHAR (10)       NOT NULL,
                                        [Email] NCHAR (20)       NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );

                                    insert into [categories] (name) values('Web');
                                    insert into [categories] (name) values('Mobile');

                                    insert into [users] (firstname,lastname,email) values('Marie','Bellin','marie@domain.com');
                                    insert into [users] (firstname,lastname,age) values('Pat','Prem',30);

                                    insert into [posts] (title,content,user_id,category_id) values('Post 1','Content 1',1,1);
                                    insert into [posts] (title,content,user_id) values('Post 2','Content 2',2);
                                    insert into [posts] (title,content,user_id,category_id) values('Post 3','Content 3',2,2);
                                    insert into [posts] (title,content,user_id) values('Post 4','Content 4',2);
                                    insert into [posts] (title,content,user_id,category_id) values('Post 5','Content 5',1,2);
                                    insert into [posts] (title,content,user_id,category_id) values('Post 6','Content 6',1,2);
                            ";

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}
