using System;
 using Server;
 using Server.Items;
 using Server.Mobiles;

namespace Server.Multis
{
     public class PrisonerCamp : BaseCamp
     {
         private Mobile m_Prisoner;
         private BaseDoor m_Gate;

         [Constructable]
         public PrisonerCamp() : base( 0x1D4C )
         {
         }

         public override void AddComponents()
         {
             IronGate gate = new IronGate( DoorFacing.EastCCW );
             m_Gate = gate;

             gate.KeyValue = Key.RandomValue();
             gate.Locked = true;

             AddItem( gate, -2, 1, 0 );
			 AddCampChests();

             switch (Utility.Random(4))
             {
                 case 0: 
                     {
                         AddMobile(new Orc(), 15, 0, -2, 0);
                         AddMobile(new OrcishMage(), 15, 0, 1, 0);
                         AddMobile(new OrcishLord(), 15, 0, -2, 0);
                         AddMobile(new OrcCaptain(), 15, 0, 1, 0);
                         AddMobile(new Orc(), 15, 0, -1, 0);
                         AddMobile(new OrcChopper(), 15, 0, -2, 0);
                     }break;
                 
                 case 1:
                     {
                         AddMobile(new Ratman(), 15, 0, -2, 0);
                         AddMobile(new Ratman(), 15, 0, 1, 0);
                         AddMobile(new RatmanMage(), 15, 0, -2, 0);
                         AddMobile(new Ratman(), 15, 0, 1, 0);
                         AddMobile(new RatmanArcher(), 15, 0, -1, 0);
                         AddMobile(new Ratman(), 15, 0, -2, 0);
                     } break;

                 case 2:
                     {
                         AddMobile(new Lizardman(), 15, 0, -2, 0);
                         AddMobile(new Lizardman(), 15, 0, 1, 0);
                         AddMobile(new Lizardman(), 15, 0, -2, 0);
                         AddMobile(new Lizardman(), 15, 0, 1, 0);
                         AddMobile(new Lizardman(), 15, 0, -1, 0);
                         AddMobile(new Lizardman(), 15, 0, -2, 0);
                     } break;

                      case 3:
                     {
                         AddMobile(new Brigand(), 15, 0, -2, 0);
                         AddMobile(new Brigand(), 15, 0, 1, 0);
                         AddMobile(new Brigand(), 15, 0, -2, 0);
                         AddMobile(new Brigand(), 15, 0, 1, 0);
                         AddMobile(new Brigand(), 15, 0, -1, 0);
                         AddMobile(new Brigand(), 15, 0, -2, 0);
                     } break;
             }
             
             switch ( Utility.Random( 2 ) )
             {
                 case 0: m_Prisoner = new Noble(); break;
                 case 1: m_Prisoner = new SeekerOfAdventure(); break;
             }

             m_Prisoner.YellHue = Utility.RandomList( 0x57, 0x67, 0x77, 0x87, 0x117 );

             AddMobile( m_Prisoner, 2, -2, 0, 0 );
         }

         public override void OnEnter( Mobile m )
         {
             base.OnEnter( m );

             if ( m.Player && m_Prisoner != null && m_Gate != null && m_Gate.Locked )
             {
                 int number;

                 switch ( Utility.Random( 10 ) )
                 {
                     default:
                     case 0: number =  502264; break; // Help a poor prisoner!
                     case 1: number =  502266; break; // Aaah! Help me!
                     case 2: number = 1046000; break; // Help! These savages wish to end my life!
                     case 3: number = 1046003; break; // Quickly! Kill them for me! HELP!!
					 case 4: number = 502261;  break; // HELP!
					 case 5: number = 502262; break; // Help me!
					 case 6: number = 502263; break; // Canst thou aid me?!
                 	 case 7: number = 502265; break; // Help! Please!
					 case 8: number = 502267; break; // Go and get some help!
					 case 9: number = 502268; break; // Quickly, I beg thee! Unlock my chains! If thou dost look at me close thou canst see them.	 
					 }

                 m_Prisoner.Yell( number );
             }
         }

         public PrisonerCamp( Serial serial ) : base( serial )
         {
         }

         public override void Serialize( GenericWriter writer )
         {
             base.Serialize( writer );

             writer.Write( (int) 0 ); // version

             writer.Write( m_Prisoner );
             writer.Write( m_Gate );
         }

         public override void Deserialize( GenericReader reader )
         {
             base.Deserialize( reader );

             int version = reader.ReadInt();

             switch ( version )
             {
                 case 0:
                 {
                     m_Prisoner = reader.ReadMobile();
                     m_Gate = reader.ReadItem() as BaseDoor;
                     break;
                 }
             }
         }
     }
}