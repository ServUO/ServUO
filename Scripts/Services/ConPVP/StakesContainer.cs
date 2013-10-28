using System;

namespace Server.Engines.ConPVP
{
    #if false
	[Flipable( 0x9A8, 0xE80 )]
	public class StakesContainer : LockableContainer
	{
		private Mobile m_Initiator;
		private Participant m_Participant;
		private Hashtable m_Owners;
		public override bool CheckItemUse( Mobile from, Item item )
		{
			Mobile owner = (Mobile)m_Owners[item];
			if ( owner != null && owner != from )
				return false;
			return base.CheckItemUse( from, item );
		}
		public override bool CheckTarget( Mobile from, Server.Targeting.Target targ, object targeted )
		{
			Mobile owner = (Mobile)m_Owners[targeted];
			if ( owner != null && owner != from )
				return false;
			return base.CheckTarget( from, targ, targeted );
		}
		public override bool CheckLift(Mobile from, Item item)
		{
			Mobile owner = (Mobile)m_Owners[item];
			if ( owner != null && owner != from )
				return false;
			return base.CheckLift( from, item );
		}
		public void ReturnItems()
		{
			ArrayList items = new ArrayList( this.Items );
			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = (Item)items[i];
				Mobile owner = (Mobile)m_Owners[item];
				if ( owner == null || owner.Deleted )
					owner = m_Initiator;
				if ( owner == null || owner.Deleted )
					return;
				if ( item.LootType != LootType.Blessed || !owner.PlaceInBackpack( item ) )
					owner.BankBox.DropItem( item );
			}
		}
		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			if ( m_Participant == null || !m_Participant.Contains( from ) )
			{
				if ( sendFullMessage )
					from.SendMessage( "You are not allowed to place items here." );
				return false;
			}
			if ( dropped is Container || dropped.Stackable )
			{
				if ( sendFullMessage )
					from.SendMessage( "That item cannot be used as stakes." );
				return false;
			}
			if ( !base.TryDropItem( from, dropped, sendFullMessage ) )
				return false;
			if ( from != null )
				m_Owners[dropped] = from;
			return true;
		}
		public override void RemoveItem( Item item )
		{
			base.RemoveItem( item );
			m_Owners.Remove( item );
		}
		public StakesContainer( DuelContext context, Participant participant ) : base( 0x9A8 )
		{
			Movable = false;
			m_Initiator = context.Initiator;
			m_Participant = participant;
			m_Owners = new Hashtable();
		}
		public StakesContainer( Serial serial ) : base( serial )
		{
		}
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
    #endif
}