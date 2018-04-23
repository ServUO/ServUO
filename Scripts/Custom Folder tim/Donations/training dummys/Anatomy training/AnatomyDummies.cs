// Scripted by Lord Greywolf
// Age of Avatars
// You can use this script how ever you want
// As is, spindle it, mutalate it, change settings, what ever
// just remember if you use it in a package, or update and resubmit it, to give credit where credit is due
using System;
using Server;
using Server.Gumps; // need this because we call a gump :)

namespace Server.Items // this is an item, so easiest to make it that way, but could use any custom one we wanted
{
	[Flipable( 0x1070, 0x1074 )] // these items can be switchs from east to sout facing, using the house deco tool
	public class AnatomyDummy : AddonComponent
	{
		private double m_MinSkill; // out variable for minimum skill to use this - setting it here makes it usable through out the script
		private double m_MaxSkill; // our variable for maximum skill to use this - setting it here makes it usable through out the script

		private Timer m_Timer; // setting our base time up

		[CommandProperty( AccessLevel.GameMaster )] // setting our skill values
		public double MinSkill
		{
			get{ return m_MinSkill; }
			set{ m_MinSkill = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )] // setting our skill values
		public double MaxSkill
		{
			get{ return m_MaxSkill; }
			set{ m_MaxSkill = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )] // setting up if it is "swinging" or not - a visual way to say it is being used :)
		public bool Swinging
		{
			get{ return ( m_Timer != null ); }
		}

		[Constructable] // that it is constructable (can be made using [add or in a craft gump, etc
		public AnatomyDummy() : this( 0x1074 ){} // this is default with no parameters and sets it itemid to 0x1074

		[Constructable]
		public AnatomyDummy( int itemID ) : base( itemID ) // this take the itemid parameter, including the one from above and sets it to match
		{
			m_MinSkill = -25.0; // setting the min skill to use this
			m_MaxSkill = +25.0; // setting the max skill to use this (also same as what you can train up to)
			Name = "Anatomy Trainer"; // setting its name
		}

		public void UpdateItemID() // this is called to update its item id based on its "normal" one and where it is in motion at
		{
			int baseItemID = (ItemID / 2) * 2;
			ItemID = baseItemID + (Swinging ? 1 : 0);
		}

		public void BeginSwing() // this calls our routine and starts our swinging timer
		{
			if ( m_Timer != null ) m_Timer.Stop(); // it a timer is all ready running - stop it
			m_Timer = new InternalTimer( this ); // sets up a new timer
			m_Timer.Start(); // starts the timer
		}

		public void EndSwing() // this is called when it is done swinging :)
		{
			if ( m_Timer != null ) m_Timer.Stop(); // stops the time if there is one
			m_Timer = null; // removes the timer
			UpdateItemID(); // used to make sure the item id is correct
		}

		public void OnHit() // simulates it be hit by a finger pointing at it lol
		{
			UpdateItemID(); // udates how it looks and then plays a random sound
			Effects.PlaySound( GetWorldLocation(), Map, Utility.RandomList( 0x3A4, 0x3A6, 0x3A9, 0x3AE, 0x3B4, 0x3B6 ) );
		}

		public override void OnDoubleClick( Mobile from ) // what happens when you double click it
		{
			if ( !from.InRange( GetWorldLocation(), 1 ) ) from.SendMessage( "You need to be closer to see the question" ); // makes sure you are with in 1 step of it
			else if ( Swinging ) from.SendMessage( "Slow down, the teacher is still setting up the next question" ); // makes sure you do not use it to fast :) based off of our timer
			else if ( from.Skills.Anatomy.Base >= m_MaxSkill ) from.SendMessage( "Your Skill is to high to train here" ); // checks if skill is to high to use it
			else if ( from.Mounted ) from.SendMessage( "You can not train while mounted, the teacher does not like the mess they leave" ); // can not use if mounted :)
			else // ready to rumble lol
			{
				BeginSwing(); // starts our timer by calling the beginswing function
				from.Direction = from.GetDirectionTo( GetWorldLocation() ); // sets us so we are looking at it
				from.CloseAllGumps(); // closes all open gumps
				from.SendGump( new AnatomyTrainingGump() ); // sends our training gump :)
			}
		}

		public AnatomyDummy(Serial serial) : base(serial){} // standard serial routine
		public override void Serialize( GenericWriter writer ) // standard writing routine (when the world saves)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_MinSkill ); // remember our skill values
			writer.Write( m_MaxSkill );
		}

		public override void Deserialize( GenericReader reader ) // standard reading routine (when server starts up)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_MinSkill = reader.ReadDouble(); // sets our variables up
			m_MaxSkill = reader.ReadDouble();
			UpdateItemID(); // makes sure it is set to standard item id for "not being used"
		}

		private class InternalTimer : Timer // our timer
		{
			private AnatomyDummy m_Dummy; // reference back to our dummy
			private bool m_Delay = true; // setting the standard of true for this variable

			public InternalTimer( AnatomyDummy dummy ) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.75 ) ) // setting up our timer so it lasts 2.75 seconds long and checks every .25 seconds
			{
				m_Dummy = dummy; // setting our variable references up
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick() // every .25 seconds :)
			{
				if ( m_Delay ) m_Dummy.OnHit(); //if true - call the onhit method)
				else m_Dummy.EndSwing(); // if false, stop the timer
				m_Delay = !m_Delay; // reversing if true/false
			}
		}
	}

	public class AnatomyDummyEastAddon : BaseAddon // our actual add on and it is a base addon
	{
		public override BaseAddonDeed Deed{ get{ return new AnatomyDummyEastDeed(); } } // name of the deed

		[Constructable] // can be made/added
		public AnatomyDummyEastAddon() 
		{
			AddComponent( new AnatomyDummy( 0x1074 ), 0, 0, 0 ); // adding in our dummy from above
		}

		public AnatomyDummyEastAddon(Serial serial) : base(serial){} // standard serial/deserial routines
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class AnatomyDummyEastDeed : BaseAddonDeed // our deed for it
	{
		public override BaseAddon Addon{ get{ return new AnatomyDummyEastAddon(); } } // which item this deed makes

		[Constructable] // of course it can be made/added
		public AnatomyDummyEastDeed()
		{
			Name = "Anatomy Training Dummy (East)"; // nice name for when single clicked/mouse over
		}

		public AnatomyDummyEastDeed(Serial serial) : base(serial){} // standard serial/deserial routines
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class AnatomyDummySouthAddon : BaseAddon // as above but for opposite direction
	{
		public override BaseAddonDeed Deed{ get{ return new AnatomyDummySouthDeed(); } }

		[Constructable]
		public AnatomyDummySouthAddon()
		{
			AddComponent( new AnatomyDummy( 0x1070 ), 0, 0, 0 );
		}

		public AnatomyDummySouthAddon(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}

	public class AnatomyDummySouthDeed : BaseAddonDeed // and the deed for above
	{
		public override BaseAddon Addon{ get{ return new AnatomyDummySouthAddon(); } }

		[Constructable]
		public AnatomyDummySouthDeed()
		{
			Name = "Anatomy Training Dummy (South)";
		}

		public AnatomyDummySouthDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}
	}
}