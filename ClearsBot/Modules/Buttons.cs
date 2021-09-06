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
    public class Buttons
    {
        [Button("Completions")]
        public async Task CompletionsButton(SocketMessageComponent Context)
        {
            string[] parameters = Context.Data.CustomId.Split("_");
            ulong guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;
            await Context.Message.DeleteAsync();
            User firstUser = Users.users[ulong.Parse(parameters[1])].FirstOrDefault(x => x.MembershipId == long.Parse(parameters[2]));
            List<User> users = Users.users[ulong.Parse(parameters[1])].Where(x => x.DiscordID == firstUser.DiscordID).ToList();
            await Context.Channel.SendMessageAsync(embed: Misc.GetCompletionsForUser(firstUser, guildId).Build(), component: Misc.GetButtonsForUser(users, ((SocketGuildChannel)Context.Channel).Guild.Id, "completions", firstUser).Build());
        }

        [Button("Register")]
        public async Task RegisterButton(SocketMessageComponent Context)
        {
            string[] parameters = Context.Data.CustomId.Split("_");
            Misc _misc = new Misc(new Bungie());
            await Context.Message.DeleteAsync();
            await _misc.RegisterUser(Context.Channel, ((SocketGuildChannel)Context.Channel).Guild.Id, Context.User.Id, Context.User.Username, parameters[1], parameters[2]);
        }

        [Button("Unregister")]
        public async Task UnregisterButton(SocketMessageComponent Context)
        {
            Users.busy = true;
            string[] parameters = Context.Data.CustomId.Split("_");
            ulong guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;
            IGuildUser user = (IGuildUser)Context.User;

            if (Guilds.GetPermissionForUser(user) < Permissions.PermissionLevels.AdminRole)
            {
                await Context.Channel.SendMessageAsync(user.Mention + " clicked a button they shouldn't have");
                return;
            }

            User userToRemove = Users.users[guildId].Where(x => x.MembershipId.ToString() == parameters[2]).FirstOrDefault();
            Users.users[guildId].Remove(userToRemove);
            await Context.Channel.SendMessageAsync($"{userToRemove.Username} has been unregistered");
            Users.busy = false;
        }

        [Button("reglist")]
        public async Task RegListButton(SocketMessageComponent Context)
        {
            string[] parameters = Context.Data.CustomId.Split("_");
            if (Context.User.Id != ulong.Parse(parameters[3])) return;
            var embed = new EmbedBuilder
            {
                Title = $"Users page ({parameters[2]}/{Math.Ceiling((double)Users.users[ulong.Parse(parameters[1])].Count() / 10)} total users: {Users.users[ulong.Parse(parameters[1])].Count()})"
            };

            foreach (User user in Misc.GetUsersByPage(ulong.Parse(parameters[1]), int.Parse(parameters[2])))
            {
                embed.Description += $"<@!{user.DiscordID}>: {user.Username} \n";
            }

            var componentBuilder = new ComponentBuilder();

            if (int.Parse(parameters[2]) != 1)
            {
                componentBuilder.WithButton(new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithLabel("previous").WithCustomId($"reglist_{parameters[1]}_{int.Parse(parameters[2]) - 1}_{parameters[3]}"));
            }

            if (int.Parse(parameters[2]) != Math.Ceiling((double)Users.users[ulong.Parse(parameters[1])].Count() / 10))
            {
                componentBuilder.WithButton(new ButtonBuilder().WithStyle(ButtonStyle.Danger).WithLabel("next").WithCustomId($"reglist_{parameters[1]}_{int.Parse(parameters[2]) + 1}_{parameters[3]}"));
            }

            await Context.Message.ModifyAsync(x => { x.Embed = embed.Build(); x.Components = componentBuilder.Build(); } );
        }
    }
}
