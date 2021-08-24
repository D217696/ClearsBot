using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Objects
{
    public class Permissions
    { 
        public enum PermissionLevels : int
        {
            AdminUser = 4,
            AdminRole = 3,
            ModRole = 2,
            User = 1
        }
    }
   
}
