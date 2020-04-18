namespace Server.Items
{
    [TypeAlias("Server.Items.goldcarpetAddon")]
    public class GoldCarpetAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new goldcarpetAddonDeed();

        [Constructable]
        public GoldCarpetAddon()
        {
            AddonComponent ac = null;
            ac = new AddonComponent(2779);
            AddComponent(ac, 2, 2, 0);
            ac = new AddonComponent(2785);
            AddComponent(ac, 2, 1, 0);
            ac = new AddonComponent(2785);
            AddComponent(ac, 2, 0, 0);
            ac = new AddonComponent(2786);
            AddComponent(ac, 1, 2, 0);
            ac = new AddonComponent(2786);
            AddComponent(ac, 0, 2, 0);
            ac = new AddonComponent(2781);
            AddComponent(ac, -2, 2, 0);
            ac = new AddonComponent(2786);
            AddComponent(ac, -1, 2, 0);
            ac = new AddonComponent(2782);
            AddComponent(ac, 2, -2, 0);
            ac = new AddonComponent(2783);
            AddComponent(ac, -2, 1, 0);
            ac = new AddonComponent(2783);
            AddComponent(ac, -2, 0, 0);
            ac = new AddonComponent(2783);
            AddComponent(ac, -2, -1, 0);
            ac = new AddonComponent(2784);
            AddComponent(ac, 1, -2, 0);
            ac = new AddonComponent(2784);
            AddComponent(ac, 0, -2, 0);
            ac = new AddonComponent(2784);
            AddComponent(ac, -1, -2, 0);
            ac = new AddonComponent(2785);
            AddComponent(ac, 2, -1, 0);
            ac = new AddonComponent(2780);
            AddComponent(ac, -2, -2, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, -1, -1, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, -1, 0, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, -1, 1, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, 0, 1, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, 0, 0, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, 0, -1, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, 1, -1, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, 1, 0, 0);
            ac = new AddonComponent(2778);
            AddComponent(ac, 1, 1, 0);

        }

        public GoldCarpetAddon(Serial serial) : base(serial)
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

    public class goldcarpetAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new GoldCarpetAddon();

        [Constructable]
        public goldcarpetAddonDeed()
        {
            Name = "goldcarpet";
        }

        public goldcarpetAddonDeed(Serial serial) : base(serial)
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