using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Modules
{
    public class Logger : ILogger
    {
        public ConsoleColor DefaultConsoleColor { get; set; } = ConsoleColor.White;
        public void LogError(string error)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"[Error]: {error}");
            Console.ForegroundColor = DefaultConsoleColor;
        }
        public void LogBungieSuccess(string endPoint, int membershipType, long membershipId, long characterId, string extra = "")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[BungieSuccess] - {endPoint}: membershipType: {membershipType} membershipId: {membershipId} characterId: {characterId} {extra}");
            Console.ForegroundColor = DefaultConsoleColor;
        }
        public void LogBungieError(string endpoint, int errorCode, string errorMessage, int membershipType, long membershipId, long characterId, string extra1 = "", string extra2 = "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[BungieError] - {endpoint}: code: {errorCode} message: {errorMessage} membershipType: {membershipType} membershipId: {membershipId} characterid: {characterId} {extra1} {extra2}");
            Console.ForegroundColor = DefaultConsoleColor;
        }
    }
}
