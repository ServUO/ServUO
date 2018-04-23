// Unique Character Names v1.1.2
// Author: Felladrin
// Started: 2013-08-01
// Updated: 2016-07-28

using System;
using Server;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;

namespace Felladrin.Automations
{
    public static class UniqueCharacterNames
    {
        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        static void OnLogin(LoginEventArgs args)
        {
            Mobile from = args.Mobile;

            if (HasValidName(from))
                return;

            from.SendMessage(33, "Your character name '{0}' is already in use by player character or it has been marked as invalid. Please choose a new one.", from.Name);
            from.RawName = "Generic Player";
            from.SendGump(new NameChangeGump());
        }

        static bool HasValidName(Mobile m)
        {
            if (m.AccessLevel != AccessLevel.Player)
                return true;

            if (m.RawName == null || m.RawName.Trim() == String.Empty || m.RawName == "Generic Player" || !NameVerification.Validate(m.RawName, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
                return false;

            foreach (Mobile otherPlayer in World.Mobiles.Values)
                if (otherPlayer is PlayerMobile && otherPlayer != m && otherPlayer.RawName != null && m.RawName != null && otherPlayer.RawName.ToLower() == m.RawName.ToLower() && m.CreationTime > otherPlayer.CreationTime)
                    return false;

            return true;
        }

        public class NameChangeGump : Gump
        {
            void AddBlackAlpha(int x, int y, int width, int height)
            {
                AddImageTiled(x, y, width, height, 2624);
                AddAlphaRegion(x, y, width, height);
            }

            void AddTextField(int x, int y, int width, int height, int index)
            {
                AddBackground(x - 2, y - 2, width + 4, height + 4, 0x2486);
                AddTextEntry(x + 2, y + 2, width - 4, height - 4, 0, index, "");
            }

            static string Center(string text)
            {
                return String.Format("<CENTER>{0}</CENTER>", text);
            }

            static string Color(string text, int color)
            {
                return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
            }

            void AddButtonLabeled(int x, int y, int buttonID, string text)
            {
                AddButton(x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
                AddHtml(x + 35, y, 240, 20, Color(text, 0xFFFFFF), false, false);
            }

            public NameChangeGump() : base(50, 50)
            {
                Closable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddBlackAlpha(10, 120, 250, 85);
                AddHtml(10, 125, 250, 20, Color(Center("Your name is already in use or invalid!"), 0xFFFFFF), false, false);
                AddLabel(73, 15, 1152, "");
                AddLabel(20, 150, 0x480, "New Name:");
                AddTextField(100, 150, 150, 20, 0);
                AddButtonLabeled(175, 180, 1, "Submit");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID != 1)
                    return;

                var m = sender.Mobile;
                var nameEntry = info.GetTextEntry(0);

                m.RawName = nameEntry != null ? nameEntry.Text.Trim() : "Generic Player";

                if (HasValidName(m))
                {
                    m.SendMessage(66, "Your name has been changed! You are now known as '{0}'.", m.RawName);
                }
                else
                {
                    m.SendMessage(33, "You can't use that name. Please choose a new one.");
                    m.RawName = "Generic Player";
                    m.CloseGump(typeof(NameChangeGump));
                    m.SendGump(new NameChangeGump());
                }
            }
        }
    }
}