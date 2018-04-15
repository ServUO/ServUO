using System;

namespace Server.Mobiles
{
    [CorpseName("a clockwork scorpion corpse")]
    public class ClockworkScorpion : BaseCreature, IRepairableMobile
    {
        public Type RepairResource { get { return typeof(Server.Items.IronIngot); } }

        [Constructable]
        public ClockworkScorpion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            Name = "a clockwork scorpion";
            Body = 717;

            SetStr(225, 245);
            SetDex(80, 100);
            SetInt(30, 40);

            SetHits(151, 210);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 80, 100);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 60, 80);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 10, 25);

            SetSkill(SkillName.MagicResist, 30.1, 50.0);
            SetSkill(SkillName.Poisoning, 95.1, 100.0);
            SetSkill(SkillName.Tactics, 70.1, 90.0);
            SetSkill(SkillName.Wrestling, 50.1, 80.0);

            Fame = 3500;
            Karma = -3500;

            ControlSlots = 1;
        }

        public ClockworkScorpion(Serial serial)
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
                return FoodType.Meat;
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
        public override bool DeleteOnRelease
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
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager, 2);
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
            Mobile master = GetMaster();

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

            base.OnDamage(amount, from, willKill);
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

            if (version == 0 && (AbilityProfile == null || AbilityProfile.MagicalAbility == MagicalAbility.None))
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }
        }
    }
}