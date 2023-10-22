#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader                                                                                                                                                                                                                                                        
using System;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Xanthos.Evo
{
	public class TrainingElementalDeed : Item
	{
		[Constructable]
		public TrainingElementalDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "a training elemental deed";
			LootType = LootType.Blessed;
			Hue = 1877;
		}

		public TrainingElementalDeed( Serial serial ) : base ( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
				Use( from );
		}

		public void Use( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster )
				from.SendMessage( "Your godly powers allow you to place this Training Elemental wherever you want." );
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house == null || !house.IsOwner( from ) )
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You are not the full owner of this house." );
					return;
				}
			}
			(new TrainingElementalSpawner()).MoveToWorld( from.Location, from.Map );
			this.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class TrainingElementalDeedTarget : Target
	{
		private TrainingElementalDeed m_Deed;

		public TrainingElementalDeedTarget( TrainingElementalDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )// Override the protected OnTarget() for our feature
		{
			if ( m_Deed.Deleted )
				return;

			if ( !( target is Item && ( target is BaseArmor || target is BaseWeapon )))
				from.SendMessage( "You cannot remove durability from that." );
			
			else
			{
				m_Deed.Delete(); // Delete the deed
			}
		}
	}
}
