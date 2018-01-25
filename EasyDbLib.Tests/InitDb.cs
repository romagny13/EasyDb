using System.Data.Common;

namespace EasyDbLib.Tests
{
    public class DbConstants
    {
        public const string SqlDb1
            = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\source\repos\v4\2\EasyDb\EasyDbLib.Tests\DbTest.mdf;Integrated Security=True;Connect Timeout=30";
        public const string SqlDbLikeMySql
            = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\source\repos\v4\2\EasyDb\EasyDbLib.Tests\DbTest2.mdf;Integrated Security=True;Connect Timeout=30";

        public const string SqlProviderName = "System.Data.SqlClient";
        public const string OleDbConnectionString =
              @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\romag\source\repos\v4\2\EasyDb\EasyDbLib.Tests\NorthWind.mdb";
        public const string OleDbProviderName = "System.Data.OleDb";
    }

    public class InitDb
    {
        public static void CreateDbLikeProperties()
        {

            using (var connection = DbProviderFactories.GetFactory(DbConstants.SqlProviderName).CreateConnection())
            {

                connection.ConnectionString = DbConstants.SqlDb1;
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DROP TABLE IF EXISTS dbo.[User_Permission];
                                            DROP TABLE IF EXISTS dbo.[Post];
                                            DROP TABLE IF EXISTS dbo.[Permission];
                                            DROP TABLE IF EXISTS dbo.[Category];
                                            DROP TABLE IF EXISTS dbo.[User];
                                            DROP TABLE IF EXISTS dbo.[UserGuid];
                                 CREATE TABLE [dbo].[User] (
                                        [Id]        INT         IDENTITY (1, 1) NOT NULL,
                                        [UserName] NCHAR (255) NOT NULL,
                                        [Age]       INT         NULL,
                                        [Email]     NCHAR (100) NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );
                                   CREATE TABLE [dbo].[Category] (
                                        [Id]   INT         IDENTITY (1, 1) NOT NULL,
                                        [Name] NCHAR (255) NOT NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );
                                    CREATE TABLE [dbo].[Permission] (
                                        [Id]          INT            IDENTITY (1, 1) NOT NULL,
                                        [Name]        NCHAR (255)    NOT NULL,
                                        [Description] NVARCHAR (MAX) NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC)
                                    );
                                    CREATE TABLE [dbo].[User_Permission] (
                                        [UserId]       INT NOT NULL,
                                        [PermissionId] INT NOT NULL,
                                        PRIMARY KEY CLUSTERED ([UserId] ASC, [PermissionId] ASC),
                                        FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
                                        FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission] ([Id])
                                    );
                                    CREATE TABLE [dbo].[Post] (
                                        [Id]          INT                   IDENTITY (1, 1) NOT NULL,
                                        [Title]       NCHAR (255)           NOT NULL,
                                        [Content]     NVARCHAR (MAX)        NOT NULL,
                                        [UserId]     INT                   NOT NULL,
                                        [CategoryId] INT                   NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC),
                                        FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
                                        FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id])
                                    );
                                    CREATE TABLE [dbo].[UserGuid] (
                                        [Id]    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
                                        [Name]  NCHAR (10)       NOT NULL,
                                        [Email] NCHAR (20)       NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );

                                    insert into [Permission] (name,description) values('read Post','description read Post');
                                    insert into [Permission] (name,description) values('create Post','description create Post');
                                    insert into [Permission] (name,description) values('edit Post','description edit Post');
                                    insert into [Permission] (name,description) values('delete Post','description delete Post');

                                    insert into [Category] (name) values('Web');
                                    insert into [Category] (name) values('Mobile');

                                    insert into [User] (UserName,Email) values('Marie','marie@domain.com');
                                    insert into [User] (UserName,Age) values('Pat',30);
                                    insert into [User] (UserName,Email) values('Deb','deb@domain.com');
                                    insert into [User] (UserName,Age) values('Ken',25);

                                    insert into [Post] (Title,Content,UserId,CategoryId) values('Post 1','Content 1',1,1);
                                    insert into [Post] (Title,Content,UserId) values('Post 2','Content 2',2);
                                    insert into [Post] (Title,Content,UserId,CategoryId) values('Post 3','Content 3',2,2);
                                    insert into [Post] (Title,Content,UserId) values('Post 4','Content 4',2);
                                    insert into [Post] (Title,Content,UserId,CategoryId) values('Post 5','Content 5',1,2);
                                    insert into [Post] (Title,Content,UserId,CategoryId) values('Post 6','Content 6',1,2);
                           
                                    insert into [User_Permission] (UserId,PermissionId) values(1,1);
                                    insert into [User_Permission] (UserId,PermissionId) values(1,2);
                                    insert into [User_Permission] (UserId,PermissionId) values(1,3);
                                    insert into [User_Permission] (UserId,PermissionId) values(1,4);
                                    insert into [User_Permission] (UserId,PermissionId) values(2,1);
                                    ";

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public static void CreateDbTestLikeMySql()
        {

            using (var connection = DbProviderFactories.GetFactory(DbConstants.SqlProviderName).CreateConnection())
            {

                connection.ConnectionString = DbConstants.SqlDbLikeMySql;
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = @"DROP TABLE IF EXISTS dbo.[users_permissions];DROP TABLE IF EXISTS dbo.[posts];DROP TABLE IF EXISTS dbo.[permissions];DROP TABLE IF EXISTS dbo.[categories];DROP TABLE IF EXISTS dbo.[users];;DROP TABLE IF EXISTS dbo.[UsersWithGuid];
                                 CREATE TABLE [dbo].[users] (
                                        [id]        INT         IDENTITY (1, 1) NOT NULL,
                                        [username] NCHAR (255) NOT NULL,
                                        [age]       INT         NULL,
                                        [email]     NCHAR (100) NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC)
                                    );
                                   CREATE TABLE [dbo].[categories] (
                                        [id]   INT         IDENTITY (1, 1) NOT NULL,
                                        [name] NCHAR (255) NOT NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC)
                                    );
                                    CREATE TABLE [dbo].[permissions] (
                                        [id]          INT            IDENTITY (1, 1) NOT NULL,
                                        [name]        NCHAR (255)    NOT NULL,
                                        [description] NVARCHAR (MAX) NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC)
                                    );
                                    CREATE TABLE [dbo].[users_permissions] (
                                        [user_id]       INT NOT NULL,
                                        [permission_id] INT NOT NULL,
                                        PRIMARY KEY CLUSTERED ([user_id] ASC, [permission_id] ASC),
                                        FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([id]),
                                        FOREIGN KEY ([permission_id]) REFERENCES [dbo].[permissions] ([id])
                                    );
                                    CREATE TABLE [dbo].[posts] (
                                        [id]          INT                   IDENTITY (1, 1) NOT NULL,
                                        [title]       NCHAR (255)           NOT NULL,
                                        [content]     NVARCHAR (MAX)        NOT NULL,
                                        [user_id]     INT                   NOT NULL,
                                        [category_id] INT                   NULL,
                                        PRIMARY KEY CLUSTERED ([id] ASC),
                                        FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([Id]),
                                        FOREIGN KEY ([category_id]) REFERENCES [dbo].[categories] ([id])
                                    );
                                    CREATE TABLE [dbo].[UsersWithGuid] (
                                        [Id]    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
                                        [Name]  NCHAR (10)       NOT NULL,
                                        [Email] NCHAR (20)       NULL,
                                        PRIMARY KEY CLUSTERED ([Id] ASC)
                                    );

                                    insert into [permissions] (name,description) values('read posts','description read posts');
                                    insert into [permissions] (name,description) values('create posts','description create posts');
                                    insert into [permissions] (name,description) values('edit posts','description edit posts');
                                    insert into [permissions] (name,description) values('delete posts','description delete posts');

                                    insert into [categories] (name) values('Web');
                                    insert into [categories] (name) values('Mobile');

                                    insert into [users] (username,email) values('Marie','marie@domain.com');
                                    insert into [users] (username,age) values('Pat',30);
                                    insert into [users] (username,email) values('Deb','deb@domain.com');
                                    insert into [users] (username,age) values('Ken',25);

                                    insert into [posts] (title,content,user_id,category_id) values('Post 1','Content 1',1,1);
                                    insert into [posts] (title,content,user_id) values('Post 2','Content 2',2);
                                    insert into [posts] (title,content,user_id,category_id) values('Post 3','Content 3',2,2);
                                    insert into [posts] (title,content,user_id) values('Post 4','Content 4',2);
                                    insert into [posts] (title,content,user_id,category_id) values('Post 5','Content 5',1,2);
                                    insert into [posts] (title,content,user_id,category_id) values('Post 6','Content 6',1,2);
                           
                                    insert into [users_permissions] (user_id,permission_id) values(1,1);
                                    insert into [users_permissions] (user_id,permission_id) values(1,2);
                                    insert into [users_permissions] (user_id,permission_id) values(1,3);
                                    insert into [users_permissions] (user_id,permission_id) values(1,4);
                                    insert into [users_permissions] (user_id,permission_id) values(2,1);
                                    ";

                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
    }
}
