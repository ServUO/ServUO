//Emote v2 by CMonkey123
/*
v2 changes:
	-Shortened script
	-Added emotes (thanks to zire):
		bow, faint, punch, slap, stickouttongue, tapfoot
	-Added emote gump (thanks to zire)
*/
/* Emote v3 by GM Jubal from Ebonspire http://www.ebonspire.com
 * I Left the above comments in here for credit to properly go back to those whom originally wrote this
 * I simply made it so that the [e command would call the gump if used by itself or if the <sound> was
 * misspelled, shortened the code down from 1300+ lines down to only 635 lines including these comments.
 * Also fixed a couple of typos in the script.
 * This has been tested on both RunUO beta .36 and RunUO RC0 1.0
*/
/* Emote v4 by Lysdexic
 * Updated for RunUO 2.0 RC2
 * Puke command could be used for teleport bug... removed that ability.
 * Typos again... 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Commands.Generic;

namespace Server.Commands
{
	public enum EmotePage 
	{ 
		P1,
		P2,
		P3,
		P4,
	}
	public class Emote
	{	
		public static void Initialize()
		{
			CommandSystem.Register( "emote", AccessLevel.Player, new CommandEventHandler( Emote_OnCommand ) );
			CommandSystem.Register( "e", AccessLevel.Player, new CommandEventHandler( Emote_OnCommand ) );
		}

	  	[Usage( "<sound>" )] 
	      	[Description( "Emote with sounds, words, and possibly an animation with one command!")] 
		private static void Emote_OnCommand( CommandEventArgs e )
		{
			Mobile pm = e.Mobile;
        	 	string em = e.ArgString.Trim();
			int SoundInt;
			switch( em )
			{
				case "ah":
					SoundInt = 1;
					break;
				case "ahha":
					SoundInt = 2;
					break;					
				case "applaud":
					SoundInt = 3;
					break;				
				case "blownose":
					SoundInt = 4;
					break;					
				case "bow":
					SoundInt = 5;
					break;
				case "bscough":
					SoundInt = 6;
					break;
				case "burp":
					SoundInt = 7;
					break;
				case "clearthroat":
					SoundInt = 8;
					break;
				case "cough":
					SoundInt = 9;
					break;
				case "cry":
					SoundInt = 10;
					break;
				case "faint":					
					SoundInt = 11;
					break;
				case "fart":
					SoundInt = 12;
					break;
				case "gasp":
					SoundInt = 13;
					break;
				case "giggle":
					SoundInt = 14;
					break;
				case "groan":
					SoundInt = 15;
					break;
				case "growl":
					SoundInt = 16;
					break;
				case "hey":
					SoundInt = 17;
					break;
				case "hiccup":
					SoundInt = 18;
					break;
				case "huh":
					SoundInt = 19;
					break;
				case "kiss":
					SoundInt = 20;
					break;
				case "laugh":
					SoundInt = 21;
					break;
				case "no":
					SoundInt = 22;
					break;
				case "oh":
					SoundInt = 23;
					break;
				case "oooh":
					SoundInt = 24;
					break;
				case "oops":
					SoundInt = 25;
					break;
				case "puke":
					SoundInt = 26;
					break;
				case "punch": 					
					SoundInt = 27;
					break;
				case "scream":
					SoundInt = 28;
					break;
				case "shush":
					SoundInt = 29;
					break;
				case "sigh":
					SoundInt = 30;
					break;
				case "slap":
					SoundInt = 31;
					break;
				case "sneeze":
					SoundInt = 32;
					break;
				case "sniff":
					SoundInt = 33;
					break;
				case "snore":
					SoundInt = 34;
					break;
				case "spit":
					SoundInt = 35;
					break;
				case "stickouttongue":
					SoundInt = 36;
					break;
				case "tapfoot":
					SoundInt = 37;
					break;
				case "whistle":
					SoundInt = 38;
					break;
				case "woohoo":
					SoundInt = 39;
					break;
				case "yawn":
					SoundInt = 40;
					break;
				case "yea":
					SoundInt = 41;
					break;
				case "yell":
					SoundInt = 42;
					break;				
				default:
					SoundInt = 0;
					e.Mobile.SendGump( new EmoteGump( e.Mobile, EmotePage.P1) );
					break;
			}
			if ( SoundInt > 0 )
				new ESound( pm, SoundInt );
		} 
	}
	public class EmoteGump : Gump 
	{ 
		private Mobile m_From; 
		private EmotePage m_Page; 
		private const int Blanco = 0xFFFFFF; 
		private const int Azul = 0x8080FF; 
		public void AddPageButton( int x, int y, int buttonID, string text, EmotePage page, params EmotePage[] subpage ) 
		{ 
			bool seleccionado = ( m_Page == page ); 
			for ( int i = 0; !seleccionado && i < subpage.Length; ++i ) 
			seleccionado = ( m_Page == subpage[i] ); 
			AddButton( x, y - 1, seleccionado ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0 ); 
			AddHtml( x + 35, y, 200, 20, Color( text, seleccionado ? Azul : Blanco ), false, false ); 
		} 
		public void AddButtonLabeled( int x, int y, int buttonID, string text ) 
		{ 
			AddButton( x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0 ); 
			AddHtml( x + 35, y, 240, 20, Color( text, Blanco ), false, false ); 
		} 
		public int GetButtonID( int type, int index ) 
		{ 
			return 1 + (index * 15) + type; 
		} 
		public string Color( string text, int color ) 
		{ 
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text ); 
		} 
		public EmoteGump ( Mobile from, EmotePage page) : base ( 600, 50 ) 
		{ 
			from.CloseGump( typeof( EmoteGump ) ); 
			m_From = from; 
			m_Page = page; 
			Closable = true; 
			Dragable = true; 
			AddPage( 0 );
			AddBackground( 0, 65, 130, 360, 5054);
			AddAlphaRegion( 10, 70, 110, 350 );
			AddImageTiled( 10, 70, 110, 20, 9354);
			AddLabel( 13, 70, 200, "Emote List");
			AddImage( 100, 0, 10410);
			AddImage( 100, 305, 10412);
			AddImage( 100, 150, 10411);
			switch ( page ) 
			{
				case EmotePage.P1:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 1 ), "Ah");
					AddButtonLabeled( 10, 115, GetButtonID( 1, 2 ), "Ah-ha");
					AddButtonLabeled( 10, 140, GetButtonID( 1, 3 ), "Applaud");
					AddButtonLabeled( 10, 165, GetButtonID( 1, 4 ), "Blows Nose");
					AddButtonLabeled( 10, 190, GetButtonID( 1, 5 ), "Bows");
					AddButtonLabeled( 10, 215, GetButtonID( 1, 6 ), "BS Cough");
					AddButtonLabeled( 10, 240, GetButtonID( 1, 7 ), "Burp");
					AddButtonLabeled( 10, 265, GetButtonID( 1, 8 ), "Clear Throat");
					AddButtonLabeled( 10, 290, GetButtonID( 1, 9 ), "Cough");
					AddButtonLabeled( 10, 315, GetButtonID( 1, 10 ), "Cry");
					AddButtonLabeled( 10, 340, GetButtonID( 1, 11 ), "Faints");
					AddButtonLabeled( 10, 365, GetButtonID( 1, 12 ), "Fart");
					AddButton( 70, 380, 4502, 0504, GetButtonID( 0,2 ), GumpButtonType.Reply, 0 );
					break; 
				}
				case EmotePage.P2:
				{ 
					AddButtonLabeled( 10, 90, GetButtonID( 1, 13 ), "Gasp");
					AddButtonLabeled( 10, 115, GetButtonID( 1, 14 ), "Giggle");
					AddButtonLabeled( 10, 140, GetButtonID( 1, 15 ), "Groan");
					AddButtonLabeled( 10, 165, GetButtonID( 1, 16 ), "Growl");
					AddButtonLabeled( 10, 190, GetButtonID( 1, 17 ), "Hey");
					AddButtonLabeled( 10, 215, GetButtonID( 1, 18 ), "Hiccup");
					AddButtonLabeled( 10, 240, GetButtonID( 1, 19 ), "Huh");
					AddButtonLabeled( 10, 265, GetButtonID( 1, 20 ), "Kiss");
					AddButtonLabeled( 10, 290, GetButtonID( 1, 21 ), "Laughs");
					AddButtonLabeled( 10, 315, GetButtonID( 1, 22 ), "No");
					AddButtonLabeled( 10, 340, GetButtonID( 1, 23 ), "Oh");
					AddButtonLabeled( 10, 365, GetButtonID( 1, 24 ), "Oooh");
					AddButton( 10, 380, 4506, 4508, GetButtonID( 0,1 ), GumpButtonType.Reply, 0 );
					AddButton( 70, 380, 4502, 0504, GetButtonID( 0,3 ), GumpButtonType.Reply, 0 );
					break; 
				} 
				case EmotePage.P3:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 25 ), "Oops");
					AddButtonLabeled( 10, 115, GetButtonID( 1, 26 ), "Puke");
					AddButtonLabeled( 10, 140, GetButtonID( 1, 27 ), "Punch");
					AddButtonLabeled( 10, 165, GetButtonID( 1, 28 ), "Scream");
					AddButtonLabeled( 10, 190, GetButtonID( 1, 29 ), "Shush");
					AddButtonLabeled( 10, 215, GetButtonID( 1, 30 ), "Sigh");
					AddButtonLabeled( 10, 240, GetButtonID( 1, 31 ), "Slap");
					AddButtonLabeled( 10, 265, GetButtonID( 1, 32 ), "Sneeze");
					AddButtonLabeled( 10, 290, GetButtonID( 1, 33 ), "Sniff");
					AddButtonLabeled( 10, 315, GetButtonID( 1, 34 ), "Snore");
					AddButtonLabeled( 10, 340, GetButtonID( 1, 35 ), "Spit");
					AddButtonLabeled( 10, 365, GetButtonID( 1, 36 ), "Sticks Tongue");
					AddButton( 10, 380, 4506, 4508, GetButtonID( 0,2 ), GumpButtonType.Reply, 0 );
					AddButton( 70, 380, 4502, 0504, GetButtonID( 0,4 ), GumpButtonType.Reply, 0 );
					break; 
				} 
				case EmotePage.P4:
				{
					AddButtonLabeled( 10, 90, GetButtonID( 1, 37 ), "Taps Foot");
					AddButtonLabeled( 10, 115, GetButtonID( 1, 38 ), "Whistle");
					AddButtonLabeled( 10, 140, GetButtonID( 1, 39 ), "Woohoo");
					AddButtonLabeled( 10, 165, GetButtonID( 1, 40 ), "Yawn");
					AddButtonLabeled( 10, 190, GetButtonID( 1, 41 ), "Yea");
					AddButtonLabeled( 10, 215, GetButtonID( 1, 42 ), "Yell");
					AddButton( 10, 380, 4506, 4508, GetButtonID( 0,3 ), GumpButtonType.Reply, 0 );
					break; 
				} 
			} 
		} 
		public override void OnResponse( Server.Network.NetState sender, RelayInfo info ) 
		{ 
			int val = info.ButtonID - 1; 
			if ( val < 0 ) 
			return; 
	
			Mobile from = m_From;
			int type = val % 15; 
			int index = val / 15; 
	
			switch ( type )
			{
				case 0:
				{
					EmotePage page;
						switch ( index )
					{
						case 1: page = EmotePage.P1; break;
						case 2: page = EmotePage.P2; break;
						case 3: page = EmotePage.P3; break;
						case 4: page = EmotePage.P4; break;
						default: return;
					}
	
					from.SendGump( new EmoteGump( from, page) );
					break;
				}
				case 1:
				{
					if ( index > 0 && index < 13 )
					{
						from.SendGump( new EmoteGump( from, EmotePage.P1) );
					}
					else if ( index > 12 && index < 25 )
					{
						from.SendGump( new EmoteGump( from, EmotePage.P2) );
					}
					else if ( index > 24 && index < 37 )
					{
						from.SendGump( new EmoteGump( from, EmotePage.P3) );
					}
					else if ( index > 36 && index < 43 )
					{
						from.SendGump( new EmoteGump( from, EmotePage.P4) );
					}
					new ESound( from, index );
					break; 
				} 
			} 
		}
	}
	public class ItemRemovalTimer : Timer 
	{ 
		private Item i_item; 
		public ItemRemovalTimer( Item item ) : base( TimeSpan.FromSeconds( 1.0 ) ) 
		{ 
			Priority = TimerPriority.OneSecond; 
			i_item = item; 
		} 

		protected override void OnTick() 
		{ 
		        if (( i_item != null ) && ( !i_item.Deleted ))
		        { 
			        i_item.Delete();
			        Stop();
		        }
		} 
	} 

	public class Puke : Item 
	{
		private Timer m_Timer;
		 
		[Constructable] 
		public Puke() : base( Utility.RandomList( 0xf3b, 0xf3c ) ) 
		{ 
			Name = "A Pile of Puke"; 
			Hue = 0x557; 
			Movable = false; 

			m_Timer = new ItemRemovalTimer( this ); 
			m_Timer.Start(); 

		} 
		
		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if ( m_Timer != null )
				m_Timer.Stop();
		}

		public override void OnSingleClick( Mobile from ) 
		{ 
			this.LabelTo( from, this.Name ); 
		} 
  
		public Puke( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize(GenericWriter writer) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize(GenericReader reader) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 

			this.Delete(); // none when the world starts 
		} 
	}
	
	public class ESound
	{
		public ESound( Mobile pm, int SoundMade )
		{
			switch( SoundMade )
			{
				case 1:
					pm.PlaySound( pm.Female ? 778 : 1049 );
					pm.Say( "*ah!*" );
					break;
				case 2:
					pm.PlaySound( pm.Female ? 779 : 1050 );
					pm.Say( "*ah ha!*" );
					break;
				case 3:
					pm.PlaySound( pm.Female ? 780 : 1051 );
					pm.Say( "*applauds*" );
					break;
				case 4:
					pm.PlaySound( pm.Female ? 781 : 1052 );
					pm.Say( "*blows nose*" );				
					if ( !pm.Mounted )
						pm.Animate( 34, 5, 1, true, false, 0 );
					break;
				case 5:
					pm.Say( "*bows*" );
					if ( !pm.Mounted )
						pm.Animate( 32, 5, 1, true, false, 0 );
					break;
				case 6:
					pm.PlaySound( pm.Female ? 786 : 1057 );
					pm.Say( "*bs cough*" );
					break;
				case 7:
					pm.PlaySound( pm.Female ? 782 : 1053 );
					pm.Say( "*burp!*" );
					if ( !pm.Mounted )
						pm.Animate( 33, 5, 1, true, false, 0 );
					break;
				case 8:
					pm.PlaySound( pm.Female ? 748 : 1055 );
					pm.Say( "*clears throat*" );
					if ( !pm.Mounted )
						pm.Animate( 33, 5, 1, true, false, 0 );
					break;
				case 9:
					pm.PlaySound( pm.Female ? 785 : 1056 );
					pm.Say( "*cough!*" );				
					if ( !pm.Mounted )
						pm.Animate( 33, 5, 1, true, false, 0 );
					break;
				case 10:
					pm.PlaySound( pm.Female ? 787 : 1058 );
					pm.Say( "*cries*" );
					break;
				case 11:
					pm.PlaySound( pm.Female ? 791 : 1063 );
					pm.Say( "*faints*" );
					if ( !pm.Mounted )
						pm.Animate( 22, 5, 1, true, false, 0 );
					break;
				case 12:
					pm.PlaySound( pm.Female ? 792 : 1064 );
					pm.Say( "*farts*" );
					break;
				case 13:
					pm.PlaySound( pm.Female ? 793 : 1065 );
					pm.Say( "*gasp!*" );
					break;
				case 14:
					pm.PlaySound( pm.Female ? 794 : 1066 );
					pm.Say( "*giggles*" );
					break;
				case 15:
					pm.PlaySound( pm.Female ? 795 : 1067 );
					pm.Say( "*groans*" );
					break;
				case 16:
					pm.PlaySound( pm.Female ? 796 : 1068 );
					pm.Say( "*growls*" );
					break;
				case 17:
					pm.PlaySound( pm.Female ? 797 : 1069 );
					pm.Say( "*hey!*" );
					break;
				case 18:
					pm.PlaySound( pm.Female ? 798 : 1070 );
					pm.Say( "*hiccup!*" );
					break;
				case 19:
					pm.PlaySound( pm.Female ? 799 : 1071 );
					pm.Say( "*huh?*" );
					break;
				case 20:
					pm.PlaySound( pm.Female ? 800 : 1072 );
					pm.Say( "*kisses*" );
					break;
				case 21:
					pm.PlaySound( pm.Female ? 801 : 1073 );
					pm.Say( "*laughs*" );
					break;
				case 22:
					pm.PlaySound( pm.Female ? 802 : 1074 );
					pm.Say( "*no!*" );
					break;
				case 23:
					pm.PlaySound( pm.Female ? 803 : 1075 );
					pm.Say( "*oh!*" );
					break;
				case 24:
					pm.PlaySound( pm.Female ? 811 : 1085 );
					pm.Say( "*oooh*" );
					break;
				case 25:
					pm.PlaySound( pm.Female ? 812 : 1086 );
					pm.Say( "*oops*" );
					break;
				case 26:
					pm.PlaySound( pm.Female ? 813 : 1087 );
					pm.Say( "*pukes*" );
		            		if ( !pm.Mounted ) 
              					pm.Animate( 32, 5, 1, true, false, 0 ); 
            				Point3D p = new Point3D( pm.Location ); 
		            		switch( pm.Direction ) 
            				{ 
               					case Direction.North: 
							p.Y--; break;							 
		               			case Direction.South: 
							p.Y++; break;							 
               					case Direction.East: 
                  					p.X++; break;							 
		               			case Direction.West: 
                		  			p.X--; break; 							
               					case Direction.Right: 
		                  			p.X++; p.Y--; break;
               					case Direction.Down: 
                  					p.X++; p.Y++; break;
		               			case Direction.Left: 
                		  			p.X--; p.Y++; break;
               					case Direction.Up: 
		                  			p.X--; p.Y--; break;
		               			default: 
                		  			break; 
					} 
       				 	p.Z = pm.Map.GetAverageZ( p.X, p.Y );
       				 	
       				 	bool canFit = Server.Spells.SpellHelper.AdjustField( ref p, pm.Map, 12, false );
       				 	
       				 	if ( canFit ) 
                    	{
		            		Puke puke = new Puke(); 
            				puke.Map = pm.Map; 
		            		puke.Location = p;
	            		}
	            		/*else
	            			pm.SendMessage( "your puke won't fit!" ); /* Debug testing */
					break;
				case 27:
					pm.PlaySound( 315 );
					pm.Say( "*punches*" );
					if ( !pm.Mounted )
						pm.Animate( 31, 5, 1, true, false, 0 );
					break;
				case 28:
					pm.PlaySound( pm.Female ? 814 : 1088 );
					pm.Say( "*ahhhh!*" );
					break;
				case 29:
					pm.PlaySound( pm.Female ? 815 : 1089 );
					pm.Say( "*shhh!*" );
					break;
				case 30:
					pm.PlaySound( pm.Female ? 816 : 1090 );
					pm.Say( "*sigh*" );
					break;
				case 31:
					pm.PlaySound( 948 );
					pm.Say( "*slaps*" );
					if ( !pm.Mounted )
						pm.Animate( 11, 5, 1, true, false, 0 );
					break;
				case 32:
					pm.PlaySound( pm.Female ? 817 : 1091 );
					pm.Say( "*ahh-choo!*" );
					if ( !pm.Mounted )
						pm.Animate( 32, 5, 1, true, false, 0 );
					break;
				case 33:
					pm.PlaySound( pm.Female ? 818 : 1092 );
					pm.Say( "*sniff*" );
					if( !pm.Mounted )
						pm.Animate( 34, 5, 1, true, false, 0 );
					break;
				case 34:
					pm.PlaySound( pm.Female ? 819 : 1093 );
					pm.Say( "*snore*" );
					break;
				case 35:
					pm.PlaySound( pm.Female ? 820 : 1094 );
					pm.Say( "*spits*" );
					if ( !pm.Mounted )
						pm.Animate( 6, 5, 1, true, false, 0 );
					break;
				case 36:
					pm.PlaySound( 792 );
					pm.Say( "*sticks out tongue*" );
					break;
				case 37:
					pm.PlaySound( 874 );
					pm.Say( "*taps foot*" );
					if ( !pm.Mounted )
						pm.Animate( 38, 5, 1, true, false, 0 );
					break;
				case 38:
					pm.PlaySound( pm.Female ? 821 : 1095 );	
					pm.Say( "*whistles*" );
					if ( !pm.Mounted )
						pm.Animate( 5, 5, 1, true, false, 0 );
					break;
				case 39:
					pm.PlaySound( pm.Female ? 783 : 1054 );
					pm.Say( "*woohoo!*" );
					break;
				case 40:
					pm.PlaySound( pm.Female ? 822 : 1096 );
					pm.Say( "*yawns*" );
					if ( !pm.Mounted )
						pm.Animate( 17, 5, 1, true, false, 0 );
					break;
				case 41:
					pm.PlaySound( pm.Female ? 823 : 1097 );
					pm.Say( "*yea!*" );
					break;
				case 42:
					pm.PlaySound( pm.Female ? 823 : 1098 );
					pm.Say( "*yells*" );
					break;
			}
		}
	}
}
