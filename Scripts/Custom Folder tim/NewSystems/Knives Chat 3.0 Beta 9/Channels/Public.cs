using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class Public : Channel
    {
        public Public() : base("Public")
        {
            Commands.Add("chat");
            Commands.Add("c");
            NewChars = true;

            Register(this);
        }
    }
}