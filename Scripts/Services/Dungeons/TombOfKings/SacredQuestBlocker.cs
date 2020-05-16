using Server.Mobiles;

namespace Server.Items
{
    public class SacredQuestBlocker : Item
    {
        [Constructable]
        public SacredQuestBlocker()
            : base(0x1BC3)
        {
            Name = "Sacred Quest Blocker";

            Movable = false;
            Visible = false;
        }

        public SacredQuestBlocker(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (!base.OnMoveOver(m))
                return false;

            PlayerMobile pm = m as PlayerMobile;

            if (m is BaseCreature)
                pm = ((BaseCreature)m).ControlMaster as PlayerMobile;

            if (pm != null && pm.AbyssEntry)
            {
                m.SendLocalizedMessage(1112227); // May the Virtues guide thine quest.

                return true;
            }
            else
            {
                m.SendLocalizedMessage(1112226); // Thou must be on a Sacred Quest to pass through.

                return false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}