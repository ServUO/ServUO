using System;

namespace Server.Items
{
    public class UpholsteredChairDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1154173; } } // Upholstered Chair       

        [Constructable]
        public UpholsteredChairDeed()
        {
        }

        public override BaseAddon Addon { get { return new UpholsteredChairAddon(); } }

        public UpholsteredChairDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class UpholsteredChairAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new UpholsteredChairDeed(); } }        
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public UpholsteredChairAddon()
        { 
            this.AddComponent(new AddonComponent(0x4C80), 0, 0, 0);
        }

        public UpholsteredChairAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }    
}