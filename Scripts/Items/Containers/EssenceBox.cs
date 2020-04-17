namespace Server.Items
{
    public class EssenceBox : WoodenBox
    {
        [Constructable]
        public EssenceBox()
            : base()
        {
            Movable = true;
            Hue = 2306;

            DropItem(Loot.RandomEssence());
        }

        public EssenceBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113770;//Essence Box
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