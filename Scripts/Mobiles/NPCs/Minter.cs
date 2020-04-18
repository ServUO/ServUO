namespace Server.Mobiles
{
    public class Minter : Banker
    {
        [Constructable]
        public Minter()
        {
            Title = "the minter";
        }

        public Minter(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.MerchantsGuild;
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