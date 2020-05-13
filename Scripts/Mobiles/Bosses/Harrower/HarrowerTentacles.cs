using Server.Items;
using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a tentacles corpse")]
    public class HarrowerTentacles : BaseCreature
    {
        private Mobile m_Harrower;
        private DrainTimer m_Timer;
        [Constructable]
        public HarrowerTentacles()
            : this(null)
        {
        }

        public HarrowerTentacles(Mobile harrower)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Harrower = harrower;

            Name = "tentacles of the harrower";
            Body = 129;

            SetStr(901, 1000);
            SetDex(126, 140);
            SetInt(1001, 1200);

            SetHits(541, 600);

            SetDamage(13, 20);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.MagicResist, 120.1, 140.0);
            SetSkill(SkillName.Swords, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 15000;
            Karma = -15000;

            m_Timer = new DrainTimer(this);
            m_Timer.Start();
        }

        public HarrowerTentacles(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Harrower
        {
            get
            {
                return m_Harrower;
            }
            set
            {
                m_Harrower = value;
            }
        }
        public override bool AutoDispel => true;

        public override bool Unprovokable => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override bool DisallowAllMoves => true;

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = true;
        }

        public override int GetIdleSound()
        {
            return 0x101;
        }

        public override int GetAngerSound()
        {
            return 0x5E;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }

        public override int GetAttackSound()
        {
            return -1; // unknown
        }

        public override int GetHurtSound()
        {
            return 0x289;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 3);
            AddLoot(LootPack.HighScrolls, 2);
            AddLoot(LootPack.MageryRegs, 50);
            AddLoot(LootPack.NecroRegs, 15, 75);
            AddLoot(LootPack.LootItem<VampiricEmbraceScroll>(33.3, true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Harrower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Harrower = reader.ReadMobile();
            m_Timer = new DrainTimer(this);
            m_Timer.Start();
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        private class DrainTimer : Timer
        {
            private static readonly ArrayList m_ToDrain = new ArrayList();
            private readonly HarrowerTentacles m_Owner;
            public DrainTimer(HarrowerTentacles owner)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_Owner = owner;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                IPooledEnumerable eable = m_Owner.GetMobilesInRange(9);

                foreach (Mobile m in eable)
                {
                    if (m == m_Owner || m == m_Owner.Harrower || !m_Owner.CanBeHarmful(m))
                        continue;

                    if (m is BaseCreature)
                    {
                        BaseCreature bc = m as BaseCreature;

                        if (bc.Controlled || bc.Summoned)
                            m_ToDrain.Add(m);
                    }
                    else if (m.Player)
                    {
                        m_ToDrain.Add(m);
                    }
                }

                eable.Free();

                foreach (Mobile m in m_ToDrain)
                {
                    m_Owner.DoHarmful(m);

                    m.FixedParticles(0x374A, 10, 15, 5013, 0x455, 0, EffectLayer.Waist);
                    m.PlaySound(0x1F1);

                    int drain = Utility.RandomMinMax(14, 30);

                    //Monster Stealables 
                    if (m is PlayerMobile)
                    {
                        PlayerMobile pm = m as PlayerMobile;
                        drain = (int)LifeShieldLotion.HandleLifeDrain(pm, drain);
                    }
                    //end 

                    m_Owner.Hits += drain;

                    if (m_Owner.Harrower != null)
                        m_Owner.Harrower.Hits += drain;

                    m.Damage(drain, m_Owner);
                }

                m_ToDrain.Clear();
            }
        }
    }
}
