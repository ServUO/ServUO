using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpiderWebbing : Item
    {
        private Timer m_Timer;
        private static List<Mobile> m_WebVictims = new List<Mobile>();

        public SpiderWebbing(Mobile m)
            : base(0xEE3 + Utility.Random(4))
        {            
            Movable = false;

            BeginWebbing(m);

            m_Timer = new InternalTimer(this, m);
            m_Timer.Start();
        }

        public SpiderWebbing(Serial serial)
            : base(serial)
        {
        }

        public void BeginWebbing(Mobile m)
        {            
            m.RevealingAction();
            m.Frozen = true;            
            m.SendLocalizedMessage(1113247); // You are wrapped in spider webbing and cannot move!
            m_WebVictims.Add(m);
            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Webbing, 1153789, 1153825));
        }

        public static bool IsTrapped(Mobile m)
        {
            return m_WebVictims != null && m_WebVictims.Contains(m);
        }

        public static void RemoveEffects(Mobile m)
        {
            m.Frozen = false;
            m.SendLocalizedMessage(1113248); // You escape the spider's web!           
            BuffInfo.RemoveBuff(m, BuffIcon.Webbing);
            m_WebVictims.Remove(m);
        }

        public override bool BlocksFit { get { return true; } }

        public override void OnDelete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            var list = new List<Mobile>(m_WebVictims);

            foreach (var m in list)
            {
                RemoveEffects(m);
            }

            ColUtility.Free(list);
            ColUtility.Free(m_WebVictims);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete();
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is BaseCreature && ((BaseCreature)m).IsMonster)
                return true;

            if (m.AccessLevel == AccessLevel.Player && m.Alive)
            {
                BeginWebbing(m);
                m.PlaySound(0x204);
                m.FixedEffect(0x376A, 10, 16);
            }

            return true;
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Target;
            private Item m_Item;
            private int m_Ticks;

            public InternalTimer(Item item, Mobile target)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_Item = item;
                m_Target = target;
                m_Ticks = 10;
            }

            protected override void OnTick()
            {
                m_Ticks--;

                if (!m_Target.Alive || m_Ticks == 0)
                {
                    m_Item.Delete();
                }
            }
        }
    }
}
