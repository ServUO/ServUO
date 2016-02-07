using System;

namespace Server.Items
{
    public class Telescope : BaseAddon
    {
        [Constructable]
        public Telescope()
        {
            this.AddComponent(new AddonComponent(0x1494), 0, 5, 0);
            this.AddComponent(new AddonComponent(0x145B), 0, 6, 0);
            this.AddComponent(new AddonComponent(0x145A), 0, 7, 0);

            this.AddComponent(new AddonComponent(0x1495), 1, 4, 0);
            this.AddComponent(new AddonComponent(0x145C), 1, 7, 0);
            this.AddComponent(new AddonComponent(0x145D), 1, 8, 0);

            this.AddComponent(new AddonComponent(0x1496), 2, 3, 0);
            this.AddComponent(new AddonComponent(0x1499), 2, 4, 0);
            this.AddComponent(new AddonComponent(0x148E), 2, 6, 0);
            this.AddComponent(new AddonComponent(0x1493), 2, 7, 0);
            this.AddComponent(new AddonComponent(0x1492), 2, 8, 0);
            this.AddComponent(new AddonComponent(0x145E), 2, 9, 0);
            this.AddComponent(new AddonComponent(0x1459), 2, 10, 0);

            this.AddComponent(new AddonComponent(0x1497), 3, 2, 0);
            this.AddComponent(new AddonComponent(0x145F), 3, 9, 0);
            this.AddComponent(new AddonComponent(0x1461), 3, 10, 0);

            this.AddComponent(new AddonComponent(0x149A), 4, 1, 0);
            this.AddComponent(new AddonComponent(0x1498), 4, 2, 0);
            this.AddComponent(new AddonComponent(0x148F), 4, 4, 0);
            this.AddComponent(new AddonComponent(0x148D), 4, 6, 0);
            this.AddComponent(new AddonComponent(0x1488), 4, 8, 0);
            this.AddComponent(new AddonComponent(0x1460), 4, 9, 0);
            this.AddComponent(new AddonComponent(0x1462), 4, 10, 0);

            this.AddComponent(new AddonComponent(0x147D), 5, 0, 0);
            this.AddComponent(new AddonComponent(0x1490), 5, 4, 0);
            this.AddComponent(new AddonComponent(0x148B), 5, 5, 0);
            this.AddComponent(new AddonComponent(0x148A), 5, 6, 0);
            this.AddComponent(new AddonComponent(0x1486), 5, 7, 0);
            this.AddComponent(new AddonComponent(0x1485), 5, 8, 0);

            this.AddComponent(new AddonComponent(0x147C), 6, 0, 0);
            this.AddComponent(new AddonComponent(0x1491), 6, 4, 0);
            this.AddComponent(new AddonComponent(0x148C), 6, 5, 0);
            this.AddComponent(new AddonComponent(0x1489), 6, 6, 0);
            this.AddComponent(new AddonComponent(0x1487), 6, 7, 0);
            this.AddComponent(new AddonComponent(0x1484), 6, 8, 0);
            this.AddComponent(new AddonComponent(0x1463), 6, 10, 0);

            this.AddComponent(new AddonComponent(0x147B), 7, 0, 0);
            this.AddComponent(new AddonComponent(0x147F), 7, 3, 0);
            this.AddComponent(new AddonComponent(0x1480), 7, 4, 0);
            this.AddComponent(new AddonComponent(0x1482), 7, 5, 0);
            this.AddComponent(new AddonComponent(0x1469), 7, 6, 0);
            this.AddComponent(new AddonComponent(0x1468), 7, 7, 0);
            this.AddComponent(new AddonComponent(0x1465), 7, 8, 0);
            this.AddComponent(new AddonComponent(0x1464), 7, 9, 0);

            this.AddComponent(new AddonComponent(0x147A), 8, 0, 0);
            this.AddComponent(new AddonComponent(0x1479), 8, 1, 0);
            this.AddComponent(new AddonComponent(0x1477), 8, 2, 0);
            this.AddComponent(new AddonComponent(0x147E), 8, 3, 0);
            this.AddComponent(new AddonComponent(0x1481), 8, 4, 0);
            this.AddComponent(new AddonComponent(0x1483), 8, 5, 0);
            this.AddComponent(new AddonComponent(0x146A), 8, 6, 0);
            this.AddComponent(new AddonComponent(0x1467), 8, 7, 0);
            this.AddComponent(new AddonComponent(0x1466), 8, 8, 0);

            this.AddComponent(new AddonComponent(0x1478), 9, 1, 0);
            this.AddComponent(new AddonComponent(0x1475), 9, 2, 0);
            this.AddComponent(new AddonComponent(0x1474), 9, 3, 0);
            this.AddComponent(new AddonComponent(0x146F), 9, 4, 0);
            this.AddComponent(new AddonComponent(0x146E), 9, 5, 0);
            this.AddComponent(new AddonComponent(0x146D), 9, 6, 0);
            this.AddComponent(new AddonComponent(0x146B), 9, 7, 0);

            this.AddComponent(new AddonComponent(0x1476), 10, 2, 0);
            this.AddComponent(new AddonComponent(0x1473), 10, 3, 0);
            this.AddComponent(new AddonComponent(0x1470), 10, 4, 0);
            this.AddComponent(new AddonComponent(0x1471), 10, 5, 0);
            this.AddComponent(new AddonComponent(0x1472), 10, 6, 0);
        }

        public Telescope(Serial serial)
            : base(serial)
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
}