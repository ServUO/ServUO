namespace Server.Items
{
    public class GlovesOfFeudalGrip : DragonGloves
    {
        public override int LabelNumber => 1157349;  // gloves of feudal grip
        public override bool IsArtifact => true;
        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override CraftResource DefaultResource => CraftResource.None;

        [Constructable]
        public GlovesOfFeudalGrip()
        {
            Resource = CraftResource.None;

            Attributes.BonusStr = 8;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.WeaponDamage = 30;
        }

        public GlovesOfFeudalGrip(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class GargishKiltOfFeudalVise : GargishPlateKilt
    {
        public override int LabelNumber => 1157367;  // Kilt of Feudal Vise
        public override bool IsArtifact => true;
        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override CraftResource DefaultResource => CraftResource.None;

        [Constructable]
        public GargishKiltOfFeudalVise()
        {
            Resource = CraftResource.None;

            Attributes.BonusStr = 8;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 3;
            Attributes.WeaponDamage = 30;
        }

        public GargishKiltOfFeudalVise(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
