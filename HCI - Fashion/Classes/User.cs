using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI___Fashion.Classes
{
    public enum UserRole {Visitor, Admin}
    [Serializable]
    public class User
    {
        string username, password;
        UserRole role;

        public User() { }

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public UserRole Role { get => role; set => role = value; }
    }
}
