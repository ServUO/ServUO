using System;
using Server.Gumps;
using Server.Items;
using System.Collections;
using Server.ContextMenus;
using System.Collections.Generic; 
namespace Server.Mobiles
{
	[CorpseName( "a humans corpse" )]
	public class RareHunter : Mobile
	{
		//List of hints
		public static string[] RareHints = new string[]
		{
			// OSI Rare Hints
			"In a rocky pass, Near some kirins you will find something to seat your rear.",
			"Near the yew courts in a building you will find something fruity.",
			"I hear the guy in luna makes a mean meat pie.",
			"Behind the bath house in bucs you will find something i am sure.",
			"What does a witch use? Hmmm if you figure that out you will find something nice.",
			"Outside of vesper there is a dungeon that if you go far enough into it you will find two nice items.",
			"In the heart of the undead you will find something to store you loot in.",
			"Outside the Terathan Keep on the road you will find a nice item.",
			"I hear there is a nice item on a dinner table in magincia, It tastes great too.",
			"Neljum has a big house, You must try thier fruit.",
			"Life is a maze.. no no life is like a fruit basket.. never know what you get till you eat it all up.",

			//Custom Rare Hints
			"South of Luna, In a field of crystals you will find something nice.",
			"Near an old tome outside of Umbra there is a nice item to find.",
			"East side of Haven you will find something to light up your world.",
			"Go rest, You look tried. I hear Haven has a nice inn.",
			"There is a snake in the city of Zento hes upstairs.",
			"In a swampy building of Tokuno you will find a list.",
			"Outside of the ankh dungeon i hear you can find something out of place.",
			"Minax has a warm spot in her heart, You should search her stronghold for something bright.",
			"Meers have terriable furniture i hear.",
			"On some red steps in Zento you will find some nice fish.",
			"On an island... Its a SHAME to place something so nice their.",
			"In Ilshenar, In a place called the ancient citadel you will find something nice for your windows.",
			"I was in a hostal last week, Found something real nasty there.",
			"Yew has some of the best woodworkers in all the land.",
			"In a little cove you will find a nice item if you search hard enough.",
			"Outside of moonglow there is a house with a very nice item just gathering dust.",
			"In north minoc you will find a stinky mess.",
			"In the serpents mouth the archer fired his arrow.",
			"Skara Brae has a hard time with there crops due to all the pests in that area.",
			"There was a big tree in the middle of yew at one time.",
			"There is a building across the waters in yew, You might find a good book there."
		};

                public virtual bool IsInvulnerable{ get{ return true; } }

		[Constructable]
		public RareHunter()
		{
			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}

			Title = "the rare hunter";

			AddItem( new HoodedShroudOfShadows() );
			AddItem( new BlackStaff() );

			Hue = Utility.RandomSkinHue();
		}

		public RareHunter( Serial serial ) : base( serial )
		{
		}
            public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list ) 
		//public override void GetContextMenuEntries( Mobile from, ArrayList list ) 
	      { 
	              base.GetContextMenuEntries( from, list ); 
        	        list.Add( new RareHunterEntry( from, this ) ); 
	      } 

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class RareHunterEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Hunter;
			
			public RareHunterEntry( Mobile from, Mobile hunter ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Hunter = hunter;
			}

			public override void OnClick()
			{
				if( !( m_Mobile is PlayerMobile ) )
					return;

				m_Hunter.Say( "For 5000 gold ill give you a hint to a location where you can find something nice." );
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
			{
				if ( dropped.Amount == 5000 )
				{
					string hint = RareHints[Utility.Random(RareHints.Length)];
					this.SayTo( from, hint );
					dropped.Delete();
					return true;
				}
				else if ( dropped.Amount > 5000 )
				{
					string hint = RareHints[Utility.Random(RareHints.Length)];
					this.SayTo( from, hint );
					
					int payback = dropped.Amount - 5000;
					from.AddToBackpack( new Gold( payback ) );
					dropped.Delete();
					return true;
				}
				else
				{
					this.SayTo( from, "I need a little more than that." );
					return false;
				}	
			}
			else if ( dropped is BankCheck )
			{
				this.SayTo( from, "Sorry i only take gold, No checks" );
				return false;
			}
			else
			{
				this.SayTo( from, "That does not interest me." );
				return false;
			} 
		}
	}
}
				


				


