using System;

namespace Server.Factions
{
    public abstract class BaseSystemController : Item
    {
        private int m_LabelNumber;
        public BaseSystemController(int itemID)
            : base(itemID)
        {
        }

        public BaseSystemController(Serial serial)
            : base(serial)
        {
        }

        public virtual int DefaultLabelNumber
        {
            get
            {
                return base.LabelNumber;
            }
        }
        public new virtual string DefaultName
        {
            get
            {
                return null;
            }
        }
        public override int LabelNumber
        {
            get
            {
                if (this.m_LabelNumber > 0)
                    return this.m_LabelNumber;

                return this.DefaultLabelNumber;
            }
        }
        public virtual void AssignName(TextDefinition name)
        {
            if (name != null && name.Number > 0)
            {
                this.m_LabelNumber = name.Number;
                this.Name = null;
            }
            else if (name != null && name.String != null)
            {
                this.m_LabelNumber = 0;
                this.Name = name.String;
            }
            else
            {
                this.m_LabelNumber = 0;
                this.Name = this.DefaultName;
            }

            this.InvalidateProperties();
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