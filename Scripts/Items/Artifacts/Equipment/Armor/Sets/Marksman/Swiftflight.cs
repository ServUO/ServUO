namespace Server.Items
{
    public class Swiftflight : Bow
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Swiftflight()
            : base()
        {
            SetHue = 0x594;

            Attributes.WeaponDamage = 40;

            SetSelfRepair = 3;
            SetAttributes.AttackChance = 15;
            SetAttributes.BonusDex = 8;
            SetAttributes.WeaponSpeed = 30;
            SetAttributes.WeaponDamage = 20;
        }

        public Swiftflight(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074308;// Swiftflight (Marksman Set)
        public override SetItem SetID => SetItem.Marksman;
        public override int Pieces => 2;
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