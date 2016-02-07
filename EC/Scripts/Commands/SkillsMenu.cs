using System;
using Server.Gumps;
using Server.Targeting;

namespace Server.Commands
{
    public class Skills
    {
        public static void Initialize()
        {
            Register();
        }

        public static void Register()
        {
            CommandSystem.Register("Skills", AccessLevel.Counselor, new CommandEventHandler(Skills_OnCommand));
        }

        [Usage("Skills")]
        [Description("Opens a menu where you can view or edit skills of a targeted mobile.")]
        private static void Skills_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new SkillsTarget();
        }

        private class SkillsTarget : Target
        {
            public SkillsTarget()
                : base(-1, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    from.SendGump(new SkillsGump(from, (Mobile)o));
            }
        }
    }
}