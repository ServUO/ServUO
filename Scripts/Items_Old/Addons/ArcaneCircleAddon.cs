using System;

namespace Server.Items
{
    public class ArcaneCircleAddon : BaseAddon
    {
        [Constructable]
        public ArcaneCircleAddon()
        {
            this.AddComponent(new AddonComponent(0x3083), -1, -1, 0);
            this.AddComponent(new AddonComponent(0x3080), -1, 0, 0);
            this.AddComponent(new AddonComponent(0x3082), 0, -1, 0);
            this.AddComponent(new AddonComponent(0x3081), 1, -1, 0);
            this.AddComponent(new AddonComponent(0x307D), -1, 1, 0);
            this.AddComponent(new AddonComponent(0x307F), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x307E), 1, 0, 0);
            this.AddComponent(new AddonComponent(0x307C), 0, 1, 0);
            this.AddComponent(new AddonComponent(0x307B), 1, 1, 0);
        }

        public ArcaneCircleAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new ArcaneCircleDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
                ValidationQueue<ArcaneCircleAddon>.Add(this);
        }

        public void Validate()
        {
            foreach (AddonComponent c in this.Components)
            {
                if (c.ItemID == 0x3083)
                {
                    c.Offset = new Point3D(-1, -1, 0);
                    c.MoveToWorld(new Point3D(this.X + c.Offset.X, this.Y + c.Offset.Y, this.Z + c.Offset.Z), this.Map);
                }
            }
        }
    }

    public class ArcaneCircleDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcaneCircleDeed()
        {
        }

        public ArcaneCircleDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new ArcaneCircleAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1072703;
            }
        }// arcane circle
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