namespace Server.Items
{
    public class FluffySpongeAddon : BaseAddon, IDyable
    {
        [Constructable]
        public FluffySpongeAddon()
        {
            AddComponent(new AddonComponent(0x4C31), 0, 0, 0);
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public FluffySpongeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new FluffySpongeDeed();

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

    public class FluffySpongeDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1098377;  // Fluffy Sponge

        [Constructable]
        public FluffySpongeDeed()
        {
            LootType = LootType.Blessed;
        }

        public FluffySpongeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new FluffySpongeAddon();

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
