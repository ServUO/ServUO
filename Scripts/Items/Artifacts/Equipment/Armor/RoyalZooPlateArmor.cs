namespace Server.Items
{
    public class RoyalZooPlateLegs : PlateLegs
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateLegs()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateGloves : PlateGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateGloves()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateGloves(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateGorget : PlateGorget
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateGorget()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateGorget(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateArms : PlateArms
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateArms()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateChest : PlateChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateChest()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateFemaleChest : FemalePlateChest
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateFemaleChest()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateFemaleChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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

    public class RoyalZooPlateHelm : PlateHelm
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RoyalZooPlateHelm()
            : base()
        {
            Hue = 0x109;
            Attributes.Luck = 100;
            Attributes.DefendChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooPlateHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073224;// Platemail Armor of the Britannia Royal Zoo
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