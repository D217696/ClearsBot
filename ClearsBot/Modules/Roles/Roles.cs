using ClearsBot.Modules;
using ClearsBot.Objects;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class Roles
    {
        readonly Users _users;
        readonly IGuilds _guilds;
        readonly IRaids _raids;
        readonly IBungie _bungie;
        readonly Completions _completions;
        public Roles(Users users, IGuilds guilds, IRaids raids, IBungie bungie, Completions completions)
        {
            _users = users;
            _guilds = guilds;
            _raids = raids;
            _bungie = bungie;
            _completions = completions;
        }

        public async Task GiveRoleToUser(IGuildUser user, IRole role, IReadOnlyCollection<IGuildUser> users)
        {
            if (role == null) return;
            bool userHasRole = false;
            foreach (IGuildUser roleUser in users.Where(x => x.RoleIds.Contains(role.Id)))
            {
                if (roleUser == user && user != null)
                {
                    userHasRole = true;
                    continue;
                }
                await roleUser.RemoveRoleAsync(role);
                Console.WriteLine($"Removed {role.Name} from {roleUser.Username}");
            }

            if (user == null) return;
            if (!userHasRole)
            {
                await user.AddRoleAsync(role);
                Console.WriteLine($"Added {role.Name} to {user.Username}");
            }
        }
        public async Task UpdateRolesForGuildsAsync()
        {
            foreach (InternalGuild guild in _guilds.GetGuilds().Values)
            {
                SocketGuild currentGuild = _guilds.GetGuildFromClient(guild.GuildId);
                await currentGuild.DownloadUsersAsync();
                foreach (Raid raid in _raids.GetRaids(guild.GuildId))
                {
                    List<(User user, int completions, int rank)> users = _completions.GetCompletionsForUsers(_users.GetGuildUsers(guild.GuildId), _bungie.ReleaseDate, DateTime.UtcNow, new[] { raid }).ToList();

                    await GiveRoleToUser(currentGuild.GetUser(users[0].user.DiscordID), currentGuild.GetRole(raid.FirstRole), currentGuild.Users);
                    await GiveRoleToUser(currentGuild.GetUser(users[1].user.DiscordID), currentGuild.GetRole(raid.SecondRole), currentGuild.Users);
                    await GiveRoleToUser(currentGuild.GetUser(users[2].user.DiscordID), currentGuild.GetRole(raid.ThirdRole), currentGuild.Users);
                }

                List<(User user, int completions, int rank)> usersTotal = _completions.GetCompletionsForUsers(_users.GetGuildUsers(guild.GuildId), _bungie.ReleaseDate, DateTime.UtcNow, _raids.GetRaids(guild.GuildId)).ToList();
                await GiveRoleToUser(currentGuild.GetUser(usersTotal[0].user.DiscordID), currentGuild.GetRole(guild.FirstRole), currentGuild.Users);
                await GiveRoleToUser(currentGuild.GetUser(usersTotal[1].user.DiscordID), currentGuild.GetRole(guild.SecondRole), currentGuild.Users);
                await GiveRoleToUser(currentGuild.GetUser(usersTotal[2].user.DiscordID), currentGuild.GetRole(guild.ThirdRole), currentGuild.Users);

                foreach (Milestone milestone in guild.Milestones)
                {
                    IEnumerable<(User user, int completions, int rank)> milestoneUsers = _completions.GetCompletionsForUsers(_users.GetGuildUsers(guild.GuildId), _bungie.ReleaseDate, DateTime.UtcNow, new[] { milestone.Raid });
                    foreach ((User user, int completions, int rank) user in milestoneUsers.Where(x => x.completions >= milestone.Completions))
                    {
                        await GiveRoleToUser(currentGuild.GetUser(user.user.DiscordID), currentGuild.GetRole(milestone.Role), currentGuild.Users);
                    }
                }
            }
        }
    }
}
