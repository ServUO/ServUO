using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Items
{
    public class TwistedWealdTele : Teleporter
    {
        [Constructable]
        public TwistedWealdTele()
            : base(new Point3D(2189, 1253, 0), Map.Ilshenar)
        {
        }

        public TwistedWealdTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile player)
            {
                if (QuestHelper.GetQuest(player, typeof(DreadhornQuest)) != null)
                {
                    return base.OnMoveOver(player);
                }

                player.SendLocalizedMessage(1074274); // You dance in the fairy ring, but nothing happens.
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
