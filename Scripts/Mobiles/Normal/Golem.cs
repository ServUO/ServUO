using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a golem corpse")]
    public class Golem : BaseCreature, IRepairableMobile
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Type RepairResource
        {
            get
            {
                return typeof(IronIngot);
            }
        }

        [Constructable]
        public Golem()
            : this(false, 1.0)
        {
        }

        [Constructable]
        public Golem(bool summoned, double scalar)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            Name = "a golem";
            Body = 752;

            if (summoned)
                Hue = 2101;

            SetStr((int)(251 * scalar), (int)(350 * scalar));
            SetDex((int)(76 * scalar), (int)(100 * scalar));
            SetInt((int)(101 * scalar), (int)(150 * scalar));

            SetHits((int)(151 * scalar), (int)(210 * scalar));

            SetDamage((int)(13 * scalar), (int)(24 * scalar));

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, (int)(35 * scalar), (int)(55 * scalar));

            if (summoned)
                SetResistance(ResistanceType.Fire, (int)(50 * scalar), (int)(60 * scalar));
            else
                SetResistance(ResistanceType.Fire, (int)(100 * scalar));

            SetResistance(ResistanceType.Cold, (int)(10 * scalar), (int)(30 * scalar));
            SetResistance(ResistanceType.Poison, (int)(10 * scalar), (int)(25 * scalar));
            SetResistance(ResistanceType.Energy, (int)(30 * scalar), (int)(40 * scalar));

            SetSkill(SkillName.MagicResist, (150.1 * scalar), (190.0 * scalar));
            SetSkill(SkillName.Tactics, (60.1 * scalar), (100.0 * scalar));
            SetSkill(SkillName.Wrestling, (60.1 * scalar), (100.0 * scalar));

            if (summoned)
            {
                Fame = 10;
                Karma = 10;
            }
            else
            {
                Fame = 3500;
                Karma = -3500;
            }

            if (!summoned)
            {
                PackItem(new IronIngot(Utility.RandomMinMax(13, 21)));

                if (0.1 > Utility.RandomDouble())
                    PackItem(new PowerCrystal());

                if (0.15 > Utility.RandomDouble())
                    PackItem(new ClockworkAssembly());

                if (0.2 > Utility.RandomDouble())
                    PackItem(new ArcaneGem());

                if (0.25 > Utility.RandomDouble())
                    PackItem(new Gears());
            }

            ControlSlots = 3;
        }

        public Golem(Serial serial)
            : base(serial)
        {
        }

        public override bool IsScaredOfScaryThings
        {
            get
            {
                return false;
            }
        }
        public override bool IsScaryToPets
        {
            get
            {
                return true;
            }
        }
        public override bool IsBondable
        {
            get
            {
                return false;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.None;
            }
        }
        public override bool CanBeDistracted
        {
            get
            {
                return false;
            }
        }
        public override bool DeleteOnRelease
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return !Controlled;
            }
        }
        public override bool BleedImmune
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
                return !Core.AOS || Controlled;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }

        public override bool DoesColossalBlow { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.05 > Utility.RandomDouble())
            {
                if (!IsParagon)
                {
                    if (0.75 > Utility.RandomDouble())
                        c.DropItem(DawnsMusicGear.RandomCommon);
                    else
                        c.DropItem(DawnsMusicGear.RandomUncommon);
                }
                else
                    c.DropItem(DawnsMusicGear.RandomRare);
            }
        }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
