using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Project.Constants
{
    public class Authorization
    {
        public enum Roles
        {
            Admin, User
        }

        public const string userName = "razib";
        public const string email = "admin@gmail.com";
        public const string password = "Pa$$w0rd";
        public const Roles role = Roles.User;
        public const Roles adminRoles = Roles.Admin;

    }
}
