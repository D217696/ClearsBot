using System;
using System.Collections.Generic;
using System.Text;
using ClearsBot.Modules;

namespace ClearsBot
{
    public class Globals
    {
        public static IPermissions _permissions;
        public Globals(IPermissions permissions)
        {
            _permissions = permissions;
        }
    }
}
