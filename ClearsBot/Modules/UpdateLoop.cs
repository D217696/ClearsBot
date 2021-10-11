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
        public UpdateLoop(Roles roles, Users users)
        {
            _roles = roles;
            _users = users;

            new Thread(new ThreadStart(UpdateUsers)).Start();
        }

        async void UpdateUsers()
        {
            while (true)
            {
                if (DateTime.Now.Minute % 5 == 0) _users.AddUsersToUpdateUsersList();
                if (DateTime.Now.Minute % 30 == 0) _ = _users.UpdateUsersAsync();
                if (DateTime.Now.Minute % 30 == 0) _ = _roles.UpdateRolesForGuildsAsync();

                Thread.Sleep(1000 * 60);
            }
        }
    }
}
