using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDbLibTest
{
    public class P
    {
        public int id { get; set; }
        public string first { get; set; }
        public string last { get; set; }
        public string email { get; set; }
        public int? age { get; set; }
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
