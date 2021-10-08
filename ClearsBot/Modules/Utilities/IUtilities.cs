using ClearsBot.Objects;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClearsBot.Modules
{
    public interface IUtilities
    {
        Func<User, int> Criteria(Raid raid = null, DateTime? startDate = null, DateTime? endDate = null);
        Func<Completion, bool> DefaultCriteria();
        Func<Completion, bool> EndDateCriteria(DateTime? endDate);
        ComponentBuilder GetButtonsForUser(List<User> users, ulong guildId, string command, User firstUser = null, string extra = "");
        ButtonStyle GetButtonStyleForPlatform(int membershipType);
        int GetCompletionCountForUser(User user, Raid raid = null, DateTime? startDate = null, DateTime? endDate = null);
        int GetCompletionCountForUser(User user, ulong guildId);
        EmbedBuilder GetCompletionsForUser(User user, ulong guildId);
        Func<Completion, bool> GetCriteriaForRaid(Raid raid);
        string GetErrorCodeForUserSearch(RequestData requestData);
        EmbedBuilder GetFastestListForUser(User user, Raid raid, ulong guildID);
        DateTime GetWeeklyResetTime(DateTime startDate);
        Func<Completion, bool> RaidCriteria(Raid raid);
        Func<Completion, bool> StartDateCriteria(DateTime? startDate);
    }
}