using System;

namespace Server.Items
{
    public class LargeGargoyleBedSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new LargeGargoyleBedSouthDeed();
            }
        }

        #region Mondain's Legacy
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        #endregion

        [Constructable]
        public LargeGargoyleBedSouthAddon()
        { 
            //Left Side
            this.AddComponent(new AddonComponent(0x4010), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x4013), 0, 1, 0);
            this.AddComponent(new AddonComponent(0x4016), 0, 2, 0);
            //Middle
            this.AddComponent(new AddonComponent(0x4011), 1, 0, 0);
            this.AddComponent(new AddonComponent(0x4014), 1, 1, 0);
            this.AddComponent(new AddonComponent(0x4017), 1, 2, 0);
            //Right Side
            this.AddComponent(new AddonComponent(0x4012), 2, 0, 0);
            this.AddComponent(new AddonComponent(0x4015), 2, 1, 0);
            this.AddComponent(new AddonComponent(0x4018), 2, 2, 0);
        }

        public LargeGargoyleBedSouthAddon(Serial serial)
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

    public class LargeGargoyleBedSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new LargeGargoyleBedSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1111761;
            }
        }// large gargish bed (south)

        [Constructable]
        public LargeGargoyleBedSouthDeed()
        {
        }

        public LargeGargoyleBedSouthDeed(Serial serial)
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