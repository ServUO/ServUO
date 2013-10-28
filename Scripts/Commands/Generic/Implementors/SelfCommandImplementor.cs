using System;

namespace Server.Commands.Generic
{
    public class SelfCommandImplementor : BaseCommandImplementor
    {
        public SelfCommandImplementor()
        {
            this.Accessors = new string[] { "Self" };
            this.SupportRequirement = CommandSupport.Self;
            this.AccessLevel = AccessLevel.Counselor;
            this.Usage = "Self <command>";
            this.Description = "Invokes the command on the commanding player.";
        }

        public override void Compile(Mobile from, BaseCommand command, ref string[] args, ref object obj)
        {
            if (command.ObjectTypes == ObjectTypes.Items)
                return; // sanity check

            obj = from;
        }
    }
}