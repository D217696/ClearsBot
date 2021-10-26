using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Modules
{
    public class MessageTracking
    {
        private List<(RestUserMessage message, DateTime timeAdded, TimeSpan timeToTrack)> TrackedMessages { get; set; } = new List<(RestUserMessage message, DateTime timeAdded, TimeSpan timeToTrack)>();

        public void AddMessageToTrack(RestUserMessage message, TimeSpan timeToTrack)
        {
            TrackedMessages.Add((message, DateTime.UtcNow, timeToTrack));
        }

        public async void CheckTrackedMessages()
        {

            foreach ((RestUserMessage message, DateTime timeAdded, TimeSpan timeToTrack) message in new List<(RestUserMessage message, DateTime timeAdded, TimeSpan timeToTrack)>(TrackedMessages))
            {
                if (message.timeAdded + message.timeToTrack <= DateTime.UtcNow)
                {
                    try
                    {
                        await message.message.ModifyAsync(x => x.Components = null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("could not remove components");
                    }
                    TrackedMessages.Remove(message);
                }
            }
        }
    }
}
