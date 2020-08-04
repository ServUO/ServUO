using System;

namespace Server.Items
{
    public class HealingStone : Item
    {
        private Mobile m_Caster;
        private int m_LifeForce;
        private int m_MaxLifeForce;
        private int m_MaxHeal;
        private int m_MaxHealTotal;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Caster => m_Caster;

        [CommandProperty(AccessLevel.GameMaster)]
        public int LifeForce { get { return m_LifeForce; } set { m_LifeForce = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLifeForce => m_MaxLifeForce;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHeal => m_MaxHeal;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHealTotal => m_MaxHealTotal;

        public override bool Nontransferable => true;

        [Constructable]
        public HealingStone(Mobile caster, int amount, int maxHeal) : base(0x4078)
        {
            m_Caster = caster;
            m_LifeForce = amount;
            m_MaxHeal = maxHeal;

            m_MaxLifeForce = amount;
            m_MaxHealTotal = maxHeal;

            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use
                return;
            }
            else if (from != m_Caster)
            {
            }
            else if (!BasePotion.HasFreeHand(from))
            {
                from.SendLocalizedMessage(1080116); // You must have a free hand to use a Healing Stone.
            }
            else if (from.Hits >= from.HitsMax && !from.Poisoned)
            {
                from.SendLocalizedMessage(1049547); //You are already at full health.
            }
            else if (from.CanBeginAction(typeof(HealingStone)))
            {
                from.BeginAction(typeof(HealingStone));

                if (m_MaxHeal > m_LifeForce)
                {
                    m_MaxHeal = m_LifeForce;
                }

                Timer.DelayCall(TimeSpan.FromSeconds(2), m => m.EndAction(typeof(HealingStone)), from);

                if (from.Poisoned)
                {
                    int toUse = Math.Min(120, from.Poison.RealLevel * 25);

                    if (m_MaxLifeForce < toUse)
                    {
                        from.SendLocalizedMessage(1115265); //Your Mysticism, Focus, or Imbuing Skills are not enough to use the heal stone to cure yourself.
                    }
                    else if (m_LifeForce < toUse)
                    {
                        from.SendLocalizedMessage(1115264); //Your healing stone does not have enough energy to remove the poison.
                        LifeForce -= toUse / 3;
                    }
                    else
                    {
                        from.CurePoison(from);

                        from.SendLocalizedMessage(500231); // You feel cured of poison!

                        from.FixedEffect(0x373A, 10, 15);
                        from.PlaySound(0x1E0);

                        LifeForce -= toUse;
                    }

                    if (m_LifeForce <= 0)
                    {
                        Consume();
                    }

                    return;
                }
                else
                {
                    int toHeal = Math.Min(m_MaxHeal, from.HitsMax - from.Hits);
                    from.Heal(toHeal);

                    from.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    from.PlaySound(0x202);

                    LifeForce -= toHeal;
                    m_MaxHeal = 1;
                }

                if (m_LifeForce <= 0)
                {
                    from.SendLocalizedMessage(1115266); //The healing stone has used up all its energy and has been destroyed.
                    Consume();
                }
                else
                {
                    if (m_Timer != null)
                        m_Timer.Stop();

                    m_Timer = new InternalTimer(this);
                }
            }
            else
            {
                from.SendLocalizedMessage(1095172); // You must wait a few seconds before using another Healing Stone.
            }
        }

        public void OnTick()
        {
            if (m_MaxHeal < m_MaxHealTotal)
            {
                int maxToHeal = m_MaxHealTotal - m_MaxHeal;
                m_MaxHeal += Math.Min(maxToHeal, m_MaxHealTotal / 15);

                if (m_MaxHeal > m_MaxHealTotal)
                    m_MaxHeal = m_MaxHealTotal;
            }
        }

        private class InternalTimer : Timer
        {
            private readonly HealingStone m_Stone;
            private int m_Ticks;

            public InternalTimer(HealingStone stone) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Stone = stone;
                m_Ticks = 0;
                Start();
            }

            protected override void OnTick()
            {
                m_Ticks++;

                m_Stone.OnTick();

                if (m_Ticks >= 15)
                    Stop();
            }
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            Delete();
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void Delete()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            base.Delete();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1115274, m_LifeForce.ToString());
        }

        public HealingStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(m_Caster);
            writer.Write(m_LifeForce);
            writer.Write(m_MaxLifeForce);
            writer.Write(m_MaxHeal);
            writer.Write(m_MaxHealTotal);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Caster = reader.ReadMobile();
                    m_LifeForce = reader.ReadInt();
                    m_MaxLifeForce = reader.ReadInt();
                    m_MaxHeal = reader.ReadInt();
                    m_MaxHealTotal = reader.ReadInt();
                    break;
                case 0:
                    break;
            }

            if (m_LifeForce <= 0)
            {
                Delete();
            }
        }

        public static void OnHealFromPotion(Mobile from, int healed)
        {
            if (from.Backpack != null)
            {
                var stone = from.Backpack.FindItemByType<HealingStone>();

                if (stone != null)
                {
                    stone.m_MaxHeal = 1;

                    if (from.CanBeginAction(typeof(HealingStone)))
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(2), m => m.EndAction(typeof(HealingStone)), from);
                    }
                }
            }
        }  
    }
}
