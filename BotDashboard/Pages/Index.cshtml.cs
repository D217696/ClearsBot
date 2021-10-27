using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotDashboard.Pages
{
    public class IndexModel : PageModel
    {
        public ClearsBot.Modules.Users _users;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, ClearsBot.Modules.Users users)
        {
            _logger = logger;
            _users = users;
        }

        public void OnGet()
        {

        }
    }
}
