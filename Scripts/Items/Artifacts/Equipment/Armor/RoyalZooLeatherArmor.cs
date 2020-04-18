namespace Server.Items
{
    public class RoyalZooLeatherLegs : LeatherLegs
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooLeatherLegs()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusMana = 3;
            Attributes.RegenStam = 3;
            Attributes.ReflectPhysical = 10;
            Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073222;// Leather Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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

    public class RoyalZooLeatherGloves : LeatherGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooLeatherGloves()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusMana = 3;
            Attributes.RegenStam = 3;
            Attributes.ReflectPhysical = 10;
            Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073222;// Leather Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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

    public class RoyalZooLeatherGorget : LeatherGorget
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooLeatherGorget()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusMana = 3;
            Attributes.RegenStam = 3;
            Attributes.ReflectPhysical = 10;
            Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073222;// Leather Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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

    public class RoyalZooLeatherArms : LeatherArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooLeatherArms()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusMana = 3;
            Attributes.RegenStam = 3;
            Attributes.ReflectPhysical = 10;
            Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073222;// Leather Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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

    public class RoyalZooLeatherChest : LeatherChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooLeatherChest()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusMana = 3;
            Attributes.RegenStam = 3;
            Attributes.ReflectPhysical = 10;
            Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073222;// Leather Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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

    public class RoyalZooLeatherFemaleChest : FemaleLeatherChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooLeatherFemaleChest()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusMana = 3;
            Attributes.RegenStam = 3;
            Attributes.ReflectPhysical = 10;
            Attributes.LowerRegCost = 15;
        }

        public RoyalZooLeatherFemaleChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073222;// Leather Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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