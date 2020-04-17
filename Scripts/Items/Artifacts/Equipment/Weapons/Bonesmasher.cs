namespace Server.Items
{
    public class Bonesmasher : DiamondMace
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Bonesmasher()
        {
            Hue = 0x482;
            SkillBonuses.SetValues(0, SkillName.Macing, 10.0);
            WeaponAttributes.HitLeechMana = 40;
            WeaponAttributes.SelfRepair = 2;
        }

        public Bonesmasher(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075030;// Bonesmasher
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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