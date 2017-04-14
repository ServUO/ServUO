using System;
using Server;

namespace Server.Items
{
    public class NexusComponent : AddonComponent
    {
        public override int LabelNumber { get { return 1152442; } } // Nexus

        public NexusComponent(int itemID)
            : base(itemID)
        {
        }

        public NexusComponent(Serial serial)
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

    public class NexusAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new NexusAddonDeed(); } }

        [Constructable]
        public NexusAddon()
        {
            this.AddComponent(new NexusComponent(19324), -1, 1, 0);
            this.AddComponent(new NexusComponent(19326), 0, 0, 0);
            this.AddComponent(new NexusComponent(19319), 1, 2, 0);
            this.AddComponent(new NexusComponent(19321), 2, 1, 0);
            this.AddComponent(new NexusComponent(19319), 1, 2, 0);
            this.AddComponent(new NexusComponent(19323), 0, 1, 0);
            this.AddComponent(new NexusComponent(19327), 1, -1, 0);
            this.AddComponent(new NexusComponent(19316), 1, 1, 0);
            this.AddComponent(new NexusComponent(19322), 0, -1, 0);
            this.AddComponent(new NexusComponent(19320), 2, 2, 0);
            this.AddComponent(new NexusComponent(19325), 1, 0, 0);
            this.AddComponent(new NexusComponent(19318), 0, 2, 0);
            this.AddComponent(new NexusComponent(19317), -1, 2, 0);
        }

        public NexusAddon(Serial serial) : base(serial)
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

    public class NexusAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new NexusAddon(); } }

        [Constructable]
        public NexusAddonDeed()
        {
            this.Name = "Nexus Deed";
        }

        public NexusAddonDeed(Serial serial) : base(serial)
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