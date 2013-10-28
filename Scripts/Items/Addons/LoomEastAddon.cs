using System;

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
            this.AddComponent(new AddonComponent(0x1060), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x105F), 0, 1, 0);
        }

        public LoomEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LoomEastDeed();
            }
        }
        public int Phase
        {
            get
            {
                return this.m_Phase;
            }
            set
            {
                this.m_Phase = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Phase);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Phase = reader.ReadInt();
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

        public override BaseAddon Addon
        {
            get
            {
                return new LoomEastAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044343;
            }
        }// loom (east)
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