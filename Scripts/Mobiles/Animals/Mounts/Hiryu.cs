using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hiryu corpse")]
    public class Hiryu : BaseMount
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public Hiryu()
            : base("a hiryu", 243, 0x3E94, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Hue = GetHue();

            this.SetStr(1201, 1410);
            this.SetDex(171, 270);
            this.SetInt(301, 325);

            this.SetHits(901, 1100);
            this.SetMana(60);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 70);
            this.SetResistance(ResistanceType.Fire, 70, 90);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 75.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 85.1, 100.0);
            this.SetSkill(SkillName.Tactics, 100.1, 110.0);
            this.SetSkill(SkillName.Wrestling, 100.1, 120.0);

            this.Fame = 18000;
            this.Karma = -18000;

            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 98.7;

            if (Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());

            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(3));
        }

        public Hiryu(Serial serial)
            : base(serial)
        {
        }

        public override bool StatLossAfterTame
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override int Meat
        {
            get
            {
                return 16;
            }
        }
        public override int Hides
        {
            get
            {
                return 60;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override bool CanAngerOnTame
        {
            get
            {
                return true;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        public override int GetAngerSound()
        {
            return 0x4FE;
        }

        public override int GetIdleSound()
        {
            return 0x4FD;
        }

        public override int GetAttackSound()
        {
            return 0x4FC;
        }

        public override int GetHurtSound()
        {
            return 0x4FF;
        }

        public override int GetDeathSound()
        {
            return 0x4FB;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.Gems, 4);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Grasping Claw
                * Start cliloc: 1070836
                * Effect: Physical resistance -15% for 5 seconds
                * End cliloc: 1070838
                * Effect: Type: "3" - From: "0x57D4F5B" (player) - To: "0x0" - ItemId: "0x37B9" - ItemIdName: "glow" - FromLocation: "(1149 808, 32)" - ToLocation: "(1149 808, 32)" - Speed: "10" - Duration: "5" - FixedDirection: "True" - Explode: "False"
                */
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070837); // The creature lands another blow in your weakened state.
                }
                else
                    defender.SendLocalizedMessage(1070836); // The blow from the creature's claws has made you more susceptible to physical attacks.

                int effect = -(defender.PhysicalResistance * 15 / 100);

                ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, effect);

                defender.FixedEffect(0x37B9, 10, 5);
                defender.AddResistanceMod(mod);

                timer = new ExpireTimer(defender, mod, TimeSpan.FromSeconds(5.0));
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
                Timer.DelayCall(TimeSpan.Zero, delegate { this.Hue = GetHue(); });

            if (version <= 1)
                Timer.DelayCall(TimeSpan.Zero, delegate
                {
                    if (this.InternalItem != null)
                    {
                        this.InternalItem.Hue = this.Hue;
                    }
                });

            if (version < 2)
            {
                for (int i = 0; i < this.Skills.Length; ++i)
                {
                    this.Skills[i].Cap = Math.Max(100.0, this.Skills[i].Cap * 0.9);

                    if (this.Skills[i].Base > this.Skills[i].Cap)
                    {
                        this.Skills[i].Base = this.Skills[i].Cap;
                    }
                }
            }
        }

        private static int GetHue()
        {
            int rand = Utility.Random(1075);

            /*
            1000	1075	No Hue Color	93.02%	0x0
            * 
            10	1075	Ice Green    	0.93%	0x847F
            10	1075	Light Blue    	0.93%	0x848D
            10	1075	Strong Cyan		0.93%	0x8495
            10	1075	Agapite			0.93%	0x8899
            10	1075	Gold			0.93%	0x8032
            * 
            8	1075	Blue and Yellow	0.74%	0x8487
            * 
            5	1075	Ice Blue       	0.47%	0x8482
            * 
            3	1075	Cyan			0.28%	0x8123
            3	1075	Light Green		0.28%	0x8295
            * 
            2	1075	Strong Yellow	0.19%	0x8037
            2	1075	Green			0.19%	0x8030	//this one is an approximation
            * 
            1	1075	Strong Purple	0.09%	0x8490
            1	1075	Strong Green	0.09%	0x855C
            * */

            if (rand <= 0)
                return 0x855C;
            else if (rand <= 1)
                return 0x8490;
            else if (rand <= 3)
                return 0x8030;
            else if (rand <= 5)
                return 0x8037;
            else if (rand <= 8)
                return 0x8295;
            else if (rand <= 11)
                return 0x8123;
            else if (rand <= 16)
                return 0x8482;
            else if (rand <= 24)
                return 0x8487;
            else if (rand <= 34)
                return 0x8032;
            else if (rand <= 44)
                return 0x8899;
            else if (rand <= 54)
                return 0x8495;
            else if (rand <= 64)
                return 0x848D;
            else if (rand <= 74)
                return 0x847F;
			
            return 0;
        }

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly ResistanceMod m_Mod;
            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;
                this.m_Mod = mod;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.m_Mobile.RemoveResistanceMod(this.m_Mod);
                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            protected override void OnTick()
            {
                this.m_Mobile.SendLocalizedMessage(1070838); // Your resistance to physical attacks has returned.
                this.DoExpire();
            }
        }
    }
}