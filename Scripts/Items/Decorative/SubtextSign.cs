using System;

namespace Server.Items
{
    public class SubtextSign : Sign
    {
        private string m_Subtext;
        [Constructable]
        public SubtextSign(SignType type, SignFacing facing, string subtext)
            : base(type, facing)
        {
            this.m_Subtext = subtext;
        }

        [Constructable]
        public SubtextSign(int itemID, string subtext)
            : base(itemID)
        {
            this.m_Subtext = subtext;
        }

        public SubtextSign(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Subtext
        {
            get
            {
                return this.m_Subtext;
            }
            set
            {
                this.m_Subtext = value;
                this.InvalidateProperties();
            }
        }
        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (!String.IsNullOrEmpty(this.m_Subtext))
                this.LabelTo(from, this.m_Subtext);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (!String.IsNullOrEmpty(this.m_Subtext))
                list.Add(this.m_Subtext);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(this.m_Subtext);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Subtext = reader.ReadString();
        }
    }
}