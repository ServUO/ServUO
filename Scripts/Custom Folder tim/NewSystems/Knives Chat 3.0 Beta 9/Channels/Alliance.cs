using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class Alliance : Channel
    {
        public Alliance() : base("Alliance")
        {
            Commands.Add("ally");
            Commands.Add("a");
            DefaultC = 0x9E;
            NewChars = true;

            Register(this);
        }

        public override ArrayList GetHistory(Mobile m)
        {
            return new ArrayList();
        }

        public override void AddHistory(Mobile m, string msg)
        {
        }

        public override void UpdateHistory(Mobile m)
        {
        }

        public override bool CanChat(Mobile m, bool say)
        {
            if (m.Guild == null)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(36));
                return false;
            }

            return base.CanChat(m, say);
        }

        protected override void Broadcast(Mobile m, string msg)
        {
            foreach (Data data in Data.Datas.Values)
            {
                if (data.Mobile.Guild == null)
                    continue;

                if (data.Mobile.AccessLevel >= m.AccessLevel && ((data.GlobalG && !data.GIgnores.Contains(m)) || data.GListens.Contains(m)))
                    data.Mobile.SendMessage(data.GlobalGC, String.Format("(Alliance) <{0}> {1}: {2}", NameFor(m), m.RawName, msg ));
                else if (IsIn(data.Mobile) && !data.Ignores.Contains(m) && (data.Mobile.Guild == m.Guild || ((Server.Guilds.Guild)data.Mobile.Guild).Allies.Contains((Server.Guilds.Guild)m.Guild)))
                    data.Mobile.SendMessage(m.AccessLevel == AccessLevel.Player ? ColorFor(data.Mobile) : Data.GetData(m).StaffC, String.Format("<{0}{1}> {2}: {3}", NameFor(m), (Style == ChatStyle.Regional && m.Region != null ? "-" + m.Region.Name : ""), m.RawName, msg));
            }
        }

        public override ArrayList BuildList(Mobile m)
        {
            ArrayList list = base.BuildList(m);

            foreach (Mobile mob in new ArrayList(list))
                if (mob.Guild == null || m.Guild == null || (mob.Guild != m.Guild && !((Server.Guilds.Guild)mob.Guild).Allies.Contains((Server.Guilds.Guild)m.Guild)))
                    list.Remove(mob);

            return list;
        }
    }
}