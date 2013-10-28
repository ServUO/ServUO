using System;
using Server.Gumps;

/*
** QuestNote
** ArteGordon
**
*/
namespace Server.Items
{
    public class QuestHolder : XmlQuestHolder
    {
        [Constructable]
        public QuestHolder()
            : base()
        {
            this.Name = "A quest";
            this.TitleString = "A quest";
        }

        public QuestHolder(Serial serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            from.CloseGump(typeof(XmlQuestStatusGump));

            from.SendGump(new XmlQuestStatusGump(this, this.TitleString));
        }

        public override void OnSnoop(Mobile from)
        {
            if (from.IsStaff())
            {
                from.CloseGump(typeof(XmlQuestStatusGump));

                from.SendGump(new XmlQuestStatusGump(this, this.TitleString));
            }
        }
    }
}