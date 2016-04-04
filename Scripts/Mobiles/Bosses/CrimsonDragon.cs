using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crimson dragon corpse")]
    public class CrimsonDragon : BasePeerless
    {
        private DateTime m_NextTerror;
        [Constructable]
        public CrimsonDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a crimson dragon";
            this.Body = 197;

            this.BaseSoundID = 362;
            this.SetStr(2034, 2140);
            this.SetDex(215, 256);
            this.SetInt(1025, 1116);

            this.SetHits(25000);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 50);

            this.SetResistance(ResistanceType.Physical, 80, 85);
            this.SetResistance(ResistanceType.Fire, 100);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 80, 85);
            this.SetResistance(ResistanceType.Energy, 80, 85);

            this.SetSkill(SkillName.EvalInt, 110.2, 125.3);
            this.SetSkill(SkillName.Magery, 110.9, 125.5);
            this.SetSkill(SkillName.MagicResist, 116.3, 125.0);
            this.SetSkill(SkillName.Tactics, 111.7, 126.3);
            this.SetSkill(SkillName.Wrestling, 120.5, 128.0);
            this.SetSkill(SkillName.Meditation, 119.4, 130.0);
            this.SetSkill(SkillName.Anatomy, 118.7, 125.0);
            this.SetSkill(SkillName.DetectHidden, 120.0);

            // ingredients
            this.PackResources(8);

            this.Fame = 20000;
            this.Karma = -20000;

            this.VirtualArmor = 70;
        }

        public CrimsonDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
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
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool Uncalmable
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int Hides
        {
            get
            {
                return 40;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override int Scales
        {
            get
            {
                return 12;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return (ScaleType)Utility.Random(4);
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Utility.RandomBool() ? Poison.Deadly : Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 8);
            this.AddLoot(LootPack.Gems, 12);
        }

        public override int GetIdleSound()
        {
            return 0x2D3;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            if (this.Map != null && caster != this && 0.50 > Utility.RandomDouble())
            {
                this.Map = caster.Map;
                this.Location = caster.Location;
                this.Combatant = caster;
                Effects.PlaySound(this.Location, this.Map, 0x1FE);
            }

            base.OnDamagedBySpell(caster);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (this.m_NextTerror < DateTime.UtcNow && m != null && this.InRange(m.Location, 3) && m.IsPlayer())
            {
                m.Frozen = true;
                m.SendLocalizedMessage(1080342, this.Name, 33); // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had

                Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerStateCallback(Terrorize), m);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (this.Map != null && attacker != this && 0.1 > Utility.RandomDouble())
            {
                if (attacker is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)attacker;
                    if (pet.ControlMaster != null && (attacker is Dragon || attacker is GreaterDragon || attacker is SkeletalDragon || attacker is WhiteWyrm || attacker is Drake))
                    {
                        this.Combatant = null;
                        pet.Combatant = null;
                        this.Combatant = null;
                        pet.ControlMaster = null;
                        pet.Controlled = false;
                        attacker.Emote(String.Format("* {0} decided to go wild *", attacker.Name));
                    }

                    if (pet.ControlMaster != null && 0.1 > Utility.RandomDouble())
                    {
                        this.Combatant = null;
                        pet.Combatant = pet.ControlMaster;
                        this.Combatant = null;
                        attacker.Emote(String.Format("* {0} is being angered *", attacker.Name));
                    }
                }
            }

            base.OnGotMeleeAttack(attacker);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override bool OnBeforeDeath()
        {
            this.Hue = 16385;

            if (!this.NoKillAwards)
            {
                Map map = this.Map;

                if (map != null)
                {
                    for (int x = -7; x <= 7; ++x)
                    {
                        for (int y = -7; y <= 3; ++y)
                        {
                            double dist = Math.Sqrt(x * x + y * y);

                            if (dist <= 12)
                                new GoodiesTimer(map, this.X + x, this.Y + y).Start();
                        }
                    }
                }
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new ParrotItem());

            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());
        }

        private void Terrorize(object o)
        {
            if (o is Mobile)
            {
                Mobile m = (Mobile)o;

                m.Frozen = false;
                m.SendLocalizedMessage(1005603); // You can move again!

                this.m_NextTerror = DateTime.UtcNow + TimeSpan.FromMinutes(5);
            }
        }

        private class GoodiesTimer : Timer
        {
            private readonly Map m_Map;
            private readonly int m_X;
            private readonly int m_Y;
            public GoodiesTimer(Map map, int x, int y)
                : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                this.m_Map = map;
                this.m_X = x;
                this.m_Y = y;
            }

            protected override void OnTick()
            {
                int z = this.m_Map.GetAverageZ(this.m_X, this.m_Y);
                bool canFit = this.m_Map.CanFit(this.m_X, this.m_Y, z, 6, false, false);

                for (int i = -3; !canFit && i <= 3; ++i)
                {
                    canFit = this.m_Map.CanFit(this.m_X, this.m_Y, z + i, 6, false, false);

                    if (canFit)
                        z += i;
                }

                if (!canFit)
                    return;

                Gold g = new Gold(300, 500);

                g.MoveToWorld(new Point3D(this.m_X, this.m_Y, z), this.m_Map);

                if (0.5 >= Utility.RandomDouble())
                {
                    switch (Utility.Random(3))
                    {
                        case 0: // Fire column
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                                Effects.PlaySound(g, g.Map, 0x208);

                                break;
                            }
                        case 1: // Explosion
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                                Effects.PlaySound(g, g.Map, 0x307);

                                break;
                            }
                        case 2: // Ball of fire
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);

                                break;
                            }
                    }
                }
            }
        }
    }
}