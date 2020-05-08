namespace Server.Items
{
    public class LeggingsOfBane : ChainLegs
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1061100;// Leggings of Bane
        public override int ArtifactRarity => 11;
        public override int BasePoisonResistance => 36;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public LeggingsOfBane()
        {
            Hue = 0x4F5;
            ArmorAttributes.DurabilityBonus = 100;
            Attributes.BonusStam = 8;
            Attributes.AttackChance = 20;
        }

        public LeggingsOfBane(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt(); 
        }
    }
}
