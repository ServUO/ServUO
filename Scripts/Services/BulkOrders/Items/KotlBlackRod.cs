namespace Server.Items
{
    public class KotlBlackRod : QuarterStaff
    {
        public override int LabelNumber => 1156990;  // kotl black rod
        public override bool IsArtifact => true;

        [Constructable]
        public KotlBlackRod()
        {
            Resource = CraftResource.None;
            Hue = 1902;

            WeaponAttributes.MageWeapon = 30;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 2;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public KotlBlackRod(Serial serial)
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

    public class GargishKotlBlackRod : GargishGnarledStaff
    {
        public override int LabelNumber => 1156994;  // gargish kotl black rod
        public override bool IsArtifact => true;

        [Constructable]
        public GargishKotlBlackRod()
        {
            Resource = CraftResource.None;
            Hue = 1902;

            WeaponAttributes.MageWeapon = 30;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 2;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GargishKotlBlackRod(Serial serial)
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
