using Server.Items;
using Server.Targeting;
using System;
using System.Collections;

namespace Server.Commands.Generic
{
    public class ContainedCommandImplementor : BaseCommandImplementor
    {
        public ContainedCommandImplementor()
        {
            Accessors = new string[] { "Contained" };
            SupportRequirement = CommandSupport.Contained;
            AccessLevel = AccessLevel.GameMaster;
            Usage = "Contained <command> [condition]";
            Description = "Invokes the command on all child items in a targeted container. Optional condition arguments can further restrict the set of objects.";
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
                from.SendMessage("That is not accessible.");
                return;
            }

            object[] states = (object[])state;
            BaseCommand command = (BaseCommand)states[0];
            string[] args = (string[])states[1];

            if (command.ObjectTypes == ObjectTypes.Mobiles)
                return; // sanity check

            if (!(targeted is Container))
            {
                from.SendMessage("That is not a container.");
            }
            else
            {
                try
                {
                    Extensions ext = Extensions.Parse(from, ref args);

                    bool items, mobiles;

                    if (!CheckObjectTypes(from, command, ext, out items, out mobiles))
                        return;

                    if (!items)
                    {
                        from.SendMessage("This command only works on items.");
                        return;
                    }

                    Container cont = (Container)targeted;

                    Item[] found = cont.FindItemsByType(typeof(Item), true);

                    ArrayList list = new ArrayList();

                    for (int i = 0; i < found.Length; ++i)
                    {
                        if (ext.IsValid(found[i]))
                            list.Add(found[i]);
                    }

                    ext.Filter(list);

                    RunCommand(from, list, command, args);
                }
                catch (Exception e)
                {
                    from.SendMessage(e.Message);
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }
        }
    }
}
