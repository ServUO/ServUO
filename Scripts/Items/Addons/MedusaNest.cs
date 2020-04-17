namespace Server.Items
{
    public class MedusaSNestAddon : BaseAddon
    {
        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
            { 7045, -1, 1, 0 }, { 12795, 0, 0, 0 }, { 7054, 1, 0, 0 }// 1	2	3	
            ,
            { 7042, 1, 1, 0 }, { 7046, -1, -1, 0 }, { 7054, -1, 0, 0 }// 4	5	6	
            ,
            { 7065, 1, -1, 0 }, { 7054, 0, 1, 0 }, { 7054, 0, -1, 0 }// 7	8	9	
        };
        [Constructable]
        public MedusaSNestAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public MedusaSNestAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MedusaSNestAddonDeed();
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

    public class MedusaSNestAddonDeed : BaseAddonDeed
    {
        [Constructable]
        public MedusaSNestAddonDeed()
        {
            Name = "MedusaSNest";
        }

        public MedusaSNestAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MedusaSNestAddon();
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