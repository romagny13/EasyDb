using System.Collections.Generic;

namespace EasyDbLib.Tests.Core
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }

        // one to many
        public List<Post> PostList { get; set; }

        // many to many
        public List<Permission> PermissionList { get; set; }
    }

    public class UserLikeTable
    {
        public int id { get; set; }
        public string username { get; set; }
        public string lastname { get; set; }
        public int? age { get; set; }
        public string email { get; set; }

        // one to many
        public List<PostLikeTable> PostList { get; set; }

        // many to many
        public List<PermissionLikeTable> PermissionList { get; set; }
    }

    public class PermissionLikeTable
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class PostLikeTable
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }

        public int user_id { get; set; }
        public UserLikeTable User { get; set; }

        public int? category_id { get; set; }
        public CategoryLikeTable Category { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryLikeTable
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
    }

}
