using Server.Network;
using System;

namespace Server.Items
{
    public class MercutiosCutlass : Cutlass
    {
        public override int LabelNumber => 1154240;  // Mercutio's Cutlass
        private int m_Lifespan;
        private Timer m_Timer;

        [Constructable]
        public MercutiosCutlass()
        {
            Hue = 2595;
            LootType = LootType.Blessed;

            if (Lifespan > 0)
            {
                m_Lifespan = Lifespan;
                StartTimer();
            }
        }

        public virtual int Lifespan => 18000;

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154241); // *It is a beautifully and masterfully crafted blade. The hilt bears the family crest of the former owner*
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item

            if (Lifespan > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(m_Lifespan);

                int weeks = t.Days / 7;
                int days = t.Days;
                int hours = t.Hours;
                int minutes = t.Minutes;

                if (weeks > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", weeks, weeks == 1 ? "week" : "weeks"));
                else if (days > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", days, days == 1 ? "day" : "days"));
                else if (hours > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", hours, hours == 1 ? "hour" : "hours"));
                else if (minutes > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", minutes, minutes == 1 ? "minute" : "minutes"));
                else
                    list.Add(1072517, m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
            }
        }

        public virtual void StartTimer()
        {
            if (m_Timer != null)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), Slice);
            m_Timer.Priority = TimerPriority.OneSecond;
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public virtual void Slice()
        {
            m_Lifespan -= 10;

            InvalidateProperties();

            if (m_Lifespan <= 0)
                Decay();
        }

        public virtual void Decay()
        {
            if (RootParent is Mobile)
            {
                Mobile parent = (Mobile)RootParent;

                if (Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(Location, Map, 0x201);
            }

            StopTimer();
            Delete();
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            cold = fire = chaos = pois = nrgy = direct = 0;
            phys = 100;
        }

        public MercutiosCutlass(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Lifespan);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Lifespan = reader.ReadInt();
            StartTimer();
        }
    }
}
