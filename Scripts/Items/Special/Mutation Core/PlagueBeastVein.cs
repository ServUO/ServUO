using System;
using Server.Network;

namespace Server.Items
{
    public class PlagueBeastVein : PlagueBeastComponent
    {
        private bool m_Cut;
        private Timer m_Timer;
        public PlagueBeastVein(int itemID, int hue)
            : base(itemID, hue)
        {
            this.m_Cut = false;
        }

        public PlagueBeastVein(Serial serial)
            : base(serial)
        {
        }

        public bool Cut
        {
            get
            {
                return this.m_Cut;
            }
        }
        public override bool Scissor(Mobile from, Scissors scissors)
        {
            if (this.IsAccessibleTo(from))
            {
                if (!this.m_Cut && this.m_Timer == null)
                {
                    this.m_Timer = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(3), new TimerStateCallback<Mobile>(CuttingDone), from);
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
            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((bool)this.m_Cut);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Cut = reader.ReadBool();
        }

        private void CuttingDone(Mobile from)
        {
            this.m_Cut = true;

            if (this.ItemID == 0x1B1C)
                this.ItemID = 0x1B1B;
            else
                this.ItemID = 0x1B1C;

            if (this.Owner != null)
                this.Owner.PlaySound(0x199);

            PlagueBeastRubbleOrgan organ = this.Organ as PlagueBeastRubbleOrgan;

            if (organ != null)
                organ.OnVeinCut(from, this);
        }
    }
}