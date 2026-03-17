using System;
using Server.Commands;
using Server.Engines.ClassSystem;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    /// <summary>
    /// Central file for all GM / staff commands specific to the DNA-ARENA shard.
    ///
    /// Location: Scripts/DNA-ARENA/GMCommands.cs
    ///
    /// How to add a new command
    /// ========================
    /// 1. Write a private static handler method: static void OnMyCommand(CommandEventArgs e)
    /// 2. Register it in Initialize():
    ///    CommandSystem.Register("mycommand", AccessLevel.GameMaster, new CommandEventHandler(OnMyCommand));
    /// 3. Done – ServUO picks up Initialize() automatically at startup.
    ///
    /// Command list
    /// ============
    ///   [setclass  – assigns a class to the targeted player
    /// </summary>
    public static class GMCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("setclass", AccessLevel.GameMaster,
                new CommandEventHandler(OnSetClass));
        }

        // ── [setclass ────────────────────────────────────────────────────────────
        //
        //   Usage : [setclass <className>
        //   Example: [setclass Avenger
        //
        //   Assigns the specified class to the player you click on.
        //   Pass "none" to remove the class entirely.

        private static void OnSetClass(CommandEventArgs e)
        {
            Mobile gm = e.Mobile;

            if (e.Length != 1)
            {
                gm.SendMessage(0x35, "Usage: [setclass <className>");
                gm.SendMessage(0x35, "Available classes:");

                foreach (IPlayerClass cls in ClassRegistry.All)
                    gm.SendMessage(0x35, $"  {cls.ClassType} – {cls.DisplayName}");

                gm.SendMessage(0x35, "  None – removes the class");
                return;
            }

            string arg = e.GetString(0).Trim();

            // Parse the class type from the argument string.
            ClassType targetType;

            if (arg.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                targetType = ClassType.None;
            }
            else if (!Enum.TryParse(arg, true, out targetType)
                     || (targetType != ClassType.None && ClassRegistry.Get(targetType) == null))
            {
                gm.SendMessage(0x22, $"Unknown class: '{arg}'. Use [setclass without arguments to see the list.");
                return;
            }

            // Ask the GM to click on a target.
            gm.SendMessage("Click on the player you want to assign the class to.");
            gm.Target = new SetClassTarget(targetType);
        }

        private sealed class SetClassTarget : Target
        {
            private readonly ClassType _classType;

            public SetClassTarget(ClassType classType)
                : base(12, false, TargetFlags.None)
            {
                _classType = classType;
            }

            protected override void OnTarget(Mobile gm, object targeted)
            {
                if (!(targeted is PlayerMobile target))
                {
                    gm.SendMessage(0x22, "You must target a player.");
                    return;
                }

                ClassType previous = target.ClassComponent.ClassType;

                target.ClassComponent.Assign(_classType);

                if (_classType == ClassType.None)
                {
                    gm.SendMessage(68,
                        $"Removed class from {target.Name} (was: {previous}).");
                    target.SendMessage(68,
                        "A GM has removed your class.");
                }
                else
                {
                    IPlayerClass cls = ClassRegistry.Get(_classType);
                    gm.SendMessage(68,
                        $"Assigned class '{cls.DisplayName}' to {target.Name} (was: {previous}).");
                    target.SendMessage(68,
                        $"A GM has assigned you the class: {cls.DisplayName}.");
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.SendMessage("Class assignment cancelled.");
            }
        }
    }
}
