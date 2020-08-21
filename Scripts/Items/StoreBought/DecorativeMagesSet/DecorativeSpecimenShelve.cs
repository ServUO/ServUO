namespace Server.Items
{
    [Furniture]
    [Flipable(0xA5E8, 0xA5E9)]
    public class DecorativeSpecimenShelve : BaseContainer
    {
        public override int LabelNumber => 1126496; // shelves
        public override int DefaultGumpID => 0x4D;
        
        [Constructable]
        public DecorativeSpecimenShelve()
            : base(0xA5E8)
        {
        }

        public DecorativeSpecimenShelve(Serial serial)
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
