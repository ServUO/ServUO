namespace Server.Items
{
    public class GauntletsOfNobility : RingmailGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GauntletsOfNobility()
        {
            Hue = 0x4FE;
            Attributes.BonusStr = 8;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 20;
        }

        public GauntletsOfNobility(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061092;// Gauntlets of Nobility
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 18;
        public override int BasePoisonResistance => 20;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (Hue == 0x562)
                    Hue = 0x4FE;

                PhysicalBonus = 0;
                PoisonBonus = 0;
            }
        }
    }
}