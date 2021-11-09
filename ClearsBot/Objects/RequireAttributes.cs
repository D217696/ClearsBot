using ClearsBot.Modules;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Objects
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequirePermissionAttribute : PreconditionAttribute
    {
        private IPermissions _permissions;
        private readonly PermissionLevel _permissionLevel;
        public RequirePermissionAttribute(PermissionLevel permissionLevel)
        {
            _permissionLevel = permissionLevel;
            _permissions = Globals._permissions;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_permissions == null) _permissions = Globals._permissions;

            if (_permissions.GetPermissionForUser((IGuildUser) context.User) >= _permissionLevel) return PreconditionResult.FromSuccess();
            return PreconditionResult.FromError("No permission");
        }
    }
}
