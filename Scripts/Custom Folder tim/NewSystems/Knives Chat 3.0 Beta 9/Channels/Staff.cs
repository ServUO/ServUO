using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class Staff : Channel
    {
        public Staff() : base("Staff")
        {
            Commands.Add("staff");
            Commands.Add("st");
            DefaultC = 0x26;
            ShowStaff = true;

            Register(this);
        }

        public override bool CanChat(Mobile m, bool say)
        {
            if (m.AccessLevel == AccessLevel.Player)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(191));
                return false;
            }

            return base.CanChat(m, say);
        }

        public override ArrayList BuildList(Mobile m)
        {
            ArrayList list = base.BuildList(m);

            foreach (Data data in Data.Datas.Values)
                if (!list.Contains(data.Mobile) && data.Mobile.AccessLevel > AccessLevel.Player)
                    list.Add(data.Mobile);

            return list;
        }
    }
}