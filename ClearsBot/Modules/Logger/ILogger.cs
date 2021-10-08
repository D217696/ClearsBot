using System;

namespace ClearsBot
{
    public interface ILogger
    {
        ConsoleColor DefaultConsoleColor { get; set; }
        void LogError(string error);
        void LogBungieSuccess(string endPoint, int membershipType, long membershipId, long characterId, string extra = "");
        void LogBungieError(string endPoint, int errorCode, string errorMessage, int membershipType, long membershipId, long characterId, string extra1 = "", string extra2 = "");
    }
}