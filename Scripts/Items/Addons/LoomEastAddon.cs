namespace Server.Items
{
    public interface ILoom
    {
        int Phase { get; set; }
    }

    public class LoomEastAddon : BaseAddon, ILoom
    {
        private int m_Phase;

        [Constructable]
        public LoomEastAddon()
        {
            AddComponent(new AddonComponent(0x1060), 0, 0, 0);
            AddComponent(new AddonComponent(0x105F), 0, 1, 0);
        }

        public LoomEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new LoomEastDeed();
        public int Phase
        {
            get
            {
                return m_Phase;
            }
            set
            {
                m_Phase = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_Phase);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Phase = reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class LoomEastDeed : BaseAddonDeed
    {
        [Constructable]
        public LoomEastDeed()
        {
        }

        public LoomEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new LoomEastAddon();
        public override int LabelNumber => 1044343;// loom (east)
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