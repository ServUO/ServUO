namespace Server.Items
{
    public class Runesabre : RuneBlade
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Runesabre()
        {
            SkillBonuses.SetValues(0, SkillName.MagicResist, 5.0);
            WeaponAttributes.MageWeapon = -29;
        }

        public Runesabre(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073537;// runesabre
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