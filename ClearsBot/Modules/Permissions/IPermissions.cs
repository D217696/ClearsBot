using Discord;

namespace ClearsBot.Modules
{
    public interface IPermissions
    {
        PermissionLevels GetPermissionForUser(IGuildUser user);
    }
}