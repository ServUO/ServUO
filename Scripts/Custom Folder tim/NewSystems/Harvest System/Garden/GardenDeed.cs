//==============================================//
// Base Created by Dupre					//
// Masterfully Modified (to be a garden)--by DarkJustin from The Bluegrass Shard-- Where Tweaking is life... :-p							//
//==============================================//
using System; 
using Server.Network; 
using Server.Prompts; 
using Server.Items; 
using Server.Mobiles; 
using Server.Gumps;
using Server.Misc;

namespace Server.Items 
{ 
   public class GardenDeed : Item 
   {
      [Constructable] 
     public GardenDeed() : base( 3720 ) //pitchfork
      { 
         Name = "Bluegrass Garden Tool"; 
         Hue = 1164; 
         Weight = 50.0; 
         LootType = LootType.Blessed; 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
      	if (GardenCheck(from)==false)
      	{
      	from.SendMessage("You already own a splendid garden.");
      	}
      	else
      	{
      	
      	if ( IsChildOf( from.Backpack ) )
     	{              
              if ( Validate(from) == true)
             	 {
              GardenFence v = new GardenFence();
               v.Location = from.Location; 
               v.Map = from.Map; 

               GardenGround y = new GardenGround(); 
               y.Location = from.Location; 
               y.Map = from.Map; 
             	 	
               GardenVerifier gardenverifier = new GardenVerifier();
               from.AddToBackpack (gardenverifier);
               
               SecureGarden securegarden = new SecureGarden((PlayerMobile)from);
               securegarden.Location = new Point3D( from.X -1, from.Y-2, from.Z ); 
               securegarden.Map = from.Map; 

               GardenDestroyer x = new GardenDestroyer(v,y,(PlayerMobile)from, (SecureGarden) securegarden,(GardenVerifier) gardenverifier); 
               x.Location = new Point3D( from.X +3, from.Y-2, from.Z ); 
               x.Map = from.Map; 

			from.SendGump( new GardenGump( from ) );
			this.Delete(); 
              }   				
   				else
   				{
   					from.SendMessage("You cannot errect your garden in this area.");
   				}
			}
      else
      {
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
	}
      	}}

	  public bool Validate(Mobile from)
	  {
				if(from.Region.Name != "Cove" && from.Region.Name != "Britain" &&//towns
                   from.Region.Name != "Jhelom" && from.Region.Name != "Minoc" &&//towns
                   from.Region.Name != "Haven" && from.Region.Name != "Trinsic" &&//towns
                   from.Region.Name != "Vesper" && from.Region.Name != "Yew" &&//towns
                   from.Region.Name != "Wind" && from.Region.Name != "Serpent's Hold" &&//towns
                   from.Region.Name != "Skara Brae" && from.Region.Name != "Nujel'm" &&//towns
                   from.Region.Name != "Moonglow" && from.Region.Name != "Magincia" &&//towns
                   from.Region.Name != "Delucia" && from.Region.Name != "Papua" &&//towns
                   from.Region.Name != "Buccaneer's Den" && from.Region.Name != "Ocllo" &&//towns
                   from.Region.Name != "Gargoyle City" && from.Region.Name != "Mistas" &&//towns
                   from.Region.Name != "Montor" && from.Region.Name != "Alexandretta's Bowl" &&//towns
                   from.Region.Name != "Lenmir Anfinmotas" && from.Region.Name != "Reg Volon" &&//towns
                   from.Region.Name != "Bet-Lem Reg" && from.Region.Name != "Lake Shire" &&//towns
                   from.Region.Name != "Ancient Citadel" && from.Region.Name != "Luna" &&//towns
                   from.Region.Name != "Umbra" && //towns
                                      
                   from.Region.Name != "Moongates" &&
                   
                   from.Region.Name != "Covetous" && from.Region.Name != "Deceit" &&//dungeons
                   from.Region.Name != "Despise" && from.Region.Name != "Destard" &&//dungeons
                   from.Region.Name != "Hythloth" && from.Region.Name != "Shame" &&//dungeons
                   from.Region.Name != "Wrong" && from.Region.Name != "Terathan Keep" &&//dungeons
                   from.Region.Name != "Fire" && from.Region.Name != "Ice" &&//dungeons
	  			   from.Region.Name != "Rock Dungeon" && from.Region.Name != "Spider Cave" &&//dungeons
	  			   from.Region.Name != "Spectre Dungeon" && from.Region.Name != "Blood Dungeon" &&//dungeons
	  			   from.Region.Name != "Wisp Dungeon" && from.Region.Name != "Ankh Dungeon" &&//dungeons
	  			   from.Region.Name != "Exodus Dungeon" && from.Region.Name != "Sorcerer's Dungeon" &&//dungeons
	  			   from.Region.Name != "Ancient Lair" && from.Region.Name != "Doom" &&//dungeons
	  			   
	  			   from.Region.Name != "Britain Graveyard" && from.Region.Name != "Wrong Entrance" &&
	  			   from.Region.Name != "Covetous Entrance" && from.Region.Name != "Despise Entrance" &&
	  			   from.Region.Name != "Despise Passage" && from.Region.Name != "Jhelom Islands" &&
	  			   from.Region.Name != "Haven Island" && from.Region.Name != "Crystal Cave Entrance" &&
	  			   from.Region.Name != "Protected Island" && from.Region.Name != "Jail")
	  			   {
	  			   	return true;
	  			   }
	  			   else
	  			   {
	  			   	return false;
	  			   }
	  }
	  	  
      public bool GardenCheck(Mobile from)
      {
      	int count=0;
      	foreach(Item verifier in from.Backpack.Items)
					{ 
						if(verifier is GardenVerifier)
						{
						count = count+1;
						}
						else
						{
						count = count+0;
						}
					}
      if (count > 3) //change this if you want players to own more than 1,2,3 etc.
      {
      	return false;
      }
      else
      {
      	return true;
      }
      return GardenCheck(from);
      }
      
      public GardenDeed( Serial serial ) : base( serial )
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

         int version = reader.ReadInt(); 
      } 
   } 
}
