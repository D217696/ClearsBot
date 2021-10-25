using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public interface ISlashes
    {
        Task RegisterSlashCommandsForGuild(ulong guildId);
    }
}