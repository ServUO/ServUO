using System;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public class Neira : BaseChampion
    {
        private const double SpeedBoostScalar = 1.2;
        private bool m_SpeedBoost;
        [Constructable]
        public Neira()
            : base(AIType.AI_Mage)
        {
            this.Name = "Neira";
            this.Title = "the necromancer";
            this.Body = 401;
            this.Hue = 0x83EC;

            this.SetStr(305, 425);
            this.SetDex(72, 150);
            this.SetInt(505, 750);

            this.SetHits(4800);
            this.SetStam(102, 300);

            this.SetDamage(25, 35);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.EvalInt, 120.0);
            this.SetSkill(SkillName.Magery, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.MagicResist, 150.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 100.0);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 30;
            this.Female = true;

            Item shroud = new HoodedShroudOfShadows();

            shroud.Movable = false;

            this.AddItem(shroud);

            Scimitar weapon = new Scimitar();

            weapon.Skill = SkillName.Wrestling;
            weapon.Hue = 38;
            weapon.Movable = false;

            this.AddItem(weapon);

            //new SkeletalMount().Rider = this;
            this.AddItem(new VirtualMountItem(this));
        }

        public Neira(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Death;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(ShroudOfDeceit) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[]
                {
                    typeof(ANecromancerShroud),
                    typeof(CaptainJohnsHat),
                    typeof(DetectiveBoots)
                };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(WallBlood), typeof(TatteredAncientMummyWrapping) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { };
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool Uncalmable
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
            this.AddLoot(LootPack.Meager);
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if (mount != null)
                mount.Rider = null;

            if (mount is Mobile)
                ((Mobile)mount).Delete();

            return base.OnBeforeDeath();
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            this.CheckSpeedBoost();
            base.OnDamage(amount, from, willKill);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to drop or throw an unholy bone
                this.AddUnholyBone(defender, 0.25);
				
            this.CheckSpeedBoost();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to drop or throw an unholy bone
                this.AddUnholyBone(attacker, 0.25);
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            base.AlterDamageScalarFrom(caster, ref scalar);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to throw an unholy bone
                this.AddUnholyBone(caster, 1.0);
        }

        public void AddUnholyBone(Mobile target, double chanceToThrow)
        {
            if (this.Map == null)
                return;

            if (chanceToThrow >= Utility.RandomDouble())
            {
                this.Direction = this.GetDirectionTo(target);
                this.MovingEffect(target, 0xF7E, 10, 1, true, false, 0x496, 0);
                new DelayTimer(this, target).Start();
            }
            else
            {
                new UnholyBone().MoveToWorld(this.Location, this.Map);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write(this.m_SpeedBoost);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch( version )
            {
                case 1:
                    {
                        this.m_SpeedBoost = reader.ReadBool();
                        break;
                    }
            }
        }

        private void CheckSpeedBoost()
        {
            if (this.Hits < (this.HitsMax / 4))
            {
                if (!this.m_SpeedBoost)
                {
                    this.ActiveSpeed /= SpeedBoostScalar;
                    this.PassiveSpeed /= SpeedBoostScalar;
                    this.m_SpeedBoost = true;
                }
            }
            else if (this.m_SpeedBoost)
            {
                this.ActiveSpeed *= SpeedBoostScalar;
                this.PassiveSpeed *= SpeedBoostScalar;
                this.m_SpeedBoost = false;
            }
        }

        private class VirtualMount : IMount
        {
            private readonly VirtualMountItem m_Item;
            public VirtualMount(VirtualMountItem item)
            {
                this.m_Item = item;
            }

            public Mobile Rider
            {
                get
                {
                    return this.m_Item.Rider;
                }
                set
                {
                }
            }
            public virtual void OnRiderDamaged(int amount, Mobile from, bool willKill)
            {
            }
        }

        private class VirtualMountItem : Item, IMountItem
        {
            private readonly VirtualMount m_Mount;
            private Mobile m_Rider;
            public VirtualMountItem(Mobile mob)
                : base(0x3EBB)
            {
                this.Layer = Layer.Mount;

                this.Movable = false;

                this.m_Rider = mob;
                this.m_Mount = new VirtualMount(this);
            }

            public VirtualMountItem(Serial serial)
                : base(serial)
            {
                this.m_Mount = new VirtualMount(this);
            }

            public Mobile Rider
            {
                get
                {
                    return this.m_Rider;
                }
            }
            public IMount Mount
            {
                get
                {
                    return this.m_Mount;
                }
            }
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write((Mobile)this.m_Rider);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                this.m_Rider = reader.ReadMobile();

                if (this.m_Rider == null)
                    this.Delete();
            }
        }

        private class DelayTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_Target;
            public DelayTimer(Mobile m, Mobile target)
                : base(TimeSpan.FromSeconds(1.0))
            {
                this.m_Mobile = m;
                this.m_Target = target;
            }

            protected override void OnTick()
            {
                if (this.m_Mobile.CanBeHarmful(this.m_Target))
                {
                    this.m_Mobile.DoHarmful(this.m_Target);
                    AOS.Damage(this.m_Target, this.m_Mobile, Utility.RandomMinMax(10, 20), 100, 0, 0, 0, 0);
                    new UnholyBone().MoveToWorld(this.m_Target.Location, this.m_Target.Map);
                }
            }
        }
    }
}