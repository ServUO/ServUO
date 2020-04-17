namespace Server.Items
{
    public class DecoDragonsBlood2 : Item
    {
        [Constructable]
        public DecoDragonsBlood2()
            : base(0xF82)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoDragonsBlood2(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}