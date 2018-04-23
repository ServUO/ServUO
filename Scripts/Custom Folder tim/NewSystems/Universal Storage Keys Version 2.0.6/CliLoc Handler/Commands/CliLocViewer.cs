using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Solaris.CliLocHandler;
using Server.Gumps;

namespace Server.Commands
{
    public class CliLocViewer
    {
        public static string _FilterText;

        public static void Initialize()
        {
            CommandSystem.Register("CliLocViewer",AccessLevel.Developer,new CommandEventHandler(OnCommand));
        }

        [Usage("CliLocViewer <filter string>")]
        [Description("Opens the Client Localization string viewer.  Optionally filters list with specified string.")]
        public static void OnCommand(CommandEventArgs e)
        {
            if (e.Length >= 1)      //if there's some arguments
            {
                e.Mobile.SendGump(new CliLocViewerGump(e.Mobile,e.Arguments[0]));
            }
            else
            {
                e.Mobile.SendGump(new CliLocViewerGump(e.Mobile));
            }
        }
    }
}