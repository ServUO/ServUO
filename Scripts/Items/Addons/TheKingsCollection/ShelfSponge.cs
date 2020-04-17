namespace Server.Items
{
    public class ShelfSpongeAddon : BaseAddon, IDyable
    {
        [Constructable]
        public ShelfSpongeAddon()
        {
            AddComponent(new AddonComponent(0x4C2F), 0, 0, 0);
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public ShelfSpongeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ShelfSpongeDeed();

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

    public class ShelfSpongeDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1098375;  // Shelf Sponge

        [Constructable]
        public ShelfSpongeDeed()
        {
            LootType = LootType.Blessed;
        }

        public ShelfSpongeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ShelfSpongeAddon();

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
