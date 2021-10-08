﻿using ClearsBot.Objects;
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
        public Permissions(IGuilds guilds)
        {
            _guilds = guilds;
        }

        public PermissionLevels GetPermissionForUser(IGuildUser user)
        {
            List<ulong> roles = JsonConvert.DeserializeObject<List<ulong>>(JsonConvert.SerializeObject(user.RoleIds));
            Guild guild = _guilds.GuildsList[user.Guild.Id];
            if (user.Id == Config.bot.owner) return PermissionLevels.BotOwner;
            if (user.Id == guild.AdminUser) return PermissionLevels.AdminUser;
            if (roles.Contains(guild.AdminRole)) return PermissionLevels.AdminRole;
            foreach (ulong roleId in guild.ModRoles)
            {
                if (roles.Contains(roleId)) return PermissionLevels.ModRole;
            }
            return PermissionLevels.User;
        }
    }
}
