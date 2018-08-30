using System;
using System.Collections.Generic;
using System.Text;

namespace Server.ACC
{
    public partial class ACC
    {
        /*GlobalMinimumAccessLevel:
         * This AccessLevel is the minimum required AccessLevel to manipulate
         * the ACC Core system in-game.  This includes:
         * ACC commands
         * ACC Core gumps
         * ACC System gumps accessed through the Core gumps
         */
        public static AccessLevel GlobalMinimumAccessLevel = AccessLevel.Administrator;
    }
}
