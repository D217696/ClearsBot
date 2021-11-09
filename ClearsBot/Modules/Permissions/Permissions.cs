using ClearsBot.Objects;
using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Modules
{
    public class Permissions : IPermissions
    {
        readonly IGuilds _guilds;
        readonly Config _config;
        public Permissions(IGuilds guilds, Config config)
        {
            _guilds = guilds;
            _config = config;
        }

        public PermissionLevel GetPermissionForUser(IGuildUser user)
        {
            if (IsBotOwner(user.Id)) return PermissionLevel.BotOwner;
            if (IsBotAdmin(user.Id)) return PermissionLevel.BotAdmin;
            if (IsGuildOwner(user.GuildId, user.Id)) return PermissionLevel.GuildOwner;
            if (IsGuildAdmin(user.GuildId, user.Id)) return PermissionLevel.GuildAdmin;
            return PermissionLevel.User;
        }

        private bool IsBotOwner(ulong userId)
        {
            return _config.bot.Owner == userId;
        }
        
        private bool IsBotAdmin(ulong userId)
        {
            return _config.bot.BotAdmins.Contains(userId);
        }

        private bool IsGuildOwner(ulong guildId, ulong userId)
        {
            return _guilds.GetGuild(guildId).GuildOwner == userId;
        }
        
        private bool IsGuildAdmin(ulong guildId, ulong userId)
        {
            return _guilds.GetGuild(guildId).AdminUsers.Contains(userId);
        }
    }
}