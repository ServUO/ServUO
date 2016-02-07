using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Commands.Generic
{
    public class OnlineCommandImplementor : BaseCommandImplementor
    {
        public OnlineCommandImplementor()
        {
            this.Accessors = new string[] { "Online" };
            this.SupportRequirement = CommandSupport.Online;
            this.SupportsConditionals = true;
            this.AccessLevel = AccessLevel.GameMaster;
            this.Usage = "Online <command> [condition]";
            this.Description = "Invokes the command on all mobiles that are currently logged in. Optional condition arguments can further restrict the set of objects.";
        }

        public override void Compile(Mobile from, BaseCommand command, ref string[] args, ref object obj)
        {
            try
            {
                Extensions ext = Extensions.Parse(from, ref args);

                bool items, mobiles;

                if (!this.CheckObjectTypes(from, command, ext, out items, out mobiles))
                    return;

                if (!mobiles) // sanity check
                {
                    command.LogFailure("This command does not support items.");
                    return;
                }

                ArrayList list = new ArrayList();

                List<NetState> states = NetState.Instances;

                for (int i = 0; i < states.Count; ++i)
                {
                    NetState ns = states[i];
                    Mobile mob = ns.Mobile;

                    if (mob == null)
                        continue;

                    if (!BaseCommand.IsAccessible(from, mob))
                        continue;

                    if (ext.IsValid(mob))
                        list.Add(mob);
                }

                ext.Filter(list);

                obj = list;
            }
            catch (Exception ex)
            {
                from.SendMessage(ex.Message);
            }
        }
    }
}