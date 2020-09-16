using Server.Network;
using System;

namespace Server.Items
{
    public class PlagueBeastVein : PlagueBeastComponent
    {
        private bool m_Cut;
        private Timer m_Timer;
        public PlagueBeastVein(int itemID, int hue)
            : base(itemID, hue)
        {
            m_Cut = false;
        }

        public PlagueBeastVein(Serial serial)
            : base(serial)
        {
        }

        public bool Cut => m_Cut;
        public override bool Scissor(Mobile from, Scissors scissors)
        {
            if (IsAccessibleTo(from))
            {
                if (!m_Cut && m_Timer == null)
                {
                    m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(3), CuttingDone, from);
                    scissors.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071899); // You begin cutting through the vein.
                    return true;
                }
                else
                    scissors.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071900); // // This vein has already been cut.
            }

            return false;
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null && m_Timer.Running)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_Cut);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Cut = reader.ReadBool();
        }

        private void CuttingDone(Mobile from)
        {
            m_Cut = true;

            if (ItemID == 0x1B1C)
                ItemID = 0x1B1B;
            else
                ItemID = 0x1B1C;

            if (Owner != null)
                Owner.PlaySound(0x199);

            PlagueBeastRubbleOrgan organ = Organ as PlagueBeastRubbleOrgan;

            if (organ != null)
                organ.OnVeinCut(from, this);
        }
    }
}
