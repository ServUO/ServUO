using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a rune beetle corpse")]
    public class RuneBeetle : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public RuneBeetle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a rune beetle";
            this.Body = 244;

            this.SetStr(401, 460);
            this.SetDex(121, 170);
            this.SetInt(376, 450);

            this.SetHits(301, 360);

            this.SetDamage(15, 22);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Poison, 10);
            this.SetDamageType(ResistanceType.Energy, 70);

            this.SetResistance(ResistanceType.Physical, 40, 65);
            this.SetResistance(ResistanceType.Fire, 35, 50);
            this.SetResistance(ResistanceType.Cold, 35, 50);
            this.SetResistance(ResistanceType.Poison, 75, 95);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.EvalInt, 100.1, 125.0);
            this.SetSkill(SkillName.Magery, 100.1, 110.0);
            this.SetSkill(SkillName.Poisoning, 120.1, 140.0);
            this.SetSkill(SkillName.MagicResist, 95.1, 110.0);
            this.SetSkill(SkillName.Tactics, 78.1, 93.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 77.5);

            this.Fame = 15000;
            this.Karma = -15000;
			
            if (Utility.RandomDouble() < .25)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
				
            switch ( Utility.Random(10))
            {
                case 0:
                    this.PackItem(new LeftArm());
                    break;
                case 1:
                    this.PackItem(new RightArm());
                    break;
                case 2:
                    this.PackItem(new Torso());
                    break;
                case 3:
                    this.PackItem(new Bone());
                    break;
                case 4:
                    this.PackItem(new RibCage());
                    break;
                case 5:
                    this.PackItem(new RibCage());
                    break;
                case 6:
                    this.PackItem(new BonePile());
                    break;
                case 7:
                    this.PackItem(new BonePile());
                    break;
                case 8:
                    this.PackItem(new BonePile());
                    break;
                case 9:
                    this.PackItem(new BonePile());
                    break;
            }
				
            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 93.9;			
        }

        public RuneBeetle(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
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
            return WeaponAbility.BleedAttack;
        }

        public override int GetAngerSound()
        {
            return 0x4E8;
        }

        public override int GetIdleSound()
        {
            return 0x4E7;
        }

        public override int GetAttackSound()
        {
            return 0x4E6;
        }

        public override int GetHurtSound()
        {
            return 0x4E9;
        }

        public override int GetDeathSound()
        {
            return 0x4E5;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.MedScrolls, 1);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.05 > Utility.RandomDouble())
            {
                /* Rune Corruption
                * Start cliloc: 1070846 "The creature magically corrupts your armor!"
                * Effect: All resistances -70 (lowest 0) for 5 seconds
                * End ASCII: "The corruption of your armor has worn off"
                */
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070845); // The creature continues to corrupt your armor!
                }
                else
                    defender.SendLocalizedMessage(1070846); // The creature magically corrupts your armor!

                List<ResistanceMod> mods = new List<ResistanceMod>();

                if (Core.ML)
                {
                    if (defender.PhysicalResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Physical, -(defender.PhysicalResistance / 2)));

                    if (defender.FireResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Fire, -(defender.FireResistance / 2)));

                    if (defender.ColdResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Cold, -(defender.ColdResistance / 2)));

                    if (defender.PoisonResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Poison, -(defender.PoisonResistance / 2)));

                    if (defender.EnergyResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Energy, -(defender.EnergyResistance / 2)));
                }
                else
                {
                    if (defender.PhysicalResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Physical, (defender.PhysicalResistance > 70) ? -70 : -defender.PhysicalResistance));

                    if (defender.FireResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Fire, (defender.FireResistance > 70) ? -70 : -defender.FireResistance));

                    if (defender.ColdResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Cold, (defender.ColdResistance > 70) ? -70 : -defender.ColdResistance));

                    if (defender.PoisonResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Poison, (defender.PoisonResistance > 70) ? -70 : -defender.PoisonResistance));

                    if (defender.EnergyResistance > 0)
                        mods.Add(new ResistanceMod(ResistanceType.Energy, (defender.EnergyResistance > 70) ? -70 : -defender.EnergyResistance));
                }

                for (int i = 0; i < mods.Count; ++i)
                    defender.AddResistanceMod(mods[i]);

                defender.FixedEffect(0x37B9, 10, 5);

                timer = new ExpireTimer(defender, mods, TimeSpan.FromSeconds(5.0));
                timer.Start();

                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.RuneBeetleCorruption, 1153796, 1153823, TimeSpan.FromSeconds(5.0), defender, String.Format("{0}\t{1}\t{2}\t{3}\t{4}", mods[0], mods[1], mods[2], mods[3], mods[4] )));

                m_Table[defender] = timer;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
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

        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly List<ResistanceMod> m_Mods;
            public ExpireTimer(Mobile m, List<ResistanceMod> mods, TimeSpan delay)
                : base(delay)
            {
                this.m_Mobile = m;
                this.m_Mods = mods;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                for (int i = 0; i < this.m_Mods.Count; ++i)
                    this.m_Mobile.RemoveResistanceMod(this.m_Mods[i]);

                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            protected override void OnTick()
            {
                this.m_Mobile.SendMessage("The corruption of your armor has worn off");
                this.DoExpire();
            }
        }
    }
}