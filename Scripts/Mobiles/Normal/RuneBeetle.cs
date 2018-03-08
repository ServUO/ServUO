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
            Name = "a rune beetle";
            Body = 244;

            SetStr(401, 460);
            SetDex(121, 170);
            SetInt(376, 450);

            SetHits(301, 360);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 10);
            SetDamageType(ResistanceType.Energy, 70);

            SetResistance(ResistanceType.Physical, 40, 65);
            SetResistance(ResistanceType.Fire, 35, 50);
            SetResistance(ResistanceType.Cold, 35, 50);
            SetResistance(ResistanceType.Poison, 75, 95);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 100.1, 125.0);
            SetSkill(SkillName.Magery, 100.1, 110.0);
            SetSkill(SkillName.Poisoning, 120.1, 140.0);
            SetSkill(SkillName.MagicResist, 95.1, 110.0);
            SetSkill(SkillName.Tactics, 78.1, 93.0);
            SetSkill(SkillName.Wrestling, 70.1, 77.5);

            Fame = 15000;
            Karma = -15000;

            if (Utility.RandomDouble() < .25)
                PackItem(Engines.Plants.Seed.RandomBonsaiSeed());

            PackBodyPartOrBones();

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 93.9;
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
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 1);
        }

        int phy, fire, cold, poison, energy;

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

                phy = 0; fire = 0; cold = 0; poison = 0; energy = 0;

                if (Core.ML)
                {
                    if (defender.PhysicalResistance > 0)
                    {
                        phy = defender.PhysicalResistance / 2;

                        mods.Add(new ResistanceMod(ResistanceType.Physical, -phy));
                    }

                    if (defender.FireResistance > 0)
                    {
                        fire = defender.FireResistance / 2;

                        mods.Add(new ResistanceMod(ResistanceType.Fire, -fire));
                    }

                    if (defender.ColdResistance > 0)
                    {
                        cold = defender.ColdResistance / 2;

                        mods.Add(new ResistanceMod(ResistanceType.Cold, -cold));
                    }

                    if (defender.PoisonResistance > 0)
                    {
                        poison = defender.PoisonResistance / 2;

                        mods.Add(new ResistanceMod(ResistanceType.Poison, -poison));
                    }

                    if (defender.EnergyResistance > 0)
                    {
                        energy = defender.EnergyResistance / 2;

                        mods.Add(new ResistanceMod(ResistanceType.Energy, -energy));
                    }
                }
                else
                {
                    if (defender.PhysicalResistance > 0)
                    {
                        phy = (defender.PhysicalResistance > 70) ? 70 : defender.PhysicalResistance;

                        mods.Add(new ResistanceMod(ResistanceType.Physical, -phy));
                    }

                    if (defender.FireResistance > 0)
                    {
                        fire = (defender.FireResistance > 70) ? 70 : defender.FireResistance;

                        mods.Add(new ResistanceMod(ResistanceType.Fire, -fire));
                    }

                    if (defender.ColdResistance > 0)
                    {
                        cold = (defender.ColdResistance > 70) ? 70 : defender.ColdResistance;

                        mods.Add(new ResistanceMod(ResistanceType.Cold, -cold));
                    }

                    if (defender.PoisonResistance > 0)
                    {
                        poison = (defender.PoisonResistance > 70) ? 70 : defender.PoisonResistance;

                        mods.Add(new ResistanceMod(ResistanceType.Poison, -poison));
                    }

                    if (defender.EnergyResistance > 0)
                    {
                        energy = (defender.EnergyResistance > 70) ? 70 : defender.EnergyResistance;

                        mods.Add(new ResistanceMod(ResistanceType.Energy, -energy));
                    }
                }

                for (int i = 0; i < mods.Count; ++i)
                    defender.AddResistanceMod(mods[i]);

                defender.FixedEffect(0x37B9, 10, 5);

                timer = new ExpireTimer(defender, mods, TimeSpan.FromSeconds(5.0));
                timer.Start();

                BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.RuneBeetleCorruption, 1153796, 1153823, TimeSpan.FromSeconds(5.0), defender, String.Format("{0}\t{1}\t{2}\t{3}\t{4}", phy, cold, poison, energy, fire)));

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
                for (int i = 0; i < Skills.Length; ++i)
                {
                    Skills[i].Cap = Math.Max(100.0, Skills[i].Cap * 0.9);

                    if (Skills[i].Base > Skills[i].Cap)
                    {
                        Skills[i].Base = Skills[i].Cap;
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
                m_Mobile = m;
                m_Mods = mods;
                Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                for (int i = 0; i < m_Mods.Count; ++i)
                    m_Mobile.RemoveResistanceMod(m_Mods[i]);

                Stop();
                m_Table.Remove(m_Mobile);
            }

            protected override void OnTick()
            {
                m_Mobile.SendMessage("The corruption of your armor has worn off");
                DoExpire();
            }
        }
    }
}
