namespace Server.Commands.Generic
{
    public class RangeCommandImplementor : BaseCommandImplementor
    {
        private static RangeCommandImplementor m_Instance;
        public RangeCommandImplementor()
        {
            Accessors = new string[] { "Range" };
            SupportRequirement = CommandSupport.Area;
            SupportsConditionals = true;
            AccessLevel = AccessLevel.GameMaster;
            Usage = "Range <range> <command> [condition]";
            Description = "Invokes the command on all appropriate objects within a specified range of you. Optional condition arguments can further restrict the set of objects.";

            m_Instance = this;
        }

        public static RangeCommandImplementor Instance => m_Instance;
        public override void Execute(CommandEventArgs e)
        {
            if (e.Length >= 2)
            {
                int range = e.GetInt32(0);

                if (range < 0)
                {
                    e.Mobile.SendMessage("The range must not be negative.");
                }
                else
                {
                    BaseCommand command = null;
                    Commands.TryGetValue(e.GetString(1), out command);

                    if (command == null)
                    {
                        e.Mobile.SendMessage("That is either an invalid command name or one that does not support this modifier.");
                    }
                    else if (e.Mobile.AccessLevel < command.AccessLevel)
                    {
                        e.Mobile.SendMessage("You do not have access to that command.");
                    }
                    else
                    {
                        string[] oldArgs = e.Arguments;
                        string[] args = new string[oldArgs.Length - 2];

                        for (int i = 0; i < args.Length; ++i)
                            args[i] = oldArgs[i + 2];

                        Process(range, e.Mobile, command, args);
                    }
                }
            }
            else
            {
                e.Mobile.SendMessage("You must supply a range and a command name.");
            }
        }

        public void Process(int range, Mobile from, BaseCommand command, string[] args)
        {
            AreaCommandImplementor impl = AreaCommandImplementor.Instance;

            if (impl == null)
                return;

            Map map = from.Map;

            if (map == null || map == Map.Internal)
                return;

            Point3D start = new Point3D(from.X - range, from.Y - range, from.Z);
            Point3D end = new Point3D(from.X + range, from.Y + range, from.Z);

            impl.OnTarget(from, map, start, end, new object[] { command, args });
        }
    }
}