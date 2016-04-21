// Look Command v1.2.0
// Author: Felladrin
// Created at 2010-02-12
// Updated at 2016-02-06

using System.Text.RegularExpressions;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Felladrin.Commands
{
    public static class Look
    {
        public static class Config
        {
            public static bool Enabled = true;                       // Is this command enabled?
            public static bool DisplayHealthStatus = false;           // Should we also display the health status along the player profile?
            public static bool OpenGumpOnPaperdollRequest = true;    // Should we open the Look Gump when player double-clicks a mobile to see their paperdoll?
        }

        public static void Initialize()
        {
            if (Config.Enabled)
            {
                CommandSystem.Register("Look", AccessLevel.Player, new CommandEventHandler(OnCommand));

                if (Config.OpenGumpOnPaperdollRequest)
                    EventSink.PaperdollRequest += OnPaperdollRequest;
            }
        }

        [Usage("Look")]
        [Description("Looks at someone and opens their profile description. Target yourself to edit your own profile.")]
        static void OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Who would you like to look at?");
            e.Mobile.Target = new LookTarget();
        }

        static void OnPaperdollRequest(PaperdollRequestEventArgs e)
        {
            if (e.Beholder.Equals(e.Beheld))
                return;
            
            e.Beholder.CloseGump(typeof(LookGump));
            e.Beholder.SendGump(new LookGump(e.Beheld));
        }

        public class LookTarget : Target
        {
            public LookTarget() : base(-1, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                var target = targeted as Mobile;
                if (target != null)
                {
                    if (from.Equals(target))
                    {
                        target.DisplayPaperdollTo(from);
                        from.Send(new DisplayProfile(!from.ProfileLocked, from, "Description of " + from.RawName, from.Profile, "Use the space above to describe your character."));
                    }
                    else
                    {
                        if(target.CanPaperdollBeOpenedBy(from))
                        {
                            //target.SendMessage("You notice that {0} is looking at you.", from.Name);
                            target.DisplayPaperdollTo(from);
                        }
                        from.CloseGump(typeof(LookGump));
                        from.SendGump(new LookGump(target));
                    }
                }
                else
                {
                    from.SendMessage("There's nothing special about it, it isn't worth looking...");
                }
            }
        }

        public class LookGump : Gump
        {
            const int Width = 300;
            const int Height = 200;

            public LookGump(Mobile target) : base(100, 100)
            {
                AddPage(0);
                AddBackground(0, 0, Width, Height, 0xDAC);
                AddPage(1);
                AddHtml(0, 10, Width, 25, "<CENTER>" + "Observing " + target.Name, false, false);
                AddHtml(20, 30, Width - 40, Height - 50, Profile(target) + HealthStatus(target), true, true);
            }

            static string NewLinesToHtmlBr(string str)
            {
                return Regex.Replace(str, @"\r\n?|\n", "<br/>");
            }

            static string Profile(Mobile m)
            {
                if (string.IsNullOrEmpty(m.Profile))
                    return null;

                return NewLinesToHtmlBr(m.Profile) + "<br/><br/>";
            }

            static string HealthStatus(Mobile m)
            {
                if (!Config.DisplayHealthStatus)
                    return null;

                string healthStatus = "";

                healthStatus += (m.Body.IsFemale || m.Body.IsMale) ? (m.Female ? "She" : "He") : "It";

                if (m.Hits == m.HitsMax)
                    healthStatus += " seems perfectly healthy.";
                else if (m.Hits > m.HitsMax * 0.8)
                    healthStatus += " looks slightly injured.";
                else if (m.Hits > m.HitsMax * 0.6)
                    healthStatus += " looks injured.";
                else if (m.Hits > m.HitsMax * 0.4)
                    healthStatus += " looks badly injured.";
                else if (m.Hits > m.HitsMax * 0.2)
                    healthStatus += " is deadly injured.";
                else
                    healthStatus += " is almost dead.";

                return healthStatus;
            }
        }
    }
}
