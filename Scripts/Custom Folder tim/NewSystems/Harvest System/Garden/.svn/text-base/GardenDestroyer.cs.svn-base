//==============================================//
// Base Created by Dupre					//
// Masterfully Modified (to be a garden)--by DarkJustin from The Bluegrass Shard-- Where Tweaking is life... :-p							//
//==============================================//
using System; 
using Server; 
using Server.Items; 
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Items 
{ 
   public class GardenDestroyer : BaseAddon 
   { 
      [Constructable] 
      public GardenDestroyer( GardenFence gardenfence, GardenGround gardenground, PlayerMobile player, SecureGarden securegarden, GardenVerifier gardenverifier) 
      { 
         Name = "'Clean-Up' Sign"; 
         m_Player = player; 
         m_GardenFence = gardenfence; 
         m_GardenGround = gardenground; 
	     m_SecureGarden = securegarden;
         m_GardenVerifier = gardenverifier;
         this.ItemID = 2981;
         this.Visible = true; 
      }
      private GardenFence m_GardenFence; 
      private GardenGround m_GardenGround; 
      private PlayerMobile m_Player; 
      private SecureGarden m_SecureGarden; 
   	  private GardenVerifier m_GardenVerifier; 

            public override void OnDoubleClick( Mobile from ) 
      { 
      if (m_Player==from)
 		{
        if ( m_SecureGarden != null && m_SecureGarden.Items.Count > 0 )
 		{
         from.SendMessage( "You must remove the items from the Garden Secure before destroying your garden." );
        } 
     else 
         { 
		from.SendGump (new GardenDGump(this,from));
		}
         } 
      else 
         { 
         from.SendMessage( "You don't appear to own this garden." ); 
         } 
      } 

      public override void OnDelete() 
      { 
      m_GardenGround.Delete(); 
      m_GardenFence.Delete(); 
      m_SecureGarden.Delete();
      m_GardenVerifier.Delete();
      } 


      public GardenDestroyer( Serial serial ) : base( serial ) 
      { 
      } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 
         writer.Write( (int) 0 ); // version 
         writer.Write( m_GardenGround ); 
         writer.Write( m_GardenFence ); 
         writer.Write( m_Player ); 
         writer.Write( m_SecureGarden );
      	 writer.Write( m_GardenVerifier );
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 
         int version = reader.ReadInt(); 

         m_GardenGround = (GardenGround)reader.ReadItem(); 
         m_GardenFence = (GardenFence)reader.ReadItem(); 
         m_Player = (PlayerMobile)reader.ReadMobile(); 
         m_SecureGarden = (SecureGarden)reader.ReadItem();
      	 m_GardenVerifier = (GardenVerifier)reader.ReadItem();
      } 

   } 
}
