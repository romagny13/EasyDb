using EasyDbLibTest.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDbLibTest
{

    public class UserLikeTable
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int? age { get; set; }
        public string email { get; set; }

        // one to many
        public List<PostLikeTable> PostList { get; set; }

        // many to many
        public List<PermissionLikeTable> PermissionList { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }

        public List<Post> PostList { get; set; }
    }

    public class P
    {
        public int id { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string email { get; set; }
        public int? age { get; set; }
    }


    public class DbUserWithIgnoredColumns
    {
        public int id { get; set; }
        public string firstname { get; set; }
    }

    public class UserWithIgnoredProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public string MyIgnoredProperty { get; set; }
    }

    public class UserWithIgnoredColumns
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
