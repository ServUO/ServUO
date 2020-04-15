namespace Server.Items
{
    public class CousteauPerronAddon : BaseAddon
    {
        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {2602, 1, 2, 0}, {7828, 1, 3, 3}, {4602, 0, 1, 0},
              {4601, -1, 1, 0}, {1560, 0, 0, 26}, {1560, 1, 0, 26},
              {1560, 2, 0, 26}, {1537, 0, 1, 14}, {1537, -1, 1, 14},
              {1537, 1, 1, 14}, {1544, 1, 0, 22}, {1544, -1, 0, 22},
              {1544, 0, 0, 22}, {740, 0, -2, 0}, {740, -1, -2, 0},
              {741, -2, -1, 0}, {741, -2, 0, 0}, {738, -2, 1, 0},
              {734, 1, 1, 0}, {736, 1, 0, 0}, {736, 1, -1, 0},
              {739, 1, -2, 0}
        };

        [Constructable]
        public CousteauPerronAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComponent(new AddonComponent(6571), 1, 3, 3);
            AddComponent(new AddonComponent(4012), 1, 3, 0);
        }

        public CousteauPerronAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}