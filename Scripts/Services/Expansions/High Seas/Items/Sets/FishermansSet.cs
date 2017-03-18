using System;
using Server;

namespace Server.Items
{
    public interface IFishingAttire
    {
        int BaitBonus { get; set; }
        int SetBonus { get; set; }
    }

    public class FishermansHat : TallStrawHat, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151190; } } //Fisherman's Tall Straw Hat

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 23; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansTrousers : StuddedLegs, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151191; } } //Fisherman's Trousers

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 6; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 7; } }
        public override int BasePoisonResistance { get { return 7; } }
        public override int BaseEnergyResistance { get { return 8; } }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansVest : LeatherChest, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151192; } } //Fisherman's Vest

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 19; } }
        public override int BaseColdResistance { get { return 5; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 5; } }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansEelskinGloves : LeatherGloves, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151189; } } //Fisherman's Eelskin Gloves

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 19; } }
        public override int BaseColdResistance { get { return 5; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 5; } }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansChestguard : GargishPlateChest, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151574; } } //Fisherman's Chestguard

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 24; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 9; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 9; } }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansKilt : GargishClothKilt, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151575; } } // Fisherman's Kilt

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 24; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 9; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 9; } }

        [Constructable]
        public FishermansKilt()
        {
            ItemID = 0x4052;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansArms : GargishLeatherArms, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151576; } } // Fisherman's Arms

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 7; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 21; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }

        [Constructable]
        public FishermansArms()
        {
            ItemID = 0x4052;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FishermansEarrings : GargishEarrings, ISetItem, IFishingAttire
    {
        public override int LabelNumber { get { return 1151577; } } // Fisherman's Earrings

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Fisherman; } }
        public override int Pieces { get { return 4; } }
        #endregion

        #region IFishingAttire Members
        public int BaitBonus { get { return 10; } set { } }
        public int SetBonus { get { return 50; } set { } }
        #endregion

        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 4; } }
        public override int BaseEnergyResistance { get { return 17; } }

        [Constructable]
        public FishermansEarrings()
        {
            ItemID = 0x4052;
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
