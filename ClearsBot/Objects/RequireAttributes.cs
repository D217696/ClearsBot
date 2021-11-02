using ClearsBot.Modules;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequirePermissionAttribute : PreconditionAttribute
    {
        readonly IPermissions _permissions;
        readonly PermissionLevels _permissionLevels;
        public RequirePermissionAttribute(PermissionLevels permissionsLevels, IPermissions permissions)
        {
            _permissions = permissions;
            _permissionLevels = permissionsLevels;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_permissions.GetPermissionForUser((IGuildUser) context.User) >= _permissionLevels) return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError("No permission");
        }
    }
}
