using ClearsBot.Objects;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public class SlashCommands
    {
        [SlashCommand("test")]
        public async Task SlashCommandTest(SocketSlashCommandData data)
        {
            Console.WriteLine("it worked!");
        }
    }
}
