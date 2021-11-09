using System;
using System.Collections.Generic;
using System.Text;

namespace ClearsBot.Objects
{
    public sealed class ButtonAttribute : Attribute
    {
        public ButtonAttribute(string buttonName)
        {

        }
    }

    public sealed class SlashCommandAttribute : Attribute
    {
        public SlashCommandAttribute()
        {

        }
        public SlashCommandAttribute(string commandName)
        {

        }
    }
}
