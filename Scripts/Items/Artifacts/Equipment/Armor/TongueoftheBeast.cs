namespace Server.Items
{
    public class TongueOfTheBeast : WoodenKiteShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112405;  // Tongue of the Beast [Replica]
        public override int BasePhysicalResistance => 10;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;

        [Constructable]
        public TongueOfTheBeast()
        {
            Hue = 153;
            Attributes.SpellChanneling = 1;
            Attributes.RegenStam = 3;
            Attributes.RegenMana = 3;
        }

        public TongueOfTheBeast(Serial serial)
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
