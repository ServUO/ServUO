namespace Server.Items
{
    public class GargishClothArmsArmor : BaseArmor
    {
        [Constructable]
        public GargishClothArmsArmor() : this(0)
        {
        }

        [Constructable]
        public GargishClothArmsArmor(int hue)
            : base(0x404)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public GargishClothArmsArmor(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 6;
        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;
        public override int StrReq => 20;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Cloth;
        public override CraftResource DefaultResource => CraftResource.None;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FemaleGargishClothArmsArmor : GargishClothArmsArmor
    {
        [Constructable]
        public FemaleGargishClothArmsArmor() : this(0)
        {
        }

        [Constructable]
        public FemaleGargishClothArmsArmor(int hue)
            : base(0x403)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public FemaleGargishClothArmsArmor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishClothChestArmor : BaseArmor
    {
        [Constructable]
        public GargishClothChestArmor()
            : this(0)
        {
        }

        [Constructable]
        public GargishClothChestArmor(int hue)
            : base(0x406)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public GargishClothChestArmor(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 6;
        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;
        public override int StrReq => 25;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Cloth;
        public override CraftResource DefaultResource => CraftResource.None;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FemaleGargishClothChestArmor : GargishClothChestArmor
    {
        [Constructable]
        public FemaleGargishClothChestArmor()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishClothChestArmor(int hue)
            : base(0x405)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public FemaleGargishClothChestArmor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishClothLegsArmor : BaseArmor
    {
        [Constructable]
        public GargishClothLegsArmor()
            : this(0)
        {
        }

        [Constructable]
        public GargishClothLegsArmor(int hue)
            : base(0x40A)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public GargishClothLegsArmor(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 6;
        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;
        public override int StrReq => 20;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Cloth;
        public override CraftResource DefaultResource => CraftResource.None;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FemaleGargishClothLegsArmor : GargishClothLegsArmor
    {
        [Constructable]
        public FemaleGargishClothLegsArmor()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishClothLegsArmor(int hue)
            : base(0x409)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public FemaleGargishClothLegsArmor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishClothKiltArmor : BaseArmor
    {
        [Constructable]
        public GargishClothKiltArmor()
            : this(0)
        {
        }

        [Constructable]
        public GargishClothKiltArmor(int hue)
            : base(0x408)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public GargishClothKiltArmor(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 6;
        public override int InitMinHits => 40;
        public override int InitMaxHits => 50;
        public override int StrReq => 20;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Cloth;
        public override CraftResource DefaultResource => CraftResource.None;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FemaleGargishClothKiltArmor : GargishClothKiltArmor
    {
        [Constructable]
        public FemaleGargishClothKiltArmor()
            : this(0)
        {
        }

        [Constructable]
        public FemaleGargishClothKiltArmor(int hue)
            : base(0x407)
        {
            Hue = hue;
            Weight = 2.0;
        }

        public FemaleGargishClothKiltArmor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
