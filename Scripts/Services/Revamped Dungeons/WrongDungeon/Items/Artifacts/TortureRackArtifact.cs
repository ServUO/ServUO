using Server;
using System;

namespace Server.Items
{
    public class TortureRackComponent : AddonComponent
    {
        public override int LabelNumber { get { return 1152307; } } // Torture Rack	
        public override bool IsArtifact { get { return true; } }
        public virtual bool ShowArtifactRarity { get { return true; } }

        public TortureRackComponent(int itemID)
            : base(itemID)
        {
        }

        public TortureRackComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }
        public virtual int ArtifactRarity { get { return 10; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (ShowArtifactRarity)
                list.Add(1061078, this.ArtifactRarity.ToString()); // artifact rarity ~1_val~
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

    public class TortureRackEast : BaseAddon
    {
        [Constructable]
        public TortureRackEast()
        {
            this.AddComponent(new TortureRackComponent(0x4AAB), 0, 0, 0);
            this.AddComponent(new TortureRackComponent(0x4AA3), 0, 1, 0);
            this.AddComponent(new TortureRackComponent(0x4AA2), 0, 2, 0);
        }

        public TortureRackEast(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new TortureRackEastDeed(); } }        

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

    public class TortureRackEastDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1152305; } }

        [Constructable]
        public TortureRackEastDeed()
        {
        }

        public TortureRackEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new TortureRackEast(); } }
        
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

    public class TortureRackSouth : BaseAddon
    {
        [Constructable]
        public TortureRackSouth()
        {
            this.AddComponent(new TortureRackComponent(0x4AA0), 0, 0, 0);
            this.AddComponent(new TortureRackComponent(0x4AA1), 1, 0, 0);
            this.AddComponent(new TortureRackComponent(0x4AAD), 2, 0, 0);
        }

        public TortureRackSouth(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new TortureRackSouthDeed(); } }

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

    public class TortureRackSouthDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1152306; } }

        [Constructable]
        public TortureRackSouthDeed()
        {
        }

        public TortureRackSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new TortureRackSouth(); } }

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