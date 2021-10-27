using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearsBot.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BotDashboard.Pages.Users
{
    public class IndexModel : PageModel
    {
        public ClearsBot.Modules.Users _users;
        public IndexModel(ClearsBot.Modules.Users users)
        {
            _users = users;

        }
        public void OnGet()
        {
        }

    }
}
