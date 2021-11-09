using ClearsBot.Objects;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearsBot.Modules
{
    public class UpdateLoop
    {
        readonly Roles _roles;
        readonly Users _users;
        readonly MessageTracking _messageTracking;
        readonly IGuilds _guilds;
        readonly IRaids _raids;
        readonly DiscordSocketClient _client;
        public UpdateLoop(Roles roles, Users users, MessageTracking messageTracking, IGuilds guilds, DiscordSocketClient client, IRaids raids)
        {
            _roles = roles;
            _users = users;
            _guilds = guilds;
            _messageTracking = messageTracking;
            _client = client;
            _raids = raids;

            new Thread(new ThreadStart(UpdateUsers)).Start();
        }

        async void UpdateUsers()
        {
            while (true)
            {
                if (_client.ConnectionState != Discord.ConnectionState.Connected) continue;
                if (DateTime.Now.Minute % 30 == 0) _ = _users.UpdateUsersAsync();
                if (DateTime.Now.Minute % 30 == 0) _ = _roles.UpdateRolesForGuildsAsync();
                if (DateTime.Now.Minute % 57 == 0) _raids.SyncRaids();
                if (DateTime.Now.Minute % 58 == 0) _guilds.SyncGuilds();
                if (DateTime.Now.Minute % 59 == 0)
                {
                    foreach (InternalGuild guild in _guilds.GetGuilds().Values)
                    {
                        _ = _users.SyncUsers(guild.GuildId);
                    }
                }
                _messageTracking.CheckTrackedMessages();

                Thread.Sleep(1000 * 60);
            }
        }
    }
}
