using System;
using Server.Engines.Craft;

namespace Server.Items
{
    #region Reward Clothing
    public class ZooMemberThighBoots : ThighBoots
    {
        public override int LabelNumber
        {
            get
            {
                return 1073221;
            }
        }// Britannia Royal Zoo Member

        [Constructable]
        public ZooMemberThighBoots()
            : this(0)
        {
        }

        [Constructable]
        public ZooMemberThighBoots(int hue)
            : base(hue)
        {
        }

        public ZooMemberThighBoots(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
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

    #endregion

    [Alterable(typeof(DefTailoring), typeof(LeatherTalons), true)]
    public abstract class BaseShoes : BaseClothing
    {
        public BaseShoes(int itemID)
            : this(itemID, 0)
        {
        }

        public BaseShoes(int itemID, int hue)
            : base(itemID, Layer.Shoes, hue)
        {
        }

        public BaseShoes(Serial serial)
            : base(serial)
        {
        }

        public override bool Scissor(Mobile from, Scissors scissors)
        {
            if (this.DefaultResource == CraftResource.None)
                return base.Scissor(from, scissors);

            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    break; // empty, resource removed
                case 1:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        this.m_Resource = this.DefaultResource;
                        break;
                    }
            }
        }
    }

    [Flipable(0x2307, 0x2308)]
    public class FurBoots : BaseShoes
    {
        [Constructable]
        public FurBoots()
            : this(0)
        {
        }

        [Constructable]
        public FurBoots(int hue)
            : base(0x2307, hue)
        {
            this.Weight = 3.0;
        }

        public FurBoots(Serial serial)
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

    [FlipableAttribute(0x170b, 0x170c)]
    public class Boots : BaseShoes
    {
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        [Constructable]
        public Boots()
            : this(0)
        {
        }

        [Constructable]
        public Boots(int hue)
            : base(0x170B, hue)
        {
            this.Weight = 3.0;
        }

        public Boots(Serial serial)
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
    public class ThighBoots : BaseShoes, IArcaneEquip
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

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.IsArcane)
                this.LabelTo(from, 1061837, String.Format("{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges));
        }

        public void Update()
        {
            if (this.IsArcane)
                this.ItemID = 0x26AF;
            else if (this.ItemID == 0x26AF)
                this.ItemID = 0x1711;

            if (this.IsArcane && this.CurArcaneCharges == 0)
                this.Hue = 0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.IsArcane)
                list.Add(1061837, "{0}\t{1}", this.m_CurArcaneCharges, this.m_MaxArcaneCharges); // arcane charges: ~1_val~ / ~2_val~
        }

        public void Flip()
        {
            if (this.ItemID == 0x1711)
                this.ItemID = 0x1712;
            else if (this.ItemID == 0x1712)
                this.ItemID = 0x1711;
        }

        #endregion

        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        [Constructable]
        public ThighBoots()
            : this(0)
        {
        }

        [Constructable]
        public ThighBoots(int hue)
            : base(0x1711, hue)
        {
            this.Weight = 4.0;
        }

        public ThighBoots(Serial serial)
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
        }
    }

    [FlipableAttribute(0x170f, 0x1710)]
    public class Shoes : BaseShoes
    {
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        [Constructable]
        public Shoes()
            : this(0)
        {
        }

        [Constructable]
        public Shoes(int hue)
            : base(0x170F, hue)
        {
            this.Weight = 2.0;
        }

        public Shoes(Serial serial)
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

    [FlipableAttribute(0x170d, 0x170e)]
    public class Sandals : BaseShoes
    {
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        [Constructable]
        public Sandals()
            : this(0)
        {
        }

        [Constructable]
        public Sandals(int hue)
            : base(0x170D, hue)
        {
            this.Weight = 1.0;
        }

        public Sandals(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
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

    [Flipable(0x2797, 0x27E2)]
    public class NinjaTabi : BaseShoes
    {
        [Constructable]
        public NinjaTabi()
            : this(0)
        {
        }

        [Constructable]
        public NinjaTabi(int hue)
            : base(0x2797, hue)
        {
            this.Weight = 2.0;
        }

        public NinjaTabi(Serial serial)
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

    [Flipable(0x2796, 0x27E1)]
    public class SamuraiTabi : BaseShoes
    {
        [Constructable]
        public SamuraiTabi()
            : this(0)
        {
        }

        [Constructable]
        public SamuraiTabi(int hue)
            : base(0x2796, hue)
        {
            this.Weight = 2.0;
        }

        public SamuraiTabi(Serial serial)
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

    [Flipable(0x2796, 0x27E1)]
    public class Waraji : BaseShoes
    {
        [Constructable]
        public Waraji()
            : this(0)
        {
        }

        [Constructable]
        public Waraji(int hue)
            : base(0x2796, hue)
        {
            this.Weight = 2.0;
        }

        public Waraji(Serial serial)
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

    [FlipableAttribute(0x2FC4, 0x317A)]
    public class ElvenBoots : BaseShoes
    {
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Elf;
            }
        }

        [Constructable]
        public ElvenBoots()
            : this(0)
        {
        }

        [Constructable]
        public ElvenBoots(int hue)
            : base(0x2FC4, hue)
        {
            this.Weight = 2.0;
        }

        public ElvenBoots(Serial serial)
            : base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}