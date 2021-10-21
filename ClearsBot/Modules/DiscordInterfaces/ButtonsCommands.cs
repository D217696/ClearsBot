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
        readonly IUtilities _utilities;
        readonly Users _users;
        readonly IPermissions _permissions;
        readonly IRaids _raids;
        readonly Commands _commands;
        readonly Buttons _buttons;
        readonly Completions _completions;
        readonly IFormatting _formatting;

        public ButtonsCommands(IUtilities utilities, Users users, IPermissions permissions, IRaids raids, Commands commands, Buttons buttons, Completions completions, IFormatting formatting)
        {
            _utilities = utilities;
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

            await Context.Channel.SendMessageAsync(embed: _formatting.GetCompletionsEmbed(user, completions).Build(), component: _buttons.GetButtonsForUser(users.ToList(), "completions", buttonData.DiscordUserId, buttonData.DiscordServerId, buttonData.DiscordChannelId, null).Build());
        }
        //[Button("Completions")]
        //public async Task CompletionsButton(SocketMessageComponent Context)
        //{
        //    string[] parameters = Context.Data.CustomId.Split("_");
        //    ulong guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;
        //    await Context.Message.DeleteAsync(); 
        //    User firstUser = _users.GetGuildUsers(ulong.Parse(parameters[1])).FirstOrDefault(x => x.MembershipId == long.Parse(parameters[2])); 
        //    List<User> users = _users.GetGuildUsers(ulong.Parse(parameters[1])).Where(x => x.DiscordID == firstUser.DiscordID).ToList();
        //    await Context.Channel.SendMessageAsync(embed: _utilities.GetCompletionsForUser(firstUser, guildId).Build(), component: _buttons.GetButtonsForUser(users.Where(x => x.MembershipId != long.Parse(parameters[2])).ToList(), "completions", users.FirstOrDefault().DiscordID, ((SocketGuildChannel)Context.Channel).Guild.Id, Context.Channel.Id, null).Build());
        //}

        //[Button("Fastest")]
        //public async Task FastestButton(SocketMessageComponent Context)
        //{
        //    string[] parameters = Context.Data.CustomId.Split("_");
        //    ulong guildId = ((SocketGuildChannel)Context.Channel).Guild.Id;
        //    await Context.Message.DeleteAsync();
        //    User firstUser = _users.GetGuildUsers(ulong.Parse(parameters[1])).FirstOrDefault(x => x.MembershipId == long.Parse(parameters[2]));
        //    List<User> users = _users.GetGuildUsers(ulong.Parse(parameters[1])).Where(x => x.DiscordID == firstUser.DiscordID).ToList();
        //    Raid raid = null;
        //    string raidString = "";
        //    if (parameters.Length >= 4)
        //    {
        //        raidString = parameters[3];
        //    }
        //    if(raidString != "")
        //    {
        //        raid = _raids.GetRaids(guildId).FirstOrDefault(x => x.Shortcuts.Contains(raidString) || x.DisplayName.ToLower().Contains(raidString));
        //    }
        //    await Context.Channel.SendMessageAsync(embed: _utilities.GetCompletionsForUser(firstUser, guildId).Build(), component: _buttons.GetButtonsForUser(users.Where(x => x.MembershipId != long.Parse(parameters[2])).ToList(), "fastest", users.FirstOrDefault().DiscordID, ((SocketGuildChannel)Context.Channel).Guild.Id, Context.Channel.Id, raid).Build());
        //}

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
