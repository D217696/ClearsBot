using ClearsBot.Objects;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Modules
{
    public class Buttons
    {

        private Dictionary<Guid, ButtonData> ActiveButtons = new Dictionary<Guid, ButtonData>();
        public Buttons()
        {

        }
        public ButtonData GetButtonData(Guid interactionId)
        {
            if (ActiveButtons.ContainsKey(interactionId)) return ActiveButtons[interactionId];

            return null;
        }

        public ButtonData GetButtonData(string interactionId)
        {
            Guid guid = Guid.Parse(interactionId);
            if (ActiveButtons.ContainsKey(guid)) return ActiveButtons[guid];

            return null;
        }

        public void AddButton(ButtonData buttonData)
        {
            if (ActiveButtons.ContainsKey(buttonData.InteractionID)) ActiveButtons.Add(buttonData.InteractionID, buttonData);
        }

        public ButtonData CreateButtonData(string commandName, ulong discordUserId, ulong discordServerId, ulong discordChannelId, long membershipId, int membershipType, Raid raid)
        {
            return new ButtonData()
            {
                InteractionID = Guid.NewGuid(),
                CommandName = commandName,
                DiscordUserId = discordUserId,
                DiscordServerId = discordServerId,
                DiscordChannelId = discordChannelId,
                MembershipId = membershipId,
                MembershipType = membershipType,
                Raid = raid,
                Handled = false
            };
        }
        public ComponentBuilder GetButtonsForUser(List<User> users, string commandName, ulong discordUserId, ulong discordServerId, ulong discordChannelId, Raid raid)
        {
            var componentBuilder = new ComponentBuilder();
            int buttons = 0;
            int buttonRow = 0;
            foreach (User user in users)
            {
                ButtonData buttonData = CreateButtonData(commandName, discordUserId, discordServerId, discordChannelId, user.MembershipId, user.MembershipType, raid);
                AddButton(buttonData);
                componentBuilder.WithButton(new ButtonBuilder().WithLabel(user.Username).WithCustomId(buttonData.InteractionID.ToString()).WithStyle(GetButtonStyleForPlatform(user.MembershipType)));
                buttons++;
                if (buttons % 5 == 0) buttonRow++;
            }

            return componentBuilder;
        }

        public ButtonStyle GetButtonStyleForPlatform(int membershipType)
        {
            return membershipType switch
            {
                1 => ButtonStyle.Success,
                2 => ButtonStyle.Primary,
                3 => ButtonStyle.Danger,
                _ => ButtonStyle.Secondary
            };
        }
    }
}
