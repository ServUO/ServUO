using System;
using Server.Gumps;

namespace Server.Engines.Quests
{
    public class ConfirmHeritageGump : Gump
    { 
        private readonly HeritageQuester m_Quester;
        public ConfirmHeritageGump(HeritageQuester quester)
            : base(50, 50)
        { 
            m_Quester = quester;
			
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
			
            AddPage(0);
			
            AddBackground(0, 0, 240, 135, 0x2422);
			
            object message = m_Quester.ConfirmMessage;
			
            if (message is int)
                AddHtmlLocalized(15, 15, 210, 75, (int)message, 0x0, false, false);
            else if (message is string)
                AddHtml(15, 15, 210, 75, (string)message, false, false);
				
            AddButton(160, 95, 0xF7, 0xF8, (int)Buttons.Okay, GumpButtonType.Reply, 0);
            AddButton(90, 95, 0xF2, 0xF1, (int)Buttons.Close, GumpButtonType.Reply, 0); 		
        }

        private enum Buttons
        {
            Close,
            Okay,
        }
        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        { 
            if (m_Quester == null)
                return;
				
            if (info.ButtonID == (int)Buttons.Okay)
            {
                Mobile m = state.Mobile;
				
                if (HeritageQuester.Check(m))
                {
                    HeritageQuester.AddPending(m, m_Quester);
                    Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerStateCallback(CloseHeritageGump), m);
					
                    state.Mobile.Send(new HeritagePacket(m.Female, (short)(m_Quester.Race.RaceID + 1)));					
                }
            }
        }

        private void CloseHeritageGump(object args)
        {
            if (args is Mobile)
            { 
                Mobile m = (Mobile)args;
				
                if (HeritageQuester.IsPending(m))
                {
                    m.Send(HeritagePacket.Close);
					
                    HeritageQuester.RemovePending(m);
                }
            }
        }
    }
}