namespace Server.Items
{
    public class CavortingClub : Club
    {
        public override bool IsArtifact => true;
        [Constructable]
        public CavortingClub()
        {
            Hue = 0x593;
            WeaponAttributes.SelfRepair = 3;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.ResistFireBonus = 8;
            WeaponAttributes.ResistColdBonus = 8;
            WeaponAttributes.ResistPoisonBonus = 8;
            WeaponAttributes.ResistEnergyBonus = 8;
        }

        public CavortingClub(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063472;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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