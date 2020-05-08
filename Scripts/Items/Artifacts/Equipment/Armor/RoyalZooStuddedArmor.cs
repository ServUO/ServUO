namespace Server.Items
{
    public class RoyalZooStuddedLegs : StuddedLegs
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073223;// Studded Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RoyalZooStuddedLegs()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusHits = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedLegs(Serial serial)
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

    public class RoyalZooStuddedGloves : StuddedGloves
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073223;// Studded Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RoyalZooStuddedGloves()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusHits = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedGloves(Serial serial)
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

    public class RoyalZooStuddedGorget : StuddedGorget
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073223;// Studded Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RoyalZooStuddedGorget()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusHits = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedGorget(Serial serial)
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

    public class RoyalZooStuddedArms : StuddedArms
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073223;// Studded Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RoyalZooStuddedArms()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusHits = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedArms(Serial serial)
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

    public class RoyalZooStuddedChest : StuddedChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073223;// Studded Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RoyalZooStuddedChest()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusHits = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedChest(Serial serial)
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

    public class RoyalZooStuddedFemaleChest : FemaleStuddedChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073223;// Studded Armor of the Britannia Royal Zoo
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RoyalZooStuddedFemaleChest()
            : base()
        {
            Hue = 0x109;
            Attributes.BonusHits = 2;
            Attributes.BonusMana = 3;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public RoyalZooStuddedFemaleChest(Serial serial)
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
