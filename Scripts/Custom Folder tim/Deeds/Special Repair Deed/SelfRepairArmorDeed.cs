//By: SHAMBAMPOW
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class SelfRepairArmorTarget : Target
	{
		private SelfRepairArmor m_Deed;

		public SelfRepairArmorTarget( SelfRepairArmor deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseArmor )
			{
				Item item = (Item)target;

				
				if ( ((BaseArmor)item).ArmorAttributes.SelfRepair >= 10 )
				{
					from.SendMessage( "That item has reached the maximum self repair!" );
					return;
				}
				else
				{
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add self repair to that item there!" ); 
					}
					else
					{
						((BaseArmor)item).ArmorAttributes.SelfRepair += 1;

						if( ((BaseArmor)item).ArmorAttributes.SelfRepair >= 10 )
						{
							((BaseArmor)item).ArmorAttributes.SelfRepair = 10;
							from.SendMessage( "You increase the self repair on the item... it has now reached it's max." );
						}
						else
							from.SendMessage( "You increase the self repair on the item..." );

						m_Deed.Delete();
					}
				}
			}
			else if ( target is BaseWeapon )
			{
				Item item = (Item)target;

				
				if ( ((BaseWeapon)item).WeaponAttributes.SelfRepair >= 10 )
				{
					from.SendMessage( "That item has reached the maximum self repair!" );
					return;
				}
				else
				{
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add self repair to that item there!" ); 
					}
					else
					{
						((BaseWeapon)item).WeaponAttributes.SelfRepair += 1;
							
						if( ((BaseWeapon)item).WeaponAttributes.SelfRepair >= 10 )
						{
							((BaseWeapon)item).WeaponAttributes.SelfRepair = 10;
							from.SendMessage( "You increase the self repair on the item... it has now reached it's max." );
						}
						else
							from.SendMessage( "You increase the self repair on the item..." );

						m_Deed.Delete();
					}
				}
			}

			else if ( target is BaseHat )
			{
				Item item = (Item)target;

				
				if ( ((BaseHat)item).ClothingAttributes.SelfRepair >= 10 )
				{
					from.SendMessage( "That item has reached the maximum self repair!" );
					return;
				}
				else
				{
					if( item.RootParent != from )
					{
						from.SendMessage( "You cannot add self repair to that item there!" ); 
					}
					else
					{
						((BaseHat)item).ClothingAttributes.SelfRepair += 1;
							
						if( ((BaseHat)item).ClothingAttributes.SelfRepair >= 10 )
						{
							((BaseHat)item).ClothingAttributes.SelfRepair = 10;
							from.SendMessage( "You increase the self repair on the item... it has now reached it's max." );
						}
						else
							from.SendMessage( "You increase the self repair on the item..." );

						m_Deed.Delete();
					}
				}
			}
			else
			{
				from.SendMessage( "You cannot put self repair on that!" );
			}
		}
	}

	public class SelfRepairArmor : Item
	{
		[Constructable]
		public SelfRepairArmor() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Self Repair +1 Deed";
			LootType = LootType.Blessed;
			Hue = 1289;
		}

		public SelfRepairArmor( Serial serial ) : base( serial )
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

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage("What item would you like to increase the self repair on?"  );
				from.Target = new SelfRepairArmorTarget( this );
			 }
		}	
	}
}