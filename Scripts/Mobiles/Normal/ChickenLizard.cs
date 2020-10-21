using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a chicken lizard corpse")]
    public class ChickenLizard : BaseCreature
    {
        private DateTime m_NextEgg;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextEgg => m_NextEgg;

        [Constructable]
        public ChickenLizard()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
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
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootItem<ChickenLizardEgg>(5.0));
        }

        public override int Meat => 3;
        public override MeatType MeatType => MeatType.Bird;
        public override FoodType FavoriteFood => FoodType.Meat;

        public override int GetIdleSound() { return 1511; }
        public override int GetAngerSound() { return 1508; }
        public override int GetHurtSound() { return 1510; }
        public override int GetDeathSound() { return 1509; }

        public override bool CheckFeed(Mobile from, Item dropped)
        {
            if (from.Map == null || from.Map == Map.Internal)
                return false;

            bool isBonded = IsBonded;
            bool fed = base.CheckFeed(from, dropped);

            if (!isBonded && IsBonded)
            {
                m_NextEgg = DateTime.UtcNow + TimeSpan.FromDays(1);
            }

            if (IsBonded && fed && DateTime.UtcNow >= m_NextEgg)
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
            writer.Write(1);

            writer.Write(m_NextEgg);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_NextEgg = reader.ReadDateTime();
                    break;
            }
        }
    }
}
