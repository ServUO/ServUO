namespace Server.Items
{
    public class EmeraldMace : DiamondMace
    {
        public override int LabelNumber => 1073530; // emerald mace

        [Constructable]
        public EmeraldMace()
        {
            WeaponAttributes.ResistPoisonBonus = 5;
        }

        public EmeraldMace(Serial serial)
            : base(serial)
        {
        }

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