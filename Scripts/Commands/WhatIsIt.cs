using Server.Targeting;

namespace Server.Commands
{
    public class WhatIsIt
    {

        public static void Initialize()
        {
            CommandSystem.Register("WhatIsIt", AccessLevel.Player, GenericCommand_OnCommand);
        }

        public class WhatIsItTarget : Target
        {

            public WhatIsItTarget()
                : base(30, true, TargetFlags.None)
            {
                CheckLOS = false;
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || targeted == null) return;

                string name = string.Empty;
                string typename = targeted.GetType().Name;
                string article = "a";

                if (!string.IsNullOrEmpty(typename))
                {
                    if ("aeiouy".IndexOf(typename.ToLower()[0]) >= 0)
                    {
                        article = "an";
                    }
                }

                if (targeted is Item)
                {
                    name = ((Item)targeted).Name;
                }
                else
                    if (targeted is Mobile)
                {
                    name = ((Mobile)targeted).Name;
                }
                if (name != string.Empty && name != null)
                {
                    from.SendMessage("That is {0} {1} named '{2}'", article, typename, name);
                }
                else
                {
                    from.SendMessage("That is {0} {1} with no name", article, typename);
                }
            }
        }

        [Usage("WhatIsIt")]
        public static void GenericCommand_OnCommand(CommandEventArgs e)
        {
            if (e == null || e.Mobile == null) return;

            e.Mobile.Target = new WhatIsItTarget();
        }
    }
}
