using ClearsBot.Objects;
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
        public UpdateLoop(Roles roles, Users users, MessageTracking messageTracking, IGuilds guilds)
        {
            _roles = roles;
            _users = users;
            _guilds = guilds;
            _messageTracking = messageTracking;

            new Thread(new ThreadStart(UpdateUsers)).Start();
        }

        async void UpdateUsers()
        {
            while (true)
            {
                if (Program._client.ConnectionState != Discord.ConnectionState.Connected) continue;
                if (DateTime.Now.Minute % 5 == 0) _users.AddUsersToUpdateUsersList();
                if (DateTime.Now.Minute % 30 == 0) _ = _users.UpdateUsersAsync();
                if (DateTime.Now.Minute % 30 == 0) _ = _roles.UpdateRolesForGuildsAsync();
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
