using System;

namespace Server.Mobiles
{
    [CorpseName("a vollem corpse")]
    public class Vollem : BaseCreature, IRepairableMobile
    {
        public virtual Type RepairResource => IsMechanical ? typeof(Items.IronIngot) : typeof(Items.Bandage);
        public virtual bool IsMechanical => true;

        [Constructable]
        public Vollem()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vollem";
            Body = 0x125;

            SetStr(496, 524);
            SetDex(88, 105);
            SetInt(94, 117);

            SetHits(300, 315);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 40);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 90.4, 97.8);
            SetSkill(SkillName.Tactics, 99.0, 99.5);
            SetSkill(SkillName.Wrestling, 84.2, 87.7);
            SetSkill(SkillName.EvalInt, 21.9, 43.3);
            SetSkill(SkillName.Magery, 25.6, 38.6);

            ControlSlots = 2;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override bool IsScaredOfScaryThings => false;
        public override bool IsScaryToPets => true;
        public override bool IsBondable => false;
        public override bool DeleteOnRelease => true;
        public override bool AutoDispel => !Controlled;
        public override bool BleedImmune => true;
        public override bool BardImmune => Controlled;
        public override Poison PoisonImmune => Poison.Lethal;

        public override FoodType FavoriteFood => FoodType.Meat;
        public override int Meat => 5;
        public override int Hides => 10;

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
            if (IsMechanical && (Controlled || Summoned))
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

        public Vollem(Serial serial)
            : base(serial)
        {
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
