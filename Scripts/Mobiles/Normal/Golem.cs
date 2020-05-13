using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a golem corpse")]
    public class Golem : BaseCreature, IRepairableMobile
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Type RepairResource => typeof(IronIngot);

        public double Scalar(Mobile m)
        {
            double scalar;

            double skill = m.Skills[SkillName.Tinkering].Value;

            if (skill >= 100.0)
                scalar = 1.0;
            else if (skill >= 90.0)
                scalar = 0.9;
            else if (skill >= 80.0)
                scalar = 0.8;
            else if (skill >= 70.0)
                scalar = 0.7;
            else
                scalar = 0.6;

            return scalar;
        }

        [Constructable]
        public Golem()
            : this(false, 1)
        {
        }

        [Constructable]
        public Golem(bool summoned, double scalar)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            Name = "a golem";
            Body = 752;

            SetStr((int)(251 * scalar), (int)(350 * scalar));
            SetDex((int)(76 * scalar), (int)(100 * scalar));
            SetInt((int)(101 * scalar), (int)(150 * scalar));

            if (summoned)
            {
                Hue = 2101;

                SetResistance(ResistanceType.Fire, 50, 65);
                SetResistance(ResistanceType.Poison, 75, 85);

                SetSkill(SkillName.MagicResist, (150.1 * scalar), (190.0 * scalar));
                SetSkill(SkillName.Tactics, (60.1 * scalar), (100.0 * scalar));
                SetSkill(SkillName.Wrestling, (60.1 * scalar), (100.0 * scalar));

                Fame = 10;
                Karma = 10;
            }
            else
            {
                SetHits(151, 210);

                SetResistance(ResistanceType.Fire, 100);
                SetResistance(ResistanceType.Poison, 10, 25);

                SetSkill(SkillName.MagicResist, 60.0, 100.0);
                SetSkill(SkillName.Tactics, 60.0, 100.0);
                SetSkill(SkillName.Wrestling, 150.0, 190.0);
                SetSkill(SkillName.DetectHidden, 45.0, 50.0);

                Fame = 3500;
                Karma = -3500;
            }

            SetDamage(13, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 45);

            ControlSlots = 3;

            SetSpecialAbility(SpecialAbility.ColossalBlow);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootItem<IronIngot>(Utility.RandomMinMax(13, 21), true));
            AddLoot(LootPack.LootItem<PowerCrystal>(1.0));
            AddLoot(LootPack.LootItem<ClockworkAssembly>(15.0));
            AddLoot(LootPack.LootItem<ArcaneGem>(20.0, 1, false, true));
            AddLoot(LootPack.LootItem<Gears>(25.0));

            AddLoot(LootPack.LootItemCallback(SpawnGears, 5.0, 1, false, false));
        }

        public static Item SpawnGears(IEntity e)
        {
            if (!(e is BaseCreature) || !((BaseCreature)e).IsParagon)
            {
                if (0.75 > Utility.RandomDouble())
                {
                    return DawnsMusicGear.RandomCommon;
                }
                else
                {
                    return DawnsMusicGear.RandomUncommon;
                }
            }
            else
            {
                return DawnsMusicGear.RandomRare;
            }
        }

        public Golem(Serial serial)
            : base(serial)
        {
        }

        public override bool IsScaredOfScaryThings => false;
        public override bool IsScaryToPets => !Controlled;
        public override bool IsBondable => false;
        public override FoodType FavoriteFood => FoodType.None;
        public override bool DeleteOnRelease => true;
        public override bool AutoDispel => !Controlled;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override int GetAngerSound()
        {
            return 541;
        }

        public override int GetIdleSound()
        {
            if (!Controlled)
                return 542;

            return base.GetIdleSound();
        }

        public override int GetDeathSound()
        {
            if (!Controlled)
                return 545;

            return base.GetDeathSound();
        }

        public override int GetAttackSound()
        {
            return 562;
        }

        public override int GetHurtSound()
        {
            if (Controlled)
                return 320;

            return base.GetHurtSound();
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Controlled || Summoned)
            {
                Mobile master = (ControlMaster);

                if (master == null)
                    master = SummonMaster;

                if (master != null && master.Player && master.Map == Map && master.InRange(Location, 20))
                {
                    if (master.Mana >= amount)
                    {
                        master.Mana -= amount;
                    }
                    else
                    {
                        amount -= master.Mana;
                        master.Mana = 0;
                        master.Damage(amount);
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
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
        }
    }
}
