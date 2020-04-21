using Server.Targeting;

namespace Server.Commands.Generic
{
    public class SingleCommandImplementor : BaseCommandImplementor
    {
        public SingleCommandImplementor()
        {
            Accessors = new string[] { "Single" };
            SupportRequirement = CommandSupport.Single;
            AccessLevel = AccessLevel.Counselor;
            Usage = "Single <command>";
            Description = "Invokes the command on a single targeted object. This is the same as just invoking the command directly.";
        }

        public override void Register(BaseCommand command)
        {
            base.Register(command);

            for (int i = 0; i < command.Commands.Length; ++i)
                CommandSystem.Register(command.Commands[i], command.AccessLevel, Redirect);
        }

        public void Redirect(CommandEventArgs e)
        {
            BaseCommand command = null;

            Commands.TryGetValue(e.Command, out command);

            if (command == null)
                e.Mobile.SendMessage("That is either an invalid command name or one that does not support this modifier.");
            else if (e.Mobile.AccessLevel < command.AccessLevel)
                e.Mobile.SendMessage("You do not have access to that command.");
            else if (command.ValidateArgs(this, e))
                Process(e.Mobile, command, e.Arguments);
        }

        public override void Process(Mobile from, BaseCommand command, string[] args)
        {
            if (command.ValidateArgs(this, new CommandEventArgs(from, command.Commands[0], GenerateArgString(args), args)))
                from.BeginTarget(-1, command.ObjectTypes == ObjectTypes.All, TargetFlags.None, new TargetStateCallback(OnTarget), new object[] { command, args });
        }

        public void OnTarget(Mobile from, object targeted, object state)
        {
            if (!BaseCommand.IsAccessible(from, targeted))
            {
                from.SendLocalizedMessage(500447); // That is not accessible.
                return;
            }

            object[] states = (object[])state;
            BaseCommand command = (BaseCommand)states[0];
            string[] args = (string[])states[1];

            switch (command.ObjectTypes)
            {
                case ObjectTypes.Both:
                    {
                        if (!(targeted is Item) && !(targeted is Mobile))
                        {
                            from.SendMessage("This command does not work on that.");
                            return;
                        }

                        break;
                    }
                case ObjectTypes.Items:
                    {
                        if (!(targeted is Item))
                        {
                            from.SendMessage("This command only works on items.");
                            return;
                        }

                        break;
                    }
                case ObjectTypes.Mobiles:
                    {
                        if (!(targeted is Mobile))
                        {
                            from.SendMessage("This command only works on mobiles.");
                            return;
                        }

                        break;
                    }
            }

            RunCommand(from, targeted, command, args);
        }
    }
}
