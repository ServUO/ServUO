using System;
using Server.Gumps;

/*
** QuestRewardStone
** used to open the QuestPointsRewardGump that allows players to purchase rewards with their XmlQuestPoints Credits.
*/
namespace Server.Items
{
    public class QuestRewardStone : Item
    {
        [Constructable]
        public QuestRewardStone()
            : base(0xED4)
        {
            this.Movable = false;
            this.Name = "a Quest Points Reward Stone";
        }

        public QuestRewardStone(Serial serial)
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
            if (from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendGump(new QuestRewardGump(from, 0));
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }
    }
}