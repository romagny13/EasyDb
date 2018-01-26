using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyDbLib.Tests.Factory
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class UserWithUserId
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("Users")]
    public class UserWithAttributes
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
        public string Age { get; set; }

        public Role Role { get; set; }
    }
}
