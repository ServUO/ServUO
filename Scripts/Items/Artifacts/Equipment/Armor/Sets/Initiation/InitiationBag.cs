namespace Server.Items
{
    public class InitiationSuitBag : Bag
    {
        public override bool IsArtifact => true;
        public override string DefaultName => "Initiation Suit Bag";

        [Constructable]
        public InitiationSuitBag()
        {
            Hue = 0x30;

            DropItem(new InitiationArms());
            DropItem(new InitiationCap());
            DropItem(new InitiationChest());
            DropItem(new InitiationGloves());
            DropItem(new InitiationGorget());
            DropItem(new InitiationLegs());
        }

        public InitiationSuitBag(Serial serial) : base(serial)
        {
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
}
