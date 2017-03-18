using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class NavreyParalyzingWeb : Item
    {
        private Timer m_Timer;
        private List<Mobile> m_StuckMobs = new List<Mobile>();
        private TimeSpan m_Duration;

        public List<Mobile> StuckMobs { get { return m_StuckMobs; } }

        public NavreyParalyzingWeb(TimeSpan duration, Mobile mob)
            : base(Utility.RandomList(0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6))
        {
            this.Visible = true;
            this.Movable = false;
            m_StuckMobs.Add(mob);
            m_Duration = duration;

            this.m_Timer = new InternalTimer(this, m_Duration);
            this.m_Timer.Start();

            BuffInfo.AddBuff(mob, new BuffInfo(BuffIcon.Webbing, 1153789, 1153825));
        }

        public NavreyParalyzingWeb(Serial serial)
            : base(serial)
        {
        }

        public override bool BlocksFit
        {
            get
            {
                return true;
            }
        }
        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            foreach (Mobile m in m_StuckMobs)
            {
                if (m.Frozen)
                {
                    m.SendLocalizedMessage(1005603); //You can move again!
                    m.Frozen = false;
                }

                Server.Mobiles.Navrey.RemoveFromTable(m);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_StuckMobs.Count);
            foreach (Mobile m in m_StuckMobs)
                writer.Write(m);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            int c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                Mobile m = reader.ReadMobile();
                if (m != null && m.Frozen)
                    m.Frozen = false;
            }

            Delete();
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is Navrey)
                return true;

            if (m.AccessLevel == AccessLevel.Player && m.Alive)
            {
                m.Paralyze(m_Duration);

                m_StuckMobs.Add(m);

                m.PlaySound(0x204);
                m.FixedEffect(0x376A, 10, 16);

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Webbing, 1153789, 1153825));
            }

            return true;
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Item;

            public InternalTimer(Item item, TimeSpan duration)
                : base(duration)
            {
                this.Priority = TimerPriority.OneSecond;
                this.m_Item = item;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}