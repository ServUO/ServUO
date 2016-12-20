using System;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class MasterKey : PeerlessKey
	{	
		public override int LabelNumber{ get{ return 1074348; } } // master key
	
		private PeerlessAltar m_Altar;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public PeerlessAltar Altar
		{
			get{ return m_Altar; }
			set{ m_Altar = value; }
		}
		
		[Constructable]
		public MasterKey( int itemID ) : base( itemID )
		{
			LootType = LootType.Blessed;
		}
	
		public MasterKey( Serial serial ) : base( serial )
		{
		}		
		
		public override void OnDoubleClick( Mobile from )
		{	
			if ( CanOfferConfirmation( from ) && m_Altar != null)
			{
                if (m_Altar.IsAvailable && m_Altar.Peerless == null)
                {
                    from.CloseGump(typeof(ConfirmPartyGump));
                    from.SendGump(new ConfirmPartyGump(this));
                }
                else
                {
                    from.CloseGump(typeof(ConfirmEntranceGump));
                    from.SendGump(new ConfirmEntranceGump(m_Altar));
                }
			}
		}	
		
		public override void OnAfterDelete()
		{			
			if ( m_Altar == null )
				return;
				
			m_Altar.MasterKeys.Remove( this );
			
			if ( m_Altar.MasterKeys.Count == 0 && m_Altar.Fighters.Count == 0 )
				m_Altar.FinishSequence();
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( (Item) m_Altar );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_Altar = reader.ReadItem() as PeerlessAltar;
		}
		
		public virtual bool CanOfferConfirmation( Mobile from )
		{
			if ( m_Altar != null && m_Altar.Fighters.Contains( from ) )
			{
				from.SendLocalizedMessage( 1063296 ); // You may not use that teleporter at this time.
				return false;				
			}
				
			return true;
		}
	}
}