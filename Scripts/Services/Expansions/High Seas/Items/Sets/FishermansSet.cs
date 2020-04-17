namespace Server.Items
{
    public interface IFishingAttire
    {
        int BaitBonus { get; set; }
        int SetBonus { get; set; }
    }

    public class FishermansHat : TallStrawHat, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151190;  //Fisherman's Tall Straw Hat

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => Resistances.Physical == 0 ? 3 : base.BasePhysicalResistance;
        public override int BaseFireResistance => Resistances.Fire == 0 ? 8 : base.BaseFireResistance;
        public override int BaseColdResistance => Resistances.Cold == 0 ? 23 : base.BaseColdResistance;
        public override int BasePoisonResistance => Resistances.Poison == 0 ? 8 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => Resistances.Energy == 0 ? 8 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansHat()
        {
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansHat(Serial serial) : base(serial)
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

    public class FishermansTrousers : StuddedLegs, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151191;  //Fisherman's Trousers

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 6 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 20 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 7 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 7 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 8 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansTrousers()
        {
            Hue = 2578;
            SetHue = 2578;

            ArmorAttributes.MageArmor = 1;
        }

        public FishermansTrousers(Serial serial)
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

    public class FishermansVest : LeatherChest, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151192;  //Fisherman's Vest

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 4 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 19 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 5 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 5 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 5 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansVest()
        {
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansVest(Serial serial)
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

    public class FishermansEelskinGloves : LeatherGloves, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151189;  //Fisherman's Eelskin Gloves

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 4 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 19 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 5 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 5 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 5 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansEelskinGloves()
        {
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansEelskinGloves(Serial serial)
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

    public class FishermansChestguard : GargishPlateChest, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151574;  //Fisherman's Chestguard

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 24 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 10 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 9 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 10 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 9 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansChestguard()
        {
            ItemID = 0x4052;
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansChestguard(Serial serial)
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

    public class FishermansKilt : GargishClothKiltArmor, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151575;  // Fisherman's Kilt

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 7 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 21 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 8 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 8 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 8 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansKilt()
        {
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansKilt(Serial serial)
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

    public class FishermansArms : GargishLeatherArms, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151576;  // Fisherman's Arms

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 7 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 8 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 21 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 8 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 8 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansArms()
        {
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansArms(Serial serial)
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

    public class FishermansEarrings : GargishEarrings, ISetItem, IFishingAttire
    {
        public override int LabelNumber => 1151577;  // Fisherman's Earrings

        #region ISetItem Members
        public override SetItem SetID => SetItem.Fisherman;
        public override int Pieces => 4;
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        public override int BasePhysicalResistance => PhysicalBonus == 0 ? 3 : base.BasePhysicalResistance;
        public override int BaseFireResistance => FireBonus == 0 ? 4 : base.BaseFireResistance;
        public override int BaseColdResistance => ColdBonus == 0 ? 4 : base.BaseColdResistance;
        public override int BasePoisonResistance => PoisonBonus == 0 ? 4 : base.BasePoisonResistance;
        public override int BaseEnergyResistance => EnergyBonus == 0 ? 17 : base.BaseEnergyResistance;

        public override int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = base.BasePhysicalResistance;
                list[1] = base.BaseFireResistance;
                list[2] = base.BaseColdResistance;
                list[3] = base.BasePoisonResistance;
                list[4] = base.BaseEnergyResistance;

                return list;
            }
        }

        [Constructable]
        public FishermansEarrings()
        {
            Hue = 2578;
            SetHue = 2578;
        }

        public FishermansEarrings(Serial serial)
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
