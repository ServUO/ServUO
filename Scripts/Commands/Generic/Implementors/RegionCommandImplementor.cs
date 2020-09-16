using System;
using System.Collections;

namespace Server.Commands.Generic
{
    public class RegionCommandImplementor : BaseCommandImplementor
    {
        public RegionCommandImplementor()
        {
            Accessors = new string[] { "Region" };
            SupportRequirement = CommandSupport.Region;
            SupportsConditionals = true;
            AccessLevel = AccessLevel.GameMaster;
            Usage = "Region <command> [condition]";
            Description = "Invokes the command on all appropriate mobiles in your current region. Optional condition arguments can further restrict the set of objects.";
        }

        public override void Compile(Mobile from, BaseCommand command, ref string[] args, ref object obj)
        {
            try
            {
                Extensions ext = Extensions.Parse(from, ref args);

                bool items, mobiles;

                if (!CheckObjectTypes(from, command, ext, out items, out mobiles))
                    return;

                Region reg = from.Region;

                ArrayList list = new ArrayList();

                if (mobiles)
                {
                    foreach (Mobile mob in reg.GetMobiles())
                    {
                        if (!BaseCommand.IsAccessible(from, mob))
                            continue;

                        if (ext.IsValid(mob))
                            list.Add(mob);
                    }
                }
                else
                {
                    command.LogFailure("This command does not support items.");
                    return;
                }

                ext.Filter(list);

                obj = list;
            }
            catch (Exception ex)
            {
                from.SendMessage(ex.Message);
                Diagnostics.ExceptionLogging.LogException(ex);
            }
        }
    }
}
