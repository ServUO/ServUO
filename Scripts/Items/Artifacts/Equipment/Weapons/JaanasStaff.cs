namespace Server.Items
{
    public class JaanasStaff : GnarledStaff
    {
        public override bool IsArtifact => true;
        [Constructable]
        public JaanasStaff()
            : base()
        {
            Hue = 0x58C;
            WeaponAttributes.MageWeapon = 10;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 220;
            Attributes.DefendChance = 15;
        }

        public JaanasStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1079790;// Jaana's Staff
        public override int ArtifactRarity => 11;
        public override int InitMinHits => 225;
        public override int InitMaxHits => 225;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NegativeAttributes.NoRepair = 0;
            }
        }
    }
}