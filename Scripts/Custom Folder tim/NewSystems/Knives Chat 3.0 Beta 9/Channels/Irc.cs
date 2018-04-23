using System;
using System.Collections;
using Server;
using Server.Gumps;

namespace Knives.Chat3
{
    public class Irc : Channel
    {
        public Irc() : base("IRC")
        {
            Commands.Add("irc");
            Commands.Add("i");
            DefaultC = 0x1FC;
            NewChars = true;

            Register(this);
        }

        public override string NameFor(Mobile m)
        {
            return Data.IrcRoom;
        }

        public override bool CanChat(Mobile m, bool say)
        {
            if (IrcConnection.Connection == null || !IrcConnection.Connection.Connected)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(158));
                return false;
            }

            return base.CanChat(m, say);
        }

        public void Broadcast(string name, string msg)
        {
            foreach (Data data in Data.Datas.Values)
            {
                if (IsIn(data.Mobile) && !data.IrcIgnores.Contains(name))
                    data.Mobile.SendMessage(ColorFor(data.Mobile), msg);
                else if (data.GlobalC)
                    data.Mobile.SendMessage(data.GlobalCC, String.Format("(Global) <{0}> {1}: {2}", Name, name, msg ));
            }
        }

        protected override void Broadcast(Mobile m, string msg)
        {
            base.Broadcast(m, msg);

            IrcConnection.Connection.SendUserMessage(m, msg);
        }

        public override ArrayList BuildList(Mobile m)
        {
            ArrayList list = base.BuildList(m);

            foreach (string str in Data.IrcList)
                list.Add(str);

            return list;
        }
    }
}