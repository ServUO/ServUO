namespace Server.Items
{
    public class ArmorEngravingToolToken : PromotionalToken
    {
        public override TextDefinition ItemName => 1080547;  // Armor Engraving Tool
        public override TextDefinition ItemReceiveMessage => 1072223;  // An item has been placed in your backpack.
        public override TextDefinition ItemGumpName => 1071163;  // <center>Armor Engraving Tool</center>

        public override bool PlaceInBank => false;

        [Constructable]
        public ArmorEngravingToolToken()
        {
        }

        public override Item CreateItemFor(Mobile from)
        {
            return new ArmorEngravingTool();
        }

        public ArmorEngravingToolToken(Serial serial)
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
