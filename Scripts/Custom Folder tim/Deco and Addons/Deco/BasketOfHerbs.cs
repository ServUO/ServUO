using System;
using Server.Multis;

namespace Server.Items
{
	public class BasketOfHerbs : Item
	{
		private const double m_Bonus = 10;
		private bool m_Active = false;
		private SkillMod m_SkillMod;

		public bool IsActive{ get{ return m_Active; } }

		[Constructable]
		public BasketOfHerbs() : base( 0x194F )
		{
			LootType = LootType.Blessed;
			Weight = 1;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Active || !IsChildOf( from.Backpack ) )
				return;

			foreach ( BasketOfHerbs basket in from.Backpack.FindItemsByType( typeof( BasketOfHerbs ) ) )
			{
				if ( basket.IsActive )
					return;
			}

			BaseHouse house = BaseHouse.FindHouseAt( from );

			if ( house != null && house.IsOwner( from ) )
				AddSkillModTo( from );
		}

		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );

			if ( m_Active )
				RemoveSkillMod();
		}

		public void AddSkillModTo( Mobile to )
		{
			m_SkillMod = new DefaultSkillMod( SkillName.Cooking, true, m_Bonus );
			m_SkillMod.ObeyCap = true;
			to.AddSkillMod( m_SkillMod );
			m_Active = true;

			to.SendLocalizedMessage( 1075540 ); //The scent of fresh herbs begins to fill your home...
		}

		public void RemoveSkillMod()
		{
			m_SkillMod.Owner.SendLocalizedMessage( 1075541 ); //The scent of herbs gradually fades away...

			m_SkillMod.Remove();
			m_SkillMod = null;
			m_Active = false;
		}

		public BasketOfHerbs( Serial serial ) : base( serial )
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
}
