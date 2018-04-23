/*  _________________________________
 -=(_)_______________________________)=-
   /   .   .   . ____  . ___      _/
  /~ /    /   / /     / /   )2005 /
 (~ (____(___/ (____ / /___/     (
  \ ----------------------------- \
   \     lucidnagual@gmail.com     \
    \_     ===================      \
     \   -Admin of "The Conjuring"-  \
      \_     ===================     ~\
       )      Advanced Archery         )
      /~      Version [2].0.1        _/
    _/_______________________________/
 -=(_)_______________________________)=-
 */
using System;
using Server;
using Server.Targeting;
using Server.HuePickers;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System.Collections;
using Server.Mobiles;

using System.Collections.Generic;

namespace Server.Items
{
	public class LightningDipTub : Item
	{
		private int i_Owner, i_Charges;
		
		public int Owner { get{ return i_Owner; } set{ i_Owner = value; InvalidateProperties(); } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges { get{ return i_Charges; } set{ i_Charges = value; InvalidateProperties(); } }
		
		[Constructable]
		public LightningDipTub() : this ( 50, 0 )
		{
		}
		
		[Constructable]
		public LightningDipTub( int charges, int owner ) : base ( 0xFAB )
		{
			Name = "Lightning Dip Tub";
			Charges = charges;
			Owner = owner;
			Hue = 1174;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
			{
				if (i_Charges >= 1 )
					from.Target = new InternalTarget( this );
				
				else if ( i_Charges < 1 )
					from.SendMessage( "You don't have enough charges to lightning strike anymore." );
			}
		}
		
		public LightningDipTub( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			
			writer.Write( (int) i_Charges );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			i_Charges = reader.ReadInt();
		}
		
		public override void AddNameProperties( ObjectPropertyList list )
		{
			AddNameProperty( list );
			
			if (Charges < 1)
				list.Add("{0} without charges", Name);
			else
				list.Add("{0} with {1} charges", Name, Charges);
			
			if ( IsSecure )
				AddSecureProperty( list );
			else if ( IsLockedDown )
				AddLockedDownProperty( list );
			
			if ( DisplayLootType )
				AddLootTypeProperty( list );
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
		}
		
		
		private class InternalTarget : Target
		{
			private LightningDipTub it_Tub;
			
			public InternalTarget( LightningDipTub tub ) : base( 1, false, TargetFlags.None )
			{
				it_Tub = tub;
			}
			
			protected override void OnTarget( Mobile from, object targeted )
			{
				Item item = targeted as Item;
				Arrow arrow = targeted as Arrow;
				Bolt bolt = targeted as Bolt;
			
				if ( targeted is PlayerMobile || targeted is BaseCreature )
					from.SendMessage( "You cannot target that." );
	
				if ( targeted != null && targeted is Arrow )
				{
					if ( it_Tub != null )
					{
						int amount = arrow.Amount;
						
						from.SendMessage( "You carefully apply the electrical material to your bundle of arrows." );
							
						it_Tub.Charges--;
						arrow.Consume( amount );
						//Note: pack.ConsumeTotal( typeof(Arrow), 20 )
						from.AddToBackpack( new LightningArrowAmmo( amount ) );
						
						// if (!( item.IsChildOf(from.Backpack )))
						// {
							// from.SendMessage( "This must be in your backpack." );
						// }
						// else if ( item.IsChildOf(from.Backpack ) && arrow.Amount == 20 )
						// {
							// from.SendMessage( "You carefully apply the electrical material to your bundle of arrows." );
							
							// it_Tub.Charges--;
							// arrow.Consume( 20 );
							//Note: pack.ConsumeTotal( typeof(Arrow), 20 )
							// from.AddToBackpack( new LightningArrowAmmo( 20 ) );
						// }
						// else if ( targeted != null && arrow.Amount != 20 )
							// from.SendMessage( "You must use a bundle of 20 at a time." );
					}
					else
						return;
				}
				else if ( targeted != null && targeted is Bolt )
				{
					if ( it_Tub != null )
					{
						int amount = bolt.Amount;
						
						from.SendMessage( "You carefully apply the electrical material to your bundle of bolts." );
							
						it_Tub.Charges--;
						bolt.Consume( amount );
						//Note: pack.ConsumeTotal( typeof(Arrow), 20 )
						from.AddToBackpack( new LightningBolt( amount ) );
						
						// if (!( item.IsChildOf(from.Backpack )))
						// {
							// from.SendMessage( "This must be in your backpack." );
						// }
						// else if ( item.IsChildOf(from.Backpack ) && bolt.Amount == 20 )
						// {
							// from.SendMessage( "You carefully apply the electrical material to your bundle of bolts." );
							
							// it_Tub.Charges--;
							// bolt.Consume( 20 );
							//Note: pack.ConsumeTotal( typeof(Bolt), 20 )
							// from.AddToBackpack( new LightningBolt( 20 ) );
						// }
						// else if ( targeted != null && bolt.Amount != 20 )
							// from.SendMessage( "You must use a bundle of 20 bolts at a time." );
					}
					else
						return;
				}
				else
					from.SendMessage( "You must target a bundle of 20." );
			}
		}
	}
}
