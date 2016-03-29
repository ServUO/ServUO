using System;

namespace Server.Items
{
    public class LoomSouthAddon : BaseAddon, ILoom
    {
        private int m_Phase;
        [Constructable]
        public LoomSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x1061), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1062), 1, 0, 0);
        }

        public LoomSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LoomSouthDeed();
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

    public class LoomSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public LoomSouthDeed()
        {
        }

        public LoomSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LoomSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044344;
            }
        }// loom (south)
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