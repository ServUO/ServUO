using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public abstract class BaseCloak : BaseClothing
    {
        public BaseCloak(int itemID)
            : this(itemID, 0)
        {
        }

        public BaseCloak(int itemID, int hue)
            : base(itemID, Layer.Cloak, hue)
        {
        }

        public BaseCloak(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable]
    public class Cloak : BaseCloak, IArcaneEquip
    {
        #region Arcane Impl
        private int m_MaxArcaneCharges, m_CurArcaneCharges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxArcaneCharges
        {
            get
            {
                return this.m_MaxArcaneCharges;
            }
            set
            {
                this.m_MaxArcaneCharges = value;
                this.InvalidateProperties();
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurArcaneCharges
        {
            get
            {
                return this.m_CurArcaneCharges;
            }
            set
            {
                this.m_CurArcaneCharges = value;
                this.InvalidateProperties();
                this.Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsArcane
        {
            get
            {
                return (this.m_MaxArcaneCharges > 0 && this.m_CurArcaneCharges >= 0);
            }
        }

        public void Update()
        {
            if (this.IsArcane)
                this.ItemID = 0x26AD;
            else if (this.ItemID == 0x26AD)
                this.ItemID = 0x1515;

            if (this.IsArcane && this.CurArcaneCharges == 0)
                this.Hue = 0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.IsArcane)
                list.Add(1061837, "{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges); // arcane charges: ~1_val~ / ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.IsArcane)
                this.LabelTo(from, 1061837, String.Format("{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges));
        }

        public void Flip()
        {
            if (this.ItemID == 0x1515)
                this.ItemID = 0x1530;
            else if (this.ItemID == 0x1530)
                this.ItemID = 0x1515;
        }

        #endregion

        [Constructable]
        public Cloak()
            : this(0)
        {
        }

        [Constructable]
        public Cloak(int hue)
            : base(0x1515, hue)
        {
            this.Weight = 5.0;
        }

        public Cloak(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            if (this.IsArcane)
            {
                writer.Write(true);
                writer.Write((int)this.m_CurArcaneCharges);
                writer.Write((int)this.m_MaxArcaneCharges);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        if (reader.ReadBool())
                        {
                            this.m_CurArcaneCharges = reader.ReadInt();
                            this.m_MaxArcaneCharges = reader.ReadInt();

                            if (this.Hue == 2118)
                                this.Hue = ArcaneGem.DefaultArcaneHue;
                        }

                        break;
                    }
            }

            if (this.Weight == 4.0)
                this.Weight = 5.0;
        }
    }

    [Flipable]
    public class RewardCloak : BaseCloak, IRewardItem
    {
        private int m_LabelNumber;
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return this.m_LabelNumber;
            }
            set
            {
                this.m_LabelNumber = value;
                this.InvalidateProperties();
            }
        }

        public override int LabelNumber
        {
            get
            {
                if (this.m_LabelNumber > 0)
                    return this.m_LabelNumber;

                return base.LabelNumber;
            }
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 3;
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod += 2;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod -= 2;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && this.m_IsRewardItem)
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { this.Hue, this.m_LabelNumber })); // X Year Veteran Reward
        }

        public override bool CanEquip(Mobile m)
        {
            if (!base.CanEquip(m))
                return false;

            return !this.m_IsRewardItem || Engines.VeteranRewards.RewardSystem.CheckIsUsableBy(m, this, new object[] { this.Hue, this.m_LabelNumber });
        }

        [Constructable]
        public RewardCloak()
            : this(0)
        {
        }

        [Constructable]
        public RewardCloak(int hue)
            : this(hue, 0)
        {
        }

        [Constructable]
        public RewardCloak(int hue, int labelNumber)
            : base(0x1515, hue)
        {
            this.Weight = 5.0;
            this.LootType = LootType.Blessed;

            this.m_LabelNumber = labelNumber;
        }

        public RewardCloak(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_LabelNumber);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_LabelNumber = reader.ReadInt();
                        this.m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }

            if (this.Parent is Mobile)
                ((Mobile)this.Parent).VirtualArmorMod += 2;
        }
    }

    [Flipable(0x230A, 0x2309)]
    public class FurCape : BaseCloak
    {
        [Constructable]
        public FurCape()
            : this(0)
        {
        }

        [Constructable]
        public FurCape(int hue)
            : base(0x230A, hue)
        {
            this.Weight = 4.0;
        }

        public FurCape(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [FlipableAttribute(0x45A4, 0x45A5)]
    public class GargishClothWingArmor : BaseClothing
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }

        [Constructable]
        public GargishClothWingArmor()
            : this(0)
        {
        }

        [Constructable]
        public GargishClothWingArmor(int hue)
            : base(0x45A4, Layer.Cloak, hue)
        {
            this.Weight = 2.0;
        }

        public GargishClothWingArmor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0x4002, 0x4003)]
    public class GargishFancyRobe : BaseClothing
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }

        [Constructable]
        public GargishFancyRobe()
            : this(0)
        {
        }

        [Constructable]
        public GargishFancyRobe(int hue)
            : base(0x4002, Layer.OuterTorso, hue)
        {
            this.Weight = 1.0;
        }

        public GargishFancyRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(0x4000, 0x4001)]
    public class GargishRobe : BaseClothing
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }

        [Constructable]
        public GargishRobe()
            : this(0)
        {
        }

        [Constructable]
        public GargishRobe(int hue)
            : base(0x4000, Layer.OuterTorso, hue)
        {
            this.Weight = 1.0;
        }

        public GargishRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}