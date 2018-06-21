using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a chicken lizard corpse")]
    public class ChickenLizard : BaseCreature
    {
        private DateTime m_NextEgg;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextEgg { get { return m_NextEgg; } }

        [Constructable]
        public ChickenLizard()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a chicken lizard";
            Body = 716;

            SetStr(74, 95);
            SetDex(78, 95);
            SetInt(6, 10);

            SetHits(74, 95);
			SetMana(6, 10);
			SetStam(78, 95);

            SetDamage(2, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 15);

            SetSkill(SkillName.MagicResist, 25.1, 29.6);
            SetSkill(SkillName.Tactics, 30.1, 44.9);
            SetSkill(SkillName.Wrestling, 26.2, 38.2);

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 0.0;

            if (0.05 > Utility.RandomDouble())
                PackItem(new ChickenLizardEgg());

            m_NextEgg = DateTime.UtcNow + TimeSpan.FromDays(7);
        }

        public override int Meat { get { return 3; } }
        public override MeatType MeatType { get { return MeatType.Bird; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override int GetIdleSound() { return 1511; }
        public override int GetAngerSound() { return 1508; }
        public override int GetHurtSound() { return 1510; }
        public override int GetDeathSound() { return 1509; }

        public override bool CheckFeed(Mobile from, Item dropped)
        {
            if (from.Map == null || from.Map == Map.Internal)
                return false;

            bool fed = base.CheckFeed(from, dropped);

            if (fed && DateTime.UtcNow >= m_NextEgg)
            {
                if (Utility.RandomBool())
                {
                    ChickenLizardEgg egg = new ChickenLizardEgg();

                    if (from.Backpack == null || from.Backpack.TryDropItem(from, egg, false))
                        egg.MoveToWorld(from.Location, from.Map);
                }

                m_NextEgg = DateTime.UtcNow + TimeSpan.FromDays(7);
            }

            return fed;
        }

        public ChickenLizard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_NextEgg);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
            {
                m_NextEgg = reader.ReadDateTime();
            }
        }
    }
}