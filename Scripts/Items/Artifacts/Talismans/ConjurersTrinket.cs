namespace Server.Items
{
    public class ConjurersTrinket : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1094800;  // Conjurer's Trinket

        [Constructable]
        public ConjurersTrinket()
            : base(0x2F58)
        {
            Hue = 1157;
            Slayer = TalismanSlayerName.Undead;
            Attributes.BonusStr = 1;
            Attributes.RegenHits = 2;
            Attributes.WeaponDamage = 20;
            Attributes.AttackChance = 10;
        }

        public ConjurersTrinket(Serial serial)
            : base(serial)
        {
        }

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