using System;

namespace Server.Items
{
    [Flipable(0xE80, 0x9A8)]
    public class ShipsStrongbox : LockableContainer
    {
        [Constructable]
        public ShipsStrongbox()
            : this(Utility.RandomMinMax(1, 3))
        {
        }

        [Constructable]
        public ShipsStrongbox(int level)
            : base(0xE80)
        {
            Hue = level >= 4 ? 0x481 : 0x836;
            level = Math.Min(4, Math.Max(1, level));

            DropItem(new Gold(10000 * level));

            for (int i = 0; i < level * 20; i++)
            {
                DropItemStacked(Loot.RandomGem());
            }

            for (int i = 0; i < (level * 5) + Utility.Random(5); i++)
            {
                switch (Utility.Random(8))
                {
                    case 0: DropItemStacked(new BlueDiamond()); break;
                    case 1: DropItemStacked(new FireRuby()); break;
                    case 2: DropItemStacked(new BrilliantAmber()); break;
                    case 3: DropItemStacked(new PerfectEmerald()); break;
                    case 4: DropItemStacked(new DarkSapphire()); break;
                    case 5: DropItemStacked(new Turquoise()); break;
                    case 6: DropItemStacked(new EcruCitrine()); break;
                    case 7: DropItemStacked(new WhitePearl()); break;
                }
            }

            if (Utility.RandomBool())
            {
                DropItem(new GoldIngot(level * 100));
            }
            else
            {
                DropItem(new CopperIngot(level * 100));
            }
        }

        public ShipsStrongbox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1149959; // A ship's strongbox
        public override int DefaultMaxWeight => 400;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}