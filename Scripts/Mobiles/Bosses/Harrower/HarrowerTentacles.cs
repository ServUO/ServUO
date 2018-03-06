using System;
using System.Collections;
using Server.Items;

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
            this.m_Harrower = harrower;

            this.Name = "tentacles of the harrower";
            this.Body = 129;

            this.SetStr(901, 1000);
            this.SetDex(126, 140);
            this.SetInt(1001, 1200);

            this.SetHits(541, 600);

            this.SetDamage(13, 20);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 35, 45);
            this.SetResistance(ResistanceType.Poison, 35, 45);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.Meditation, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.1, 140.0);
            this.SetSkill(SkillName.Swords, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 60;

            this.m_Timer = new DrainTimer(this);
            this.m_Timer.Start();

            this.PackReg(50);
            this.PackNecroReg(15, 75);

			switch (Utility.Random(3))
            {
                case 0: PackItem(new VampiricEmbraceScroll()); break;
			}

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
                return this.m_Harrower;
            }
            set
            {
                this.m_Harrower = value;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override bool DisallowAllMoves
        {
            get
            {
                return true;
            }
        }
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
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.MedScrolls, 3);
            this.AddLoot(LootPack.HighScrolls, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Harrower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Harrower = reader.ReadMobile();

                        this.m_Timer = new DrainTimer(this);
                        this.m_Timer.Start();

                        break;
                    }
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        private class DrainTimer : Timer
        {
            private static readonly ArrayList m_ToDrain = new ArrayList();
            private readonly HarrowerTentacles m_Owner;
            public DrainTimer(HarrowerTentacles owner)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                this.m_Owner = owner;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (this.m_Owner.Deleted)
                {
                    this.Stop();
                    return;
                }

                IPooledEnumerable eable = m_Owner.GetMobilesInRange(9);

                foreach (Mobile m in eable)
                {
                    if (m == this.m_Owner || m == this.m_Owner.Harrower || !this.m_Owner.CanBeHarmful(m))
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
                    this.m_Owner.DoHarmful(m);

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

                    this.m_Owner.Hits += drain;

                    if (this.m_Owner.Harrower != null)
                        this.m_Owner.Harrower.Hits += drain;

                    m.Damage(drain, this.m_Owner);
                }

                m_ToDrain.Clear();
            }
        }
    }
}