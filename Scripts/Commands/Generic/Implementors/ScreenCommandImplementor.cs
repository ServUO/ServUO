using System;

namespace Server.Commands.Generic
{
    public class ScreenCommandImplementor : BaseCommandImplementor
    {
        public ScreenCommandImplementor()
        {
            this.Accessors = new string[] { "Screen" };
            this.SupportRequirement = CommandSupport.Area;
            this.SupportsConditionals = true;
            this.AccessLevel = AccessLevel.GameMaster;
            this.Usage = "Screen <command> [condition]";
            this.Description = "Invokes the command on all appropriate objects in your screen. Optional condition arguments can further restrict the set of objects.";
        }

        public override void Process(Mobile from, BaseCommand command, string[] args)
        {
            RangeCommandImplementor impl = RangeCommandImplementor.Instance;

            if (impl == null)
                return;

            impl.Process(18, from, command, args);
        }
    }
}