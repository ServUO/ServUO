namespace Server.Items
{
    public class ArcaneCircleAddon : BaseAddon
    {
        [Constructable]
        public ArcaneCircleAddon()
        {
            AddComponent(new AddonComponent(0x3083), -1, -1, 0);
            AddComponent(new AddonComponent(0x3080), -1, 0, 0);
            AddComponent(new AddonComponent(0x3082), 0, -1, 0);
            AddComponent(new AddonComponent(0x3081), 1, -1, 0);
            AddComponent(new AddonComponent(0x307D), -1, 1, 0);
            AddComponent(new AddonComponent(0x307F), 0, 0, 0);
            AddComponent(new AddonComponent(0x307E), 1, 0, 0);
            AddComponent(new AddonComponent(0x307C), 0, 1, 0);
            AddComponent(new AddonComponent(0x307B), 1, 1, 0);
        }

        public ArcaneCircleAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ArcaneCircleDeed();
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
            foreach (AddonComponent c in Components)
            {
                if (c.ItemID == 0x3083)
                {
                    c.Offset = new Point3D(-1, -1, 0);
                    c.MoveToWorld(new Point3D(X + c.Offset.X, Y + c.Offset.Y, Z + c.Offset.Z), Map);
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

        public override BaseAddon Addon => new ArcaneCircleAddon();
        public override int LabelNumber => 1072703;// arcane circle
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