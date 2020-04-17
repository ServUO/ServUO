namespace Server.Items
{
    public class EscutcheonDeAriadne : MetalKiteShield
    {
        public override bool IsArtifact => true;
        [Constructable]
        public EscutcheonDeAriadne()
        {
            LootType = LootType.Blessed;
            Hue = 0x8A5;
            ArmorAttributes.DurabilityBonus = 49;
            Attributes.ReflectPhysical = 5;
            Attributes.DefendChance = 5;
        }

        public EscutcheonDeAriadne(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077694;// Escutcheon de Ariadne
        public override int BasePhysicalResistance => 5;
        public override int BaseEnergyResistance => 1;
        public override int StrReq => 14;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}