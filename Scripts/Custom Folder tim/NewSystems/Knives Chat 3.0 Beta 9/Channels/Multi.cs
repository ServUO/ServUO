using System;
using System.Collections;
using Server;
using Server.Gumps;

namespace Knives.Chat3
{
    public class Multi : Channel
    {
        public Multi() : base("Multi")
        {
            Commands.Add("Mult");
            Commands.Add("mu");
            DefaultC = 0x1FC;

            Register(this);
        }

        public override string NameFor(Mobile m)
        {
            return Server.Misc.ServerList.ServerName;
        }

        public override bool CanChat(Mobile m, bool say)
        {
            if (MultiConnection.Connection == null || !MultiConnection.Connection.Connected)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(158));
                return false;
            }

            return base.CanChat(m, say);
        }

        public void Broadcast(string msg)
        {
            foreach (Data data in Data.Datas.Values)
            {
                if (IsIn(data.Mobile))
                    data.Mobile.SendMessage(ColorFor(data.Mobile), msg);
                else if (data.GlobalC)
                    data.Mobile.SendMessage(data.GlobalCC, String.Format("(Global) {0}", msg ));
            }
        }

        protected override void Broadcast(Mobile m, string msg)
        {
            //base.Broadcast(m, msg);

            MultiConnection.Connection.SendMessage(m, msg);
        }

        public override ArrayList BuildList(Mobile m)
        {
            ArrayList list = base.BuildList(m);

            //foreach (string str in Data.IrcList)
            //    list.Add(str);

            return list;
        }
    }
}