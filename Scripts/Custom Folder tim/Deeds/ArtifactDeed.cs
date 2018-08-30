///////////////////
//    By Nerun   //
// Avatar System //
//    v.0.9.3    //
///////////////////
using System; 
using System.Net; 
using Server; 
using Server.Accounting; 
using Server.Gumps; 
using Server.Items; 
using Server.Mobiles; 
using Server.Network; 

namespace Server.Items
{

	public class ArtifactDeed : Item
	{

		[Constructable]
		public ArtifactDeed() : this( null )
		{
		}

		[Constructable]
		public ArtifactDeed ( string name ) : base ( 0x14F0 )
		{
			Name = "Artifact Deed";
			LootType = LootType.Blessed;
			Hue = 1172;
		}

		public ArtifactDeed ( Serial serial ) : base ( serial )
		{
		}

      		public override void OnDoubleClick( Mobile from ) 
      		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 );
			}
			else
			{ 
				from.SendGump( new ArtifactGump( from, this ) ); 
			}
		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}

namespace Server.Gumps 
{ 
   public class ArtifactGump : Gump 
   { 
      private Mobile m_Mobile;
      private Item m_Deed;
 

      public ArtifactGump( Mobile from, Item deed ) : base( 30, 20 ) 
      { 
         m_Mobile = from;
	 m_Deed = deed; 
	
	 AddPage( 1 ); 

	 AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5120 ); 

         AddLabel( 40, 12, 37, "Artifact List" );  

         Account a = from.Account as Account; 


         AddLabel( 52, 40, 37, "Weapons" ); 
         AddButton( 12, 40, 4005, 4007, 0, GumpButtonType.Page, 2 ); 
         AddLabel( 52, 60, 37, "Armor" ); 
         AddButton( 12, 60, 4005, 4007, 0, GumpButtonType.Page, 3 ); 
         AddLabel( 52, 80, 37, "Jewelery" ); 
         AddButton( 12, 80, 4005, 4007, 10, GumpButtonType.Page, 4 ); 
         AddLabel( 52, 100, 37, "Shields" ); 
         AddButton( 12, 100, 4005, 4007, 0, GumpButtonType.Page, 5 );
         AddLabel( 52, 120, 37, "Hats & Masks" ); 
         AddButton( 12, 120, 4005, 4007, 0, GumpButtonType.Page, 6 );  
         AddLabel( 52, 360, 37, "Close" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Reply, 0 );
	
	 AddPage( 2 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5120 ); 

	 AddLabel( 40, 12, 37, "Weapons List" );
        	
          

         AddLabel( 52, 40, 37, "Axe of the Heavens" ); 
         AddButton( 12, 40, 4005, 4007, 1, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Blade of Insanity" ); 
         AddButton( 12, 60, 4005, 4007, 2, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Blade of the Righteous" ); 
         AddButton( 12, 80, 4005, 4007, 3, GumpButtonType.Reply, 3 ); 
         AddLabel( 52, 100, 37, "Bone Crusher" ); 
         AddButton( 12, 100, 4005, 4007, 4, GumpButtonType.Reply, 4 ); 
         AddLabel( 52, 120, 37, "Breath of the Dead" ); 
         AddButton( 12, 120, 4005, 4007, 5, GumpButtonType.Reply, 5 ); 
         AddLabel( 52, 140, 37, "Frostbringer" ); 
         AddButton( 12, 140, 4005, 4007, 6, GumpButtonType.Reply, 6 ); 
         AddLabel( 52, 160, 37, "Legacy of the Dread Lord" ); 
         AddButton( 12, 160, 4005, 4007, 7, GumpButtonType.Reply, 7 ); 
         AddLabel( 52, 180, 37, "Serpent's Fang" ); 
         AddButton( 12, 180, 4005, 4007, 8, GumpButtonType.Reply, 8 ); 
         AddLabel( 52, 200, 37, "Staff of the Magi" ); 
         AddButton( 12, 200, 4005, 4007, 9, GumpButtonType.Reply, 9 ); 
         AddLabel( 52, 220, 37, "The Beserker's Maul" ); 
         AddButton( 12, 220, 4005, 4007, 10, GumpButtonType.Reply, 10 ); 
         AddLabel( 52, 240, 37, "The Dragon Slayer" ); 
         AddButton( 12, 240, 4005, 4007, 11, GumpButtonType.Reply, 11 );  
         AddLabel( 52, 260, 37, "Titans Hammer" ); 
         AddButton( 12, 260, 4005, 4007, 12, GumpButtonType.Reply, 12 );
         AddLabel( 52, 280, 37, "The Taskmaster" ); 
         AddButton( 12, 280, 4005, 4007, 13, GumpButtonType.Reply, 13 );
         AddLabel( 52, 300, 37, "Zyronic Claw" ); 
         AddButton( 12, 300, 4005, 4007, 14, GumpButtonType.Reply, 14 );
         AddLabel( 52, 320, 37, "The Dryad Bow" );
         AddButton( 12, 320, 4005, 4007, 15, GumpButtonType.Reply, 15 );


         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 ); 
	

         AddPage( 3 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5120 ); 

         AddLabel( 40, 12, 37, "Armor List" ); 

         
         AddLabel( 52, 40, 37, "Armor of Fortune" ); 
         AddButton( 12, 40, 4005, 4007, 16, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Gauntlets of Nobility" ); 
         AddButton( 12, 60, 4005, 4007, 17, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Helm of Insight" ); 
         AddButton( 12, 80, 4005, 4007, 18, GumpButtonType.Reply, 3 ); 
         AddLabel( 52, 100, 37, "Holy Knight's Breastplate" ); 
         AddButton( 12, 100, 4005, 4007, 19, GumpButtonType.Reply, 4 ); 
         AddLabel( 52, 120, 37, "Jackal's Collar" ); 
         AddButton( 12, 120, 4005, 4007, 20, GumpButtonType.Reply, 5 ); 
         AddLabel( 52, 140, 37, "Leggings of Bane" ); 
         AddButton( 12, 140, 4005, 4007, 21, GumpButtonType.Reply, 6 ); 
         AddLabel( 52, 160, 37, "Midnight Bracers" ); 
         AddButton( 12, 160, 4005, 4007, 22, GumpButtonType.Reply, 7 ); 
         AddLabel( 52, 180, 37, "Ornate Crown of the Harrower" ); 
         AddButton( 12, 180, 4005, 4007, 23, GumpButtonType.Reply, 8 ); 
         AddLabel( 52, 200, 37, "Shadow Dancer Leggings" ); 
         AddButton( 12, 200, 4005, 4007, 24, GumpButtonType.Reply, 9 ); 
         AddLabel( 52, 220, 37, "The Inquisitor's Resolution" ); 
         AddButton( 12, 220, 4005, 4007, 25, GumpButtonType.Reply, 10 ); 
         AddLabel( 52, 240, 37, "Tunic of Fire" ); 
         AddButton( 12, 240, 4005, 4007, 26, GumpButtonType.Reply, 11 ); 
         AddLabel( 52, 260, 37, "Voice of the Fallen King" ); 
         AddButton( 12, 260, 4005, 4007, 27, GumpButtonType.Reply, 12 ); 
         

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
	 

	 AddPage( 4 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5120 );  

         AddLabel( 40, 12, 37, "Jewelery List" ); 

         

         AddLabel( 52, 40, 37, "Bracelet of Health" ); 
         AddButton( 12, 40, 4005, 4007, 29, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Ornament of the Magician" ); 
         AddButton( 12, 60, 4005, 4007, 30, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Ring of the Elements" ); 
         AddButton( 12, 80, 4005, 4007, 31, GumpButtonType.Reply, 3 ); 
         AddLabel( 52, 100, 37, "Ring of the Vile" );
	 AddButton( 12, 100, 4005, 4007, 32, GumpButtonType.Reply, 4 ); 

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );
	  

	 AddPage( 5 ); 
  
         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5120 );

         AddLabel( 40, 12, 37, "Shields List" ); 

         

         AddLabel( 52, 40, 37, "Ægis" ); 
         AddButton( 12, 40, 4005, 4007, 34, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Arcane Shield" ); 
         AddButton( 12, 60, 4005, 4007, 35, GumpButtonType.Reply, 2 );  

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );


	 AddPage( 6 ); 

         AddBackground( 0, 0, 300, 400, 3000 ); 
         AddBackground( 8, 8, 284, 384, 5120 );  

         AddLabel( 40, 12, 37, "Hats & Masks List" ); 

         

         AddLabel( 52, 40, 37, "Divine Countenance" ); 
         AddButton( 12, 40, 4005, 4007, 36, GumpButtonType.Reply, 1 ); 
         AddLabel( 52, 60, 37, "Hat of the Magi" ); 
         AddButton( 12, 60, 4005, 4007, 37, GumpButtonType.Reply, 2 ); 
         AddLabel( 52, 80, 37, "Hunters Headdress" ); 
         AddButton( 12, 80, 4005, 4007, 38, GumpButtonType.Reply, 3 ); 
         AddLabel( 52, 100, 37, "Spirit of the Totem" );
	 AddButton( 12, 100, 4005, 4007, 39, GumpButtonType.Reply, 4 ); 

         AddLabel( 52, 360, 37, "Main Menu" ); 
         AddButton( 12, 360, 4005, 4007, 0, GumpButtonType.Page, 1 );



      } 


      public override void OnResponse( NetState state, RelayInfo info ) 
      { 
         Mobile from = state.Mobile; 

         switch ( info.ButtonID ) 
         { 
            case 0: //Close Gump 
            { 
               from.CloseGump( typeof( ArtifactGump ) );	 
               break; 
            } 
             case 1: // Axe of the Heavens 
            { 
		Item item = new AxeOfTheHeavens();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 2: // Blade of insanity 
            { 
		Item item = new BladeOfInsanity();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 3: //Blade of the Righteous
            { 
		Item item = new BladeOfTheRighteous();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 4: //Bone Crusher 
            { 
		Item item = new BoneCrusher();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 5: //Breath of the Dead 
            { 
		Item item = new BreathOfTheDead();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 6: //Frostbringer 
            { 
		Item item = new Frostbringer();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 7: //Legacy of the Dread Lord
            { 
		Item item = new LegacyOfTheDreadLord();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
            case 8: //Serpent's Fang 
            { 
		Item item = new SerpentsFang();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 9: //Staff of the Magi
	    { 
		Item item = new StaffOfTheMagi();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 10: //The Beserker's Maul 
            { 
		Item item = new TheBeserkersMaul();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 11: //The Dragon Slayer 
            { 
		Item item = new TheDragonSlayer();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 12: //Titans Hammer 
            { 
		Item item = new TitansHammer();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 13: //The Taskmaster 
            { 
		Item item = new TheTaskmaster();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 14: //Zyronic Claw
            { 
		Item item = new ZyronicClaw();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
        case 15: //The Dryad Bow
            {
        Item item = new TheDryadBow();
        item.LootType = LootType.Blessed;
        from.AddToBackpack( item );
        from.CloseGump( typeof( ArtifactGump ) );
        m_Deed.Delete();
        break;
            } 
	    case 16: //Armor of Fortune 
            { 
		Item item = new ArmorOfFortune();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 17: //Gauntlets of Nobility 
            { 
		Item item = new GauntletsOfNobility();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
 	    case 18: //Helm of Insight 
            { 
		Item item = new HelmOfInsight();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 19: //Holy Knights Breastplate 
            { 
		Item item = new HolyKnightsBreastplate();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 20: //Jackal's Collar 
            { 
		Item item = new JackalsCollar();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 21: //Leggings of Bane 
            { 
		Item item = new LeggingsOfBane();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 22: //Midnight Bracers 
            { 
		Item item = new MidnightBracers();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 23: //Ornate Crown of the Harrower 
            { 
		Item item = new OrnateCrownOfTheHarrower();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 24: //Shadow Dancer Leggings 
            { 
		Item item = new ShadowDancerLeggings();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 25: //Inquisitor's Resolution 
            { 
		Item item = new InquisitorsResolution();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 26: //Tunic of Fire 
            { 
		Item item = new TunicOfFire();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 27: //Voice of the Fallen King 
            { 
		Item item = new VoiceOfTheFallenKing();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            
            } 
	    case 29: //Bracelet of Health 
            { 
		Item item = new BraceletOfHealth();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 30: //Ornament of the Magician 
            { 
		Item item = new OrnamentOfTheMagician();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 31: //Ring of the Elements 
            { 
		Item item = new RingOfTheElements();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	    case 32: //Ring of the Vile 
            { 
		Item item = new RingOfTheVile();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 34: //Aegis 
            { 
		Item item = new Aegis();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 35: //Arcane Shield 
            { 
		Item item = new ArcaneShield();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 36: //Divine Countenance 
            { 
		Item item = new DivineCountenance();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 37: //Hat of the Magi
            { 
		Item item = new HatOfTheMagi();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 38: //Hunters Headdress 
            { 
		Item item = new HuntersHeaddress();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            }
	    case 39: //Spirit of the Totem
            { 
		Item item = new SpiritOfTheTotem();
		item.LootType = LootType.Blessed;
		from.AddToBackpack( item ); 
		from.CloseGump( typeof( ArtifactGump ) );
		m_Deed.Delete();
		break;
            } 
	         
         }    
      } 
   } 
}