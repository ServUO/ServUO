namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class FireDemonStatueAddon : BaseAddon
    {
        [Constructable]
        public FireDemonStatueAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x364B, 1076674), 0, 0, 0);
        }

        public FireDemonStatueAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new FireDemonStatueDeed();

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    AddComponent(new LocalizedAddonComponent(0x369B, 1076674), 0, 0, 0);
                    break;
                case Direction.South:
                    AddComponent(new LocalizedAddonComponent(0x364B, 1076674), 0, 0, 0);
                    break;
            }
        }

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

    public class FireDemonStatueDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076674;  // Fire Demon Statue

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public FireDemonStatueDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public FireDemonStatueDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new FireDemonStatueAddon();


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
