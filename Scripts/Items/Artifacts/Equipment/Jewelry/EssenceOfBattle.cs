namespace Server.Items
{
    public class EssenceOfBattle : GoldRing
    {
        public override bool IsArtifact => true;
        [Constructable]
        public EssenceOfBattle()
        {
            Hue = 0x550;
            Attributes.BonusDex = 7;
            Attributes.BonusStr = 7;
            Attributes.WeaponDamage = 30;
        }

        public EssenceOfBattle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072935;// Essence of Battle
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