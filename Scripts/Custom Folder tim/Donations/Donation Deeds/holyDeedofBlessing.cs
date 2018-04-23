using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class HolyDeedofBlessingTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private HolyDeedofBlessing m_Deed;

		public HolyDeedofBlessingTarget( HolyDeedofBlessing deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( m_Deed == null || from.Backpack == null || !m_Deed.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 );
			else if ( target is BaseClothing )
			{
				Item item = (Item)target;
				if ( item.LootType == LootType.Blessed || item.BlessedFor == from ) // Check if its already newbied (blessed)
					from.SendLocalizedMessage( 1045113 ); // That item is already blessed
				else if ( item.LootType != LootType.Regular )
					from.SendLocalizedMessage( 1045114 ); // You can not bless that item
				else
				{
                    if (!item.IsChildOf(from.Backpack) || item is HoodedShroudOfShadows ) // Make sure its in their pack or they are wearing it
						from.SendLocalizedMessage( 500509 ); // You cannot bless that object
					else
					{
						item.LootType = LootType.Blessed;
						item.Hue = 1154;

						string namecheck = item.Name;
						if ( !String.IsNullOrEmpty( namecheck ) )
							namecheck = namecheck.ToLower();
						else
							namecheck = String.Empty;

						if ( namecheck.IndexOf( "blessed by unholy skull" ) == -1 && namecheck.IndexOf( "with holy blessing" ) == -1 )
						{
							if ( item.Name == null )
								item.Name = item.ItemData.Name + " with Holy Blessing";
							else
								item.Name = item.Name + " with Holy Blessing";
						}

						from.SendLocalizedMessage( 1010026 );

						m_Deed.Delete(); // Delete the bless deed
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 500509 ); // You cannot bless that object
			}
		}
	}

	public class HolyDeedofBlessing : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public HolyDeedofBlessing() : base( 0x1226 )
		{
			Hue = 1154;
			Weight = 15.0;
			Name = "Holy Angel of Blessings";
			LootType = LootType.Cursed;
		}

		public HolyDeedofBlessing( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return true; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendLocalizedMessage( 1005018 ); // What item would you like to bless? (Clothes Only)
				from.Target = new HolyDeedofBlessingTarget( this ); // Call our target
			 }
		}
	}
}