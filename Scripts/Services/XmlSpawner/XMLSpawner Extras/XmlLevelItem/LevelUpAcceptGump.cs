using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections;

namespace Server.Gumps
{
	public class LevelUpAcceptGump : Gump
	{
        private LevelUpScroll m_Scroll;
		private Mobile m_From;

		public LevelUpAcceptGump( LevelUpScroll scroll, Mobile from ) : base( 0, 0 )
		{
			m_Scroll = scroll;
			m_From = from;

            string PaymentMsg = null;
            if (LevelItems.RewardBlacksmith && LevelItems.BlacksmithRewardAmt > 0)
                PaymentMsg = "<BR><BR>If you accept and the process is successful, you will be given additional compensation of " + LevelItems.BlacksmithRewardAmt + ".";

			Closable=false;
			Disposable=false;
			Dragable=true;
			Resizable=false;
			AddPage(0);


            AddBackground(25, 22, 318, 268, 9390);
            AddLabel(52, 27, 0, @"Level Increase Request");
            AddLabel(52, 60, 0, @"Requested By:");
            AddLabel(52, 81, 0, @"Level Amount:");
            AddHtml(49, 109, 271, 116, @"<CENTER><U>Max Level Increase Request</U><BR><BR>Someone has requested your expert services in increasing the max levels of a levelable item."+PaymentMsg+"<BR><BR>Do you accept their offer?", (bool)false, (bool)true);
            AddButton(50, 235, 4023, 4024, 1, GumpButtonType.Reply, 0);
            AddButton(83, 235, 4017, 4018, 2, GumpButtonType.Reply, 0);
            if (m_From != null)
                AddLabel(155, 60, 390, m_From.Name.ToString());
            if (m_Scroll != null)
                AddLabel(155, 81, 390, m_Scroll.Value.ToString());
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile smith = state.Mobile;

			if ( smith == null )
				return;

			//Accept
			if ( info.ButtonID == 1 )
			{
                if ( m_From != null && m_Scroll != null)
                {
					if ( m_From != null )
					{
						m_Scroll.BlacksmithValidated = true;
                        m_From.CloseGump(typeof(AwaitingSmithApprovalGump));
                        m_From.SendMessage("They have validated your scroll.  Select a levelable item to increase max levels or ESC to apply at another time.");
                        m_From.Target = new LevelUpScroll.LevelItemTarget(m_Scroll); // Call our target
					}

					if ( smith != null ) //Accepted... send message to smith and pay them bonus reward
					{
						smith.SendMessage("Thank you for your services!");
                        if (smith != m_From && LevelItems.RewardBlacksmith && LevelItems.BlacksmithRewardAmt > 0)
                        {
                            smith.AddToBackpack(new BankCheck(LevelItems.BlacksmithRewardAmt));
                            smith.SendMessage("A Bonus payment has been added to your pack.");
                        }
					}
				}
				else
				{
					if ( m_From != null && smith != null )
					{
						m_From.SendMessage( "There was a problem validating this scroll." );
                        smith.SendMessage( "There was a problem validating this scroll." );
					}
				}
			}

			//Decline
			if ( info.ButtonID == 2 )
			{
				smith.SendMessage( "You have declined their offer." );

				if ( m_From != null )
                    m_From.CloseGump(typeof(AwaitingSmithApprovalGump));
					m_From.SendMessage( "They have declined your offer" );
			}
		}
	}
}