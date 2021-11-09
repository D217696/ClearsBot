using Discord;
using ClearsBot.Objects;

namespace ClearsBot.Modules
{
    public interface IPermissions
    {
        PermissionLevel GetPermissionForUser(IGuildUser user);
    }
}