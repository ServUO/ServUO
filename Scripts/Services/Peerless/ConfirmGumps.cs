using System;
using Server;
using Server.Items;

namespace Server.Gumps
{
	public class ConfirmPartyGump : BaseConfirmGump
	{
		public override int LabelNumber{ get{ return 1072525; } } // <CENTER>Are you sure you want to teleport <BR>your party to an unknown area?</CENTER>
		
		private MasterKey m_Key;
	
		public ConfirmPartyGump( MasterKey key ) : base()
		{
			m_Key = key;
		}
		
		public override void Confirm( Mobile from )
		{		
			if ( m_Key == null )
				return;				
				
			if (  m_Key.Altar == null  )
				return;
				
			m_Key.Altar.SendConfirmations( from );
			m_Key.Delete();		
		}
	}
	
	public class ConfirmExitGump : BaseConfirmGump
	{
		public override int LabelNumber{ get{ return 1075026; } } // Are you sure you wish to teleport?
		
		private object m_Altar;
	
		public ConfirmExitGump( object altar ) : base()
		{
			m_Altar = altar;
		}
		
		public override void Confirm( Mobile from )
		{		
			if ( m_Altar == null )
				return;
				
            if(m_Altar is PeerlessAltar)
			    ((PeerlessAltar)m_Altar).Exit( from );
		}
	}
	
	public class ConfirmEntranceGump : BaseConfirmGump
	{
		public override int LabelNumber{ get{ return 1072526; } } // <CENTER>Your party is teleporting to an unknown area.<BR>Do you wish to go?</CENTER>
			
		private PeerlessAltar m_Altar;	
			
		public ConfirmEntranceGump( PeerlessAltar altar ) : base()
		{
			m_Altar = altar;
		}
		
		public override void Confirm( Mobile from )
		{		
			if ( m_Altar == null )
				return;
				
			m_Altar.AddFighter( from, true );

            if (!m_Altar.IsAvailable && m_Altar.Peerless != null && m_Altar.Peerless.Alive)
            {
                m_Altar.AddFighter(from);
                m_Altar.Enter(from);
            }
		}
		
		public override void Refuse( Mobile from )
		{		
			if ( m_Altar == null )
				return;
				
			m_Altar.AddFighter( from, false );
		}
	}
}