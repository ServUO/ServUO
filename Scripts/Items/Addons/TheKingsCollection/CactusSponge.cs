namespace Server.Items
{
    public class CactusSpongeAddon : BaseAddon, IDyable
    {
        [Constructable]
        public CactusSpongeAddon()
        {
            AddComponent(new AddonComponent(0x4C2E), 0, 0, 0);
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public CactusSpongeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CactusSpongeDeed();

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

    public class CactusSpongeDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1098374;  // Cactus Sponge

        [Constructable]
        public CactusSpongeDeed()
        {
            LootType = LootType.Blessed;
        }

        public CactusSpongeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new CactusSpongeAddon();

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
