using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class ScaleCollar : Item
    {
        public override int LabelNumber { get { return 1112480; } } //a scale collar

        private Timer m_Timer;

        [Constructable]
        public ScaleCollar() : base(4235)
        {
            Hue = 2125;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && m_Timer == null)
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1112481); //Which battle chicken do you wish to ensnare?
            }
            else if (IsChildOf(from.Backpack) && m_Timer != null)
            {
                from.SendLocalizedMessage(501789); //You must wait before trying again.
            }
            else
                from.SendLocalizedMessage(1042004); // That must be in your pack for you to use it.
        }

        public void OnTarget(Mobile from, object targeted)
        {
            if (targeted is BattleChickenLizard && !((BattleChickenLizard)targeted).Controlled)
            {
                BattleChickenLizard bcl = (BattleChickenLizard)targeted;

                int chance = 50 / (int)Math.Max(1, from.GetDistanceToSqrt(bcl.Location));

                if (chance > Utility.Random(100))
                {
                    bcl.Frozen = true;

                    m_Timer = new InternalTimer(this, bcl, from);
                    from.SendLocalizedMessage(1112484); //You successfully ensnare the chicken! You best hurry before it frees itself from it!
                }
                else
                    from.SendLocalizedMessage(1112483); //The collar falls to the ground as the chicken deftly avoids it.
            }
        }

        public void OnTick(BaseCreature lizard, Mobile owner)
        {
            if (lizard != null && lizard.Controlled)
            {
                lizard.Frozen = false;

                m_Timer.Stop();
                m_Timer = null;
            }
            else
                lizard.FixedEffect(0x376A, 1, 32);
        }

        public void EndTimer(BaseCreature lizard, Mobile owner)
        {
            if (lizard != null && lizard.Alive)
            {
                lizard.Frozen = false;

                if (owner != null && !lizard.Controlled)
                    owner.SendLocalizedMessage(1112482); //The chicken frees itself of the collar!!
            }

            m_Timer.Stop();
            m_Timer = null;
        }

        private class InternalTarget : Target
        {
            private ScaleCollar m_Collar;

            public InternalTarget(ScaleCollar collar) : base(-1, false, TargetFlags.None)
            {
                m_Collar = collar;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if(m_Collar != null)
                    m_Collar.OnTarget(from, targeted);
            }
        }

        private class InternalTimer : Timer
        {
            private ScaleCollar m_Collar;
            private BattleChickenLizard m_Lizard;
            private DateTime m_EndTime;
            private Mobile m_Owner;

            public InternalTimer(ScaleCollar collar, BattleChickenLizard lizard, Mobile owner) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Collar = collar;
                m_Lizard = lizard;
                m_Owner = owner;
                m_EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);

                lizard.FixedEffect(0x376A, 1, 32);
            }

            protected override void OnTick()
            {
                if (m_EndTime < DateTime.UtcNow)
                    m_Collar.EndTimer(m_Lizard, m_Owner);
                else
                    m_Collar.OnTick(m_Lizard, m_Owner);
            }
        }

        public ScaleCollar(Serial serial) : base(serial)
        {
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
        }
    }
}