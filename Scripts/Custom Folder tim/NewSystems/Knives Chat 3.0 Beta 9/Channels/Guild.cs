using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
    public class Guild : Channel
    {
        private Hashtable c_History = new Hashtable();

        public Guild() : base("Guild")
        {
            Commands.Add("guild");
            Commands.Add("g");
            DefaultC = 0x44;
            NewChars = true;
            Filter = false;
            Delay = false;

            Register(this);
        }

        public override ArrayList GetHistory(Mobile m)
        {
            if (m.Guild == null)
                return new ArrayList();

            if (c_History[m.Guild] == null)
                c_History[m.Guild] = new ArrayList();

            return (ArrayList)c_History[m.Guild];
        }

        public override void AddHistory(Mobile m, string msg)
        {
            if (m.Guild == null)
                return;

            if (c_History[m.Guild] == null)
                c_History[m.Guild] = new ArrayList();

            ((ArrayList)c_History[m.Guild]).Add(new ChatHistory(m, msg));
        }

        public override void UpdateHistory(Mobile m)
        {
            if(m.Guild == null)
                return;

            if (c_History[m.Guild] == null)
                c_History[m.Guild] = new ArrayList();

            if (((ArrayList)c_History[m.Guild]).Count > 50)
                ((ArrayList)c_History[m.Guild]).RemoveAt(0);
        }

        public override string NameFor(Mobile m)
        {
            if (m.Guild == null)
                return Name;

            if (m.Guild.Abbreviation == "")
                return Name;

            return m.Guild.Abbreviation;
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
                if (data.Mobile.AccessLevel >= m.AccessLevel && ((data.GlobalG && !data.GIgnores.Contains(m)) || data.GListens.Contains(m)))
                    data.Mobile.SendMessage(data.GlobalGC, String.Format("(Global) <{0}> {1}: {2}", NameFor(m), m.RawName, msg));
                else if (IsIn(data.Mobile) && !data.Ignores.Contains(m) && data.Mobile.Guild == m.Guild)
                    data.Mobile.SendMessage(m.AccessLevel == AccessLevel.Player ? ColorFor(data.Mobile) : Data.GetData(m).StaffC, String.Format("<{0}{1}> {2}: {3}", NameFor(m), (Style == ChatStyle.Regional && m.Region != null ? "-" + m.Region.Name : ""), m.RawName, msg));
            }
        }

        public override ArrayList BuildList(Mobile m)
        {
            ArrayList list = base.BuildList(m);

            foreach (Mobile mob in new ArrayList(list))
                if (mob.Guild != m.Guild)
                    list.Remove(mob);

            return list;
        }
    }
}