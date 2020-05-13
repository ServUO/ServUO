namespace Server.Items
{
    public class GargishClothWingArmorBearingTheCrestOfBlackthorn : GargishClothWingArmor
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishClothWingArmorBearingTheCrestOfBlackthorn()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Hue = 1766;
            Attributes.DefendChance = 5;
        }

        public GargishClothWingArmorBearingTheCrestOfBlackthorn(Serial serial)
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