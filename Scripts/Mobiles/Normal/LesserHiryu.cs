using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hiryu corpse")]
    public class LesserHiryu : BaseMount
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public LesserHiryu()
            : base("a lesser hiryu", 243, 0x3E94, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Hue = GetHue();

            this.SetStr(301, 410);
            this.SetDex(171, 270);
            this.SetInt(301, 325);

            this.SetHits(401, 600);
            this.SetMana(60);

            this.SetDamage(18, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 70);
            this.SetResistance(ResistanceType.Fire, 60, 80);
            this.SetResistance(ResistanceType.Cold, 5, 15);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 75.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 85.1, 100.0);
            this.SetSkill(SkillName.Tactics, 100.1, 110.0);
            this.SetSkill(SkillName.Wrestling, 100.1, 120.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 98.7;

            if (Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
        }

        public LesserHiryu(Serial serial)
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
                return 3;
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

        public override bool OverrideBondingReqs()
        {
            if (this.ControlMaster.Skills[SkillName.Bushido].Base >= 90.0)
                return true;
            return false;
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
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.Gems, 4);
        }

        public override double GetControlChance(Mobile m, bool useBaseSkill)
        {
            double tamingChance = base.GetControlChance(m, useBaseSkill);

            if (tamingChance >= 0.95)
            {
                return tamingChance;
            }

            double skill = (useBaseSkill ? m.Skills.Bushido.Base : m.Skills.Bushido.Value);

            if (skill < 90.0)
            {
                return tamingChance;
            }

            double bushidoChance = (skill - 30.0) / 100;

            if (m.Skills.Bushido.Base >= 120)
                bushidoChance += 0.05;

            return bushidoChance > tamingChance ? bushidoChance : tamingChance;
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
            int rand = Utility.Random(527);

            /*

            500	527	No Hue Color	94.88%	0
            10	527	Green			1.90%	0x8295
            10	527	Green			1.90%	0x8163	(Very Close to Above Green)	//this one is an approximation
            5	527	Dark Green		0.95%	0x87D4
            1	527	Valorite		0.19%	0x88AB
            1	527	Midnight Blue	0.19%	0x8258

            * */

            if (rand <= 0)
                return 0x8258;
            else if (rand <= 1)
                return 0x88AB;
            else if (rand <= 6)
                return 0x87D4;
            else if (rand <= 16)
                return 0x8163;
            else if (rand <= 26)
                return 0x8295;

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