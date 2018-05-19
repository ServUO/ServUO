using System;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Items
{
    [FlipableAttribute(0xE80, 0x9A8)]
    public class ShipsStrongbox : LockableContainer
    {
        [Constructable]
        public ShipsStrongbox()
            : this (Utility.RandomMinMax(1, 3))
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
                DropItemStack(Loot.RandomGem());
            }

            for (int i = 0; i < (level * 5) + Utility.Random(5); i++)
            {
                switch (Utility.Random(8))
                {
                    case 0: DropItemStack(new BlueDiamond()); break;
                    case 1: DropItemStack(new FireRuby()); break;
                    case 2: DropItemStack(new BrilliantAmber()); break;
                    case 3: DropItemStack(new PerfectEmerald()); break;
                    case 4: DropItemStack(new DarkSapphire()); break;
                    case 5: DropItemStack(new Turquoise()); break;
                    case 6: DropItemStack(new EcruCitrine()); break;
                    case 7: DropItemStack(new WhitePearl()); break;
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

        public override int LabelNumber { get { return 1149959; } }// A ship's strongbox
        public override int DefaultMaxWeight { get { return 400; } }

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
}