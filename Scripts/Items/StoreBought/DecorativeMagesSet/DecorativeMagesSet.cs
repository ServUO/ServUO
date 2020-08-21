namespace Server.Items
{
    public class DecorativeMagesSet : WoodenChest
    {
        public override int LabelNumber => 1159521;  // Decorative Mage Set

        [Constructable]
        public DecorativeMagesSet()
        {
            DropItem(new DecorativeMageThrone());
            DropItem(new DecorativeMageThrone());
            DropItem(new DecorativeMagicBookStand());
            DropItem(new DecorativeSpecimenShelve());
            DropItem(new DecorativeSpecimenShelve());
            DropItem(new DecorativeMagesCrystalBall());
            DropItem(new DecorativeMagesRugAddonDeed());
        }


        public DecorativeMagesSet(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
