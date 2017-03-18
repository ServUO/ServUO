using System;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0xe41, 0xe40)]
    public class SorcerersRewardChest : BaseTreasureChestMod
    {
        public override int LabelNumber { get { return 1023712; } } // strong box

        [Constructable]
        public SorcerersRewardChest() : base(0x9AA)
        {
            this.Locked = true;
            this.Hue = 1912;
            TrapType = TrapType.MagicTrap;
            TrapPower = 4 * Utility.Random(1, 25);
            DropItem(new Gold(200, 400));
            DropItem(new BlankScroll(Utility.Random(1, 4)));

            for (int i = Utility.Random(1, 4); i > 1; i--)
            {
                Item ReagentLoot = Loot.RandomReagent();
                ReagentLoot.Amount = Utility.Random(6, 12);
                DropItem(ReagentLoot);
            }

            for (int i = Utility.Random(1, 4); i > 1; i--)
                DropItem(Loot.RandomPotion());

            if (0.75 > Utility.RandomDouble()) //75% chance = 3/4
                for (int i = Utility.RandomMinMax(8, 16); i > 0; i--)
                    DropItem(Loot.RandomScroll(0, 47, SpellbookType.Regular));

            if (0.75 > Utility.RandomDouble()) //75% chance = 3/4
                for (int i = Utility.RandomMinMax(6, 12) + 1; i > 0; i--)
                    DropItem(Loot.RandomGem());

            for (int i = Utility.Random(1, 4); i > 1; i--)
                DropItem(Loot.RandomWand());
            

            for (int i = Utility.Random(3) + 1; i > 0; i--) // random 1 to 3
            {
                switch (Utility.Random(3))
                {
                    case 0: DropItem(new Bolt(10)); break;
                    case 1: DropItem(new Bandage(TrapPower / 5)); break;
                    case 2: DropItem(new HealPotion(TrapPower / 5)); break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Locked)
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154226); // *It's an unassuming strong box. You examine the lock more closely and determine there is no way to pick it. You'll need to find a key.*
                return;
            }

            base.OnDoubleClick(from);
        }

        public SorcerersRewardChest(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class StrongboxKey : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154227; } } // Strongbox Key
        

        [Constructable]
        public StrongboxKey() : base(0x410A)
        {
            Stackable = false;
            Weight = 0.01;
            Hue = 2721;
            LootType = LootType.Blessed;
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public StrongboxKey(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                return;
            }

            from.Target = new ChestTarget(from, this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ChestTarget : Target
    {
        private static Mobile m_From;
        private static StrongboxKey m_Key;

        public ChestTarget(Mobile from, StrongboxKey key) : base(2, false, TargetFlags.None)
        {
            m_From = from;
            m_Key = key;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is SorcerersRewardChest)
            {
                SorcerersRewardChest chest = (SorcerersRewardChest)o;

                Container box = (Container)chest;

                box.DropItem(new SalvagerSuitPlans());

                m_From.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154228); // *You insert the key into the mechanism and turn it. To your delight the lock opens with a click and you remove the contents*

                chest.Locked = false;

                m_Key.Delete();
            }
        }
    }
}
