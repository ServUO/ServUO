using System;

namespace Server.Items
{
    [FlipableAttribute(0x1bdd, 0x1be0)]
    public class Log : Item, ICommodity, IAxe
    {
        protected virtual CraftResource DefaultResource { get { return CraftResource.RegularWood; } }

        private CraftResource m_Resource;
        [Constructable]
        public Log()
            : this(1)
        {
        }

        [Constructable]
        public Log(int amount)
            : this(CraftResource.RegularWood, amount)
        {
        }

        [Constructable]
        public Log(CraftResource resource)
            : this(resource, 1)
        {
        }

        [Constructable]
        public Log(CraftResource resource, int amount)
            : base(0x1BDD)
        {
            this.Stackable = true;
            this.Weight = 2.0;
            this.Amount = amount;

            this.m_Resource = resource;
            this.Hue = CraftResources.GetHue(resource);
        }

        public Log(Serial serial)
            : base(serial)
        {
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
                return CraftResources.IsStandard(this.m_Resource) ? this.LabelNumber : 1075062 + ((int)this.m_Resource - (int)CraftResource.RegularWood);
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

            writer.Write((int)1); // version

            writer.Write((int)this.m_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2: // Reset from Resource System
                    this.m_Resource = this.DefaultResource;
                    reader.ReadString();
                    break;
                case 1:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if (version == 0)
                this.m_Resource = CraftResource.RegularWood;
        }

        public virtual bool TryCreateBoards(Mobile from, double skill, Item item)
        {
            if (this.Deleted || !from.CanSee(this)) 
                return false;
            else if (from.Skills.Carpentry.Value < skill &&
                     from.Skills.Lumberjacking.Value < skill)
            {
                item.Delete();
                from.SendLocalizedMessage(1072652); // You cannot work this strange and unusual wood.
                return false;
            }
            base.ScissorHelper(from, item, 1, false);
            return true;
        }

        public virtual bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 0, new Board()))
                return false;
			
            return true;
        }
    }

    public class HeartwoodLog : Log
    {
        protected override CraftResource DefaultResource { get { return CraftResource.Heartwood; } }

        [Constructable]
        public HeartwoodLog()
            : this(1)
        {
        }

        [Constructable]
        public HeartwoodLog(int amount)
            : base(CraftResource.Heartwood, amount)
        {
        }

        public HeartwoodLog(Serial serial)
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 100, new HeartwoodBoard()))
                return false;

            return true;
        }
    }

    public class BloodwoodLog : Log
    {
        protected override CraftResource DefaultResource { get { return CraftResource.Bloodwood; } }

        [Constructable]
        public BloodwoodLog()
            : this(1)
        {
        }

        [Constructable]
        public BloodwoodLog(int amount)
            : base(CraftResource.Bloodwood, amount)
        {
        }

        public BloodwoodLog(Serial serial)
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 100, new BloodwoodBoard()))
                return false;

            return true;
        }
    }

    public class FrostwoodLog : Log
    {
        protected override CraftResource DefaultResource { get { return CraftResource.Frostwood; } }

        [Constructable]
        public FrostwoodLog()
            : this(1)
        {
        }

        [Constructable]
        public FrostwoodLog(int amount)
            : base(CraftResource.Frostwood, amount)
        {
        }

        public FrostwoodLog(Serial serial)
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 100, new FrostwoodBoard()))
                return false;

            return true;
        }
    }

    public class OakLog : Log
    {
        protected override CraftResource DefaultResource { get { return CraftResource.OakWood; } }

        [Constructable]
        public OakLog()
            : this(1)
        {
        }

        [Constructable]
        public OakLog(int amount)
            : base(CraftResource.OakWood, amount)
        {
        }

        public OakLog(Serial serial)
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 65, new OakBoard()))
                return false;

            return true;
        }
    }

    public class AshLog : Log
    {
        protected override CraftResource DefaultResource { get { return CraftResource.AshWood; } }

        [Constructable]
        public AshLog()
            : this(1)
        {
        }

        [Constructable]
        public AshLog(int amount)
            : base(CraftResource.AshWood, amount)
        {
        }

        public AshLog(Serial serial)
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 80, new AshBoard()))
                return false;

            return true;
        }
    }

    public class YewLog : Log
    {
        protected override CraftResource DefaultResource { get { return CraftResource.YewWood; } }

        [Constructable]
        public YewLog()
            : this(1)
        {
        }

        [Constructable]
        public YewLog(int amount)
            : base(CraftResource.YewWood, amount)
        {
        }

        public YewLog(Serial serial)
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

        public override bool Axe(Mobile from, BaseAxe axe)
        {
            if (!this.TryCreateBoards(from, 95, new YewBoard()))
                return false;

            return true;
        }
    }
}