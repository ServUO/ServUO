//Unknown Author 
//Fix By Formosa

using System;
using System.Collections;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class Repair_Armor_DeedTarget : Target
	{
		private Repair_Armor_Deed m_Deed;
			
		public Repair_Armor_DeedTarget( Repair_Armor_Deed deed ) :  base ( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( targeted is BaseArmor )
			{
				BaseArmor repairing = (BaseArmor)targeted;
	
				if ( !repairing.IsChildOf( from.Backpack ) )
				{
					from.SendLocalizedMessage( 1044275 ); // The item must be in your backpack to repair it.
				}
				else if ( repairing.MaxHitPoints <= 0 || repairing.HitPoints == repairing.MaxHitPoints )
				{
					from.SendLocalizedMessage( 1044281 );// That item is in full repair
				}	
				else 
				{
					from.SendLocalizedMessage( 1044279 ); // You repair the item.
					repairing.HitPoints = repairing.MaxHitPoints;
					
					m_Deed.Delete();
				}	
			}
			else if ( targeted is Item )
			{
				from.SendLocalizedMessage( 1044277 ); // That item cannot be repaired.
			}
			else
			{
				from.SendLocalizedMessage( 500426 ); // You can't repair that.
			}
		}
	}

	public class Repair_Armor_Deed : Item, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.Seer )]
		public bool IsRewardItem
		{
			get{ return m_IsRewardItem; }
			set{ m_IsRewardItem = value; InvalidateProperties(); }
		}

		[Constructable]
		public Repair_Armor_Deed() : base( 0x14F0 )
		{
			Weight = 50.0;
			Name = "(Special) Armor Repair Deed";
			Hue = 0x851;
			LootType = LootType.Blessed;
			Movable = false;
		}

		public Repair_Armor_Deed( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_IsRewardItem )
				list.Add( 1076218 ); // 2nd Year Veteran Reward
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (bool) m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_IsRewardItem = reader.ReadBool();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.Target = new Repair_Armor_DeedTarget( this );
				from.SendLocalizedMessage( 1044276 ); // Target an item to repair.
			}
		}
	}
}