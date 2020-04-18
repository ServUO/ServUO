namespace Server.Items
{
    public class FriendOfTheLibraryToken : BaseTalisman
    {
        [Constructable]
        public FriendOfTheLibraryToken()
            : base(0x2F58)
        {
            Weight = 1.0;
            Hue = 0x28A;
        }

        public FriendOfTheLibraryToken(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073136;// Friend of the Library Token (allows donations to be made)
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