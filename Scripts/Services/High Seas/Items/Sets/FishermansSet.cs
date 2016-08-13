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

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }

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

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }

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

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }

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

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }

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
}
