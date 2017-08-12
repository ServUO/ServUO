using System;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class CoralTheOwl : Item, Server.Engines.VeteranRewards.IRewardItem
	{
        public override int LabelNumber { get { return 1123603; } } // Coral the Owl

        private Timer m_NewsTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        [Constructable]
        public CoralTheOwl() : base(0x9A9B)
        {
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if(IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public override bool HandlesOnSpeech { get { return true; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (IsLockedDown && e.HasKeyword(0x30) && e.Mobile.Alive && e.Mobile.InLOS(this.Location) && e.Mobile.InRange(this, 12)) // *news*
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

		public CoralTheOwl( Serial serial ) : base( serial )
		{
		}	
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write(IsRewardItem);
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    IsRewardItem = reader.ReadBool();
                    break;
                case 0:
                    break;
            }

            if (version == 0)
                IsRewardItem = true;
		}
	}
}