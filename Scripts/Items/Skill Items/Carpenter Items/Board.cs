using System;

namespace Server.Items
{
    [FlipableAttribute(0x1BD7, 0x1BDA)]
    public class Board : Item, ICommodity
    {
        protected virtual CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

        private CraftResource m_Resource;
        [Constructable]
        public Board()
            : this(1)
        {
        }

        [Constructable]
        public Board(int amount)
            : this(CraftResource.RegularWood, amount)
        {
        }

        public Board(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public Board(CraftResource resource)
            : this(resource, 1)
        {
        }

        [Constructable]
        public Board(CraftResource resource, int amount)
            : base(0x1BD7)
        {
            this.Stackable = true;
            this.Amount = amount;

            this.m_Resource = resource;
            this.Hue = CraftResources.GetHue(resource);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return this.m_Resource;
            }
            set
            {
                this.m_Resource = value;
                this.InvalidateProperties();
            }
        }
        int ICommodity.DescriptionNumber 
        { 
            get
            {
                if (this.m_Resource >= CraftResource.OakWood && this.m_Resource <= CraftResource.YewWood)
                    return 1075052 + ((int)this.m_Resource - (int)CraftResource.OakWood);

                switch ( this.m_Resource )
                {
                    case CraftResource.Bloodwood:
                        return 1075055;
                    case CraftResource.Frostwood:
                        return 1075056;
                    case CraftResource.Heartwood:
                        return 1075062;	//WHY Osi.  Why?
                }

                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!CraftResources.IsStandard(this.m_Resource))
            {
                int num = CraftResources.GetLocalizationNumber(this.m_Resource);

                if (num > 0)
                    list.Add(num);
                else
                    list.Add(CraftResources.GetName(this.m_Resource));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);

            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 4: // Reset from Resource System
                    this.m_Resource = this.DefaultResource;
                    reader.ReadString();
                    break;
                case 3:
                case 2:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if ((version == 0 && this.Weight == 0.1) || (version <= 2 && this.Weight == 2))
                this.Weight = -1;

            if (version <= 1)
                this.m_Resource = CraftResource.RegularWood;
        }
    }

    public class HeartwoodBoard : Board
    {
        protected override CraftResource DefaultResource { get { return CraftResource.Heartwood; } }

        [Constructable]
        public HeartwoodBoard()
            : this(1)
        {
        }

        [Constructable]
        public HeartwoodBoard(int amount)
            : base(CraftResource.Heartwood, amount)
        {
        }

        public HeartwoodBoard(Serial serial)
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

    public class BloodwoodBoard : Board
    {
        protected override CraftResource DefaultResource { get { return CraftResource.Bloodwood; } }

        [Constructable]
        public BloodwoodBoard()
            : this(1)
        {
        }

        [Constructable]
        public BloodwoodBoard(int amount)
            : base(CraftResource.Bloodwood, amount)
        {
        }

        public BloodwoodBoard(Serial serial)
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

    public class FrostwoodBoard : Board
    {
        protected override CraftResource DefaultResource { get { return CraftResource.Frostwood; } }

        [Constructable]
        public FrostwoodBoard()
            : this(1)
        {
        }

        [Constructable]
        public FrostwoodBoard(int amount)
            : base(CraftResource.Frostwood, amount)
        {
        }

        public FrostwoodBoard(Serial serial)
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

    public class OakBoard : Board
    {
        protected override CraftResource DefaultResource { get { return CraftResource.OakWood; } }

        [Constructable]
        public OakBoard()
            : this(1)
        {
        }

        [Constructable]
        public OakBoard(int amount)
            : base(CraftResource.OakWood, amount)
        {
        }

        public OakBoard(Serial serial)
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

    public class AshBoard : Board
    {
        protected override CraftResource DefaultResource { get { return CraftResource.AshWood; } }

        [Constructable]
        public AshBoard()
            : this(1)
        {
        }

        [Constructable]
        public AshBoard(int amount)
            : base(CraftResource.AshWood, amount)
        {
        }

        public AshBoard(Serial serial)
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

    public class YewBoard : Board
    {
        protected override CraftResource DefaultResource { get { return CraftResource.YewWood; } }

        [Constructable]
        public YewBoard()
            : this(1)
        {
        }

        [Constructable]
        public YewBoard(int amount)
            : base(CraftResource.YewWood, amount)
        {
        }

        public YewBoard(Serial serial)
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