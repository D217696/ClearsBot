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
    public class ButtonsCommands
    {
        readonly Users _users;
        readonly IPermissions _permissions;
        readonly IRaids _raids;
        readonly Commands _commands;
        readonly Buttons _buttons;
        readonly Completions _completions;
        readonly IFormatting _formatting;

        public ButtonsCommands(Users users, IPermissions permissions, IRaids raids, Commands commands, Buttons buttons, Completions completions, IFormatting formatting)
        {
            _users = users;
            _permissions = permissions;
            _raids = raids;
            _commands = commands;
            _buttons = buttons;
            _completions = completions;
            _formatting = formatting;
        }

        [Button("Completions")]
        public async Task CompletionsButton(SocketMessageComponent Context)
        {
            ButtonData buttonData = _buttons.GetButtonData(Context.Data.CustomId);
            await Context.Message.DeleteAsync();
            User user = _users.GetUserByMembershipId(buttonData.MembershipId);
            IEnumerable<User> users = _users.GetUsersByDiscordId(buttonData.DiscordUserId);
            var completions = _completions.GetRaidCompletionsForUser(user, buttonData.DiscordServerId);

            await Context.Channel.SendMessageAsync(embed: _formatting.GetCompletionsEmbed(user, completions).Build(), components: _buttons.GetButtonsForUser(users.Where(x => x.MembershipId != user.MembershipId).ToList(), "completions", buttonData.DiscordUserId, buttonData.DiscordServerId, buttonData.DiscordChannelId, null).Build());
        }

        [Button("Fastest")]
        public async Task FastestButton(SocketMessageComponent Context)
        {
            ButtonData buttonData = _buttons.GetButtonData(Context.Data.CustomId);
            await Context.Message.DeleteAsync();
            User user = _users.GetUserByMembershipId(buttonData.MembershipId);
            IEnumerable<User> users = _users.GetUsersByDiscordId(buttonData.DiscordUserId);
            Raid raid = null;
            string raidName = "raid";
            if (buttonData.Raid != null)
            {
                raid = buttonData.Raid;
                raidName = buttonData.Raid.DisplayName;
            }
            await Context.Channel.SendMessageAsync(embed: _commands.FastestCommand(user, buttonData.DiscordServerId, raidName).Build(), components: _buttons.GetButtonsForUser(users.Where(x => x.MembershipId != user.MembershipId).ToList(), "fastest", buttonData.DiscordUserId, buttonData.DiscordServerId, buttonData.DiscordChannelId, raid).Build());
        }

        [Button("Register")]
        public async Task RegisterButton(SocketMessageComponent Context)
        {
            ButtonData buttonData = _buttons.GetButtonData(Context.Data.CustomId);
            await Context.Message.DeleteAsync();
            await _commands.RegisterUserCommand(Context.Channel, buttonData.DiscordServerId, buttonData.DiscordUserId, "", buttonData.MembershipId.ToString(), buttonData.MembershipType.ToString());
        }

        //[Button("Register")]
        //public async Task RegisterButton(SocketMessageComponent Context)
        //{
        //    string[] parameters = Context.Data.CustomId.Split("_");
        //    await Context.Message.DeleteAsync();
        //    await _commands.RegisterUserCommand(Context.Channel, ((SocketGuildChannel)Context.Channel).Guild.Id, Context.User.Id, Context.User.Username, parameters[1], parameters[2]);
        //}

        //[Button("Unregister")]
        //public async Task UnregisterButton(SocketMessageComponent Context)
        //{
        //    string[] parameters = Context.Data.CustomId.Split("_");
        //    ulong guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;
        //    IGuildUser user = (IGuildUser)Context.User;

        //    if (_permissions.GetPermissionForUser(user) < PermissionLevels.AdminRole)
        //    {
        //        await Context.Channel.SendMessageAsync(user.Mention + " clicked a button they shouldn't have");
        //        return;
        //    }

        //    User userToRemove = _users.GetGuildUsers(guildId).Where(x => x.MembershipId.ToString() == parameters[2]).FirstOrDefault();
        //    _users.GetGuildUsers(guildId).Remove(userToRemove);
        //    await Context.Channel.SendMessageAsync($"{userToRemove.Username} has been unregistered");
        //}

        //[Button("reglist")]
        //public async Task RegListButton(SocketMessageComponent Context)
        //{
        //    string[] parameters = Context.Data.CustomId.Split("_");
        //    if (Context.User.Id != ulong.Parse(parameters[3])) return;
        //    var embed = new EmbedBuilder
        //    {
        //        Title = $"Users page ({parameters[2]}/{Math.Ceiling((double)_users.GetGuildUsers(ulong.Parse(parameters[1])).Count() / 10)} total users: {_users.GetGuildUsers(ulong.Parse(parameters[1])).Count()})"
        //    };

        //    foreach (User user in _users.GetUsersByPage(ulong.Parse(parameters[1]), int.Parse(parameters[2])))
        //    {
        //        embed.Description += $"<@!{user.DiscordID}>: {user.Username} \n";
        //    }

        //    var componentBuilder = new ComponentBuilder();

        //    if (int.Parse(parameters[2]) != 1)
        //    {
        //        componentBuilder.WithButton(new ButtonBuilder().WithStyle(ButtonStyle.Primary).WithLabel("previous").WithCustomId($"reglist_{parameters[1]}_{int.Parse(parameters[2]) - 1}_{parameters[3]}"));
        //    }

        //    if (int.Parse(parameters[2]) != Math.Ceiling((double)_users.GetGuildUsers(ulong.Parse(parameters[1])).Count() / 10))
        //    {
        //        componentBuilder.WithButton(new ButtonBuilder().WithStyle(ButtonStyle.Danger).WithLabel("next").WithCustomId($"reglist_{parameters[1]}_{int.Parse(parameters[2]) + 1}_{parameters[3]}"));
        //    }

        //    await Context.Message.ModifyAsync(x => { x.Embed = embed.Build(); x.Components = componentBuilder.Build(); } );
        //}
    }
}
