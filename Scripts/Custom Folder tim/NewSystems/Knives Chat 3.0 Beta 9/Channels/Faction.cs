using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class Faction : Channel
    {
        public Faction() : base("Faction")
        {
            Commands.Add("faction");
            Commands.Add("f");
            DefaultC = 0x17;
            NewChars = true;

            Register(this);
        }

        public override bool CanChat(Mobile m, bool say)
        {
            if (!General.IsInFaction(m))
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(37));
                return false;
            }

            return base.CanChat(m, say);
        }

        protected override void Broadcast(Mobile m, string msg)
        {
            foreach (Data data in Data.Datas.Values)
            {
                if (IsIn(data.Mobile) && !data.Ignores.Contains(m) && General.FactionName(data.Mobile) == General.FactionName(m))
                    data.Mobile.SendMessage(m.AccessLevel == AccessLevel.Player ? ColorFor(data.Mobile) : Data.GetData(m).StaffC, String.Format("<{0}{1}> {2}: {3}", NameFor(m), (Style == ChatStyle.Regional && m.Region != null ? "-" + m.Region.Name : ""), m.RawName, msg));
                else if (data.Mobile.AccessLevel >= m.AccessLevel && ((data.GlobalF && !data.GIgnores.Contains(m)) || data.GListens.Contains(m)))
                    data.Mobile.SendMessage(data.GlobalFC, String.Format("(Global) <{0}> {1}: {2}", NameFor(m), m.RawName, msg));
            }
        }

        public override ArrayList BuildList(Mobile m)
        {
            ArrayList list = base.BuildList(m);

            foreach (Mobile mob in new ArrayList(list))
                if (General.FactionName(mob) != General.FactionName(m))
                    list.Remove(mob);

            return list;
        }
    }
}