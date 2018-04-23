//By: GoldDraco13  www.81x.com/golddraco13/dragons_of_pern
using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Mobiles 
{ 
   public class MadCrafter : BaseCreature 
   { 

	private static bool m_Talked;

	string[] kfcsay = new string[]
	{
	"Let me measure you up!!!...for your grave!!!",
	"Why do you need new stuff? I hate making new stuff!!!",
	"I have power scrolls but....not for you!!!",
	"I'll let you pick the color, Reds my favorite..."
	};

      [Constructable] 
      public MadCrafter() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 ) 
      { 
         SpeechHue = Utility.RandomDyedHue(); 
         Hue = 0x54321; 

         if ( Female = Utility.RandomBool() ) 
         { 
            Body = 401; 
            Name = "a insane crafter"; 
         } 
         else 
         { 
            Body = 400; 
            Name = "a mad crafter";
	    Hue = 2118; 
         } 

         SetStr( 200, 210 ); 
         SetDex( 95, 100 ); 
         SetInt( 25, 35 );

	 SetHits( 475, 1550 ); 

         SetDamage( 30, 40 ); 

         SetDamageType( ResistanceType.Physical, 100 ); 

         SetResistance( ResistanceType.Physical, 30, 45 ); 
         SetResistance( ResistanceType.Fire, 20, 30 ); 
         SetResistance( ResistanceType.Cold, 20, 30 ); 
         SetResistance( ResistanceType.Poison, 5, 20 ); 
         SetResistance( ResistanceType.Energy, 5, 20 ); 

         SetSkill( SkillName.Anatomy, 120.0 ); 
         SetSkill( SkillName.MagicResist, 90.0 ); 
         SetSkill( SkillName.Macing, 120.0 ); 
         SetSkill( SkillName.Tactics, 120.0 ); 

         Fame = 5000; 
         Karma = -5000; 

         new SilverBeetle().Rider = this; 

         AddItem( new PlateChest() ); 
         AddItem( new PlateArms() ); 
         AddItem( new PlateGloves() ); 
         AddItem( new PlateGorget() ); 
         AddItem( new PlateLegs() ); 
         AddItem( new WarHammer() ); 

	 Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x204A ) ); 
         hair.Hue = 1153; 
         hair.Layer = Layer.Hair; 
         hair.Movable = false; 
         AddItem( hair );
 
         PackGold( 1150, 1400);
	 //AddToBackpack( new ResourceRecipe() );
	 if( Utility.Random( 50 ) < 50 ) 
			switch ( Utility.Random( 300 )) 
			{ 
				case 0: PackItem( new PowerScroll( SkillName.Alchemy, 120 ) ); break; 
				case 1: PackItem( new PowerScroll( SkillName.Fletching, 120 ) ); break; 
				case 2: PackItem( new PowerScroll( SkillName.Carpentry, 120 ) ); break; 
				case 3: PackItem( new PowerScroll( SkillName.Cooking, 120 ) ); break; 
				case 4: PackItem( new PowerScroll( SkillName.Fishing, 120 ) ); break; 
				case 5: PackItem( new PowerScroll( SkillName.Herding, 120 ) ); break; 
				case 6: PackItem( new PowerScroll( SkillName.Lockpicking, 120 ) ); break; 
				case 7: PackItem( new PowerScroll( SkillName.Lumberjacking, 120 ) ); break; 
				case 8: PackItem( new PowerScroll( SkillName.Mining, 120 ) ); break; 
				case 9: PackItem( new PowerScroll( SkillName.RemoveTrap, 120 ) ); break; 
				case 10: PackItem( new PowerScroll( SkillName.Snooping, 120 ) ); break;  
				case 11: PackItem( new PowerScroll( SkillName.Tinkering, 120 ) ); break; 
				case 12: PackItem( new PowerScroll( SkillName.Begging, 120 ) ); break; 
				case 13: PackItem( new PowerScroll( SkillName.Camping, 120 ) ); break; 
				case 14: PackItem( new PowerScroll( SkillName.Cartography, 120 ) ); break; 
				case 15: PackItem( new PowerScroll( SkillName.DetectHidden, 120 ) ); break; 
				case 16: PackItem( new PowerScroll( SkillName.Hiding, 120 ) ); break; 
				case 17: PackItem( new PowerScroll( SkillName.Inscribe, 120 ) ); break; 
				case 18: PackItem( new PowerScroll( SkillName.Poisoning, 120 ) ); break;
                case 19: PackItem(new PowerScroll(SkillName.Imbuing, 120)); break;
			}
			SendLocalizedMessage( 1049524 ); // You have received a scroll of power!

			switch ( Utility.Random( 200 ) )
			{
				case 0: PackItem( new RunicFletchersTools( CraftResource.Heartwood, 30 ) ); break;
				case 1: PackItem( new RunicFletchersTools( CraftResource.Bloodwood, 30 ) ); break;
				case 2: PackItem( new RunicFletchersTools( CraftResource.Frostwood, 30 ) ); break;
				case 3: PackItem( new RunicHammer( CraftResource.Agapite, 30 ) ); break;
                case 4: PackItem(new RunicHammer(CraftResource.Valorite, 30)); break;
				case 5: PackItem( new RunicSewingKit( CraftResource.SpinedLeather, 30 ) ); break; // Spined Leather runic sewing kit with 10 charges
				case 6: PackItem( new RunicSewingKit( CraftResource.HornedLeather, 30 ) ); break;
				case 7: PackItem( new RunicSewingKit( CraftResource.BarbedLeather, 30 ) ); break;
                //case 8: PackItem( new RunicSewingKit( CraftResource.Valorite, 30 ) ); break;
				
						
			}
			SendMessage( "You have recieved a runic tool."); 

      } 
       
      public override bool OnBeforeDeath() 
      { 
         IMount mount = this.Mount; 

         if ( mount != null ) 
            mount.Rider = null; 

         if ( mount is Mobile ) 
            ((Mobile)mount).Delete(); 

         return base.OnBeforeDeath(); 
      } 

      public override bool AlwaysMurderer{ get{ return true; } } 

      public MadCrafter( Serial serial ) : base( serial ) 
      { 
      }


	public override void OnMovement( Mobile m, Point3D oldLocation ) 
               {                                                    
         		if( m_Talked == false ) 
        		 { 
          		 	 if ( m.InRange( this, 4 ) ) 
          			  {                
          				m_Talked = true; 
              				SayRandom( kfcsay, this ); 
				this.Move( GetDirectionTo( m.Location ) ); 
				SpamTimer t = new SpamTimer(); 
				t.Start(); 
            			} 
		} 
	} 

	private class SpamTimer : Timer 
	{ 
		public SpamTimer() : base( TimeSpan.FromSeconds( 8 ) ) 
		{ 
			Priority = TimerPriority.OneSecond; 
		} 

		protected override void OnTick() 
		{ 
		m_Talked = false; 
		} 
	} 

	private static void SayRandom( string[] say, Mobile m ) 
	{ 
		m.Say( say[Utility.Random( say.Length )] ); 
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