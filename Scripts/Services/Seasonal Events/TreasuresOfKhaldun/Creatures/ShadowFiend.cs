using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("an incorporeal corpse")]
    public class ShadowFiend : BaseCreature
    {
        private UnhideTimer m_Timer;

        [Constructable]
        public ShadowFiend()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a shadow fiend";
            Body = 0x10;
            Hue = 2051;

            // this to allow shadow fiend to loot from corpses
            Backpack backpack = new Backpack
            {
                Movable = false
            };
            AddItem(backpack);

            SetStr(300, 400);
            SetDex(200, 250);
            SetInt(45, 55);

            SetHits(300, 500);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 100.0);
            SetSkill(SkillName.DetectHidden, 250.0);
            SetSkill(SkillName.Hiding, 100.0);
            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.Focus, 0.0, 20.0);

            Fame = 1000;
            Karma = -1000;

            m_Timer = new UnhideTimer(this);
            m_Timer.Start();
        }

        public override void OnBeforeDamage(Mobile from, ref int totalDamage, DamageType type)
        {
            if (Region.IsPartOf("Khaldun") && IsChampionSpawn && !Caddellite.CheckDamage(from, type))
            {
                totalDamage = 0;
            }

            base.OnBeforeDamage(from, ref totalDamage, type);
        }

        public ShadowFiend(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath => true;
        public override bool CanRummageCorpses => true;
        public override int GetIdleSound()
        {
            return 0x37A;
        }

        public override int GetAngerSound()
        {
            return 0x379;
        }

        public override int GetDeathSound()
        {
            return 0x381;
        }

        public override int GetAttackSound()
        {
            return 0x37F;
        }

        public override int GetHurtSound()
        {
            return 0x380;
        }

        public override bool OnBeforeDeath()
        {
            if (Backpack != null)
                Backpack.Destroy();

            Effects.SendLocationEffect(Location, Map, 0x376A, 10, 1);
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Timer = new UnhideTimer(this);
            m_Timer.Start();
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        private class UnhideTimer : Timer
        {
            private readonly ShadowFiend m_Owner;
            public UnhideTimer(ShadowFiend owner)
                : base(TimeSpan.FromSeconds(30.0))
            {
                m_Owner = owner;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                IPooledEnumerable eable = m_Owner.GetMobilesInRange(3);

                foreach (Mobile m in eable)
                {
                    if (m != m_Owner && m.Player && m.Hidden && m_Owner.CanBeHarmful(m) && m.IsPlayer())
                        m.Hidden = false;
                }
                eable.Free();
            }
        }
    }
}
