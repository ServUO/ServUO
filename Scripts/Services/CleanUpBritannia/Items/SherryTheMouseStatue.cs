using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class SherryTheMouseStatue : Item
    {
        public override int LabelNumber => 1080171;  // Sherry the Mouse Statue
        private Timer m_NewsTimer;

        [Constructable]
        public SherryTheMouseStatue()
            : base(0x20D0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public override bool HandlesOnSpeech => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (IsLockedDown && e.HasKeyword(0x30) && e.Mobile.Alive && e.Mobile.InLOS(Location) && e.Mobile.InRange(this, 12)) // *news*
            {
                TownCrierEntry tce = GlobalTownCrierEntryList.Instance.GetRandomEntry();

                if (tce == null)
                {
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 1005643); // I have no news at this time.
                }
                else
                {
                    m_NewsTimer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(3.0), new TimerStateCallback(ShoutNews_Callback), new object[] { tce, 0 });

                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 502978); // Some of the latest news!
                }
            }
        }

        private void ShoutNews_Callback(object state)
        {
            object[] states = (object[])state;
            TownCrierEntry tce = (TownCrierEntry)states[0];
            int index = (int)states[1];

            if (index < 0 || index >= tce.Lines.Length)
            {
                if (m_NewsTimer != null)
                    m_NewsTimer.Stop();

                m_NewsTimer = null;
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, false, tce.Lines[index]);
                states[1] = index + 1;
            }
        }

        public SherryTheMouseStatue(Serial serial)
            : base(serial)
        {
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