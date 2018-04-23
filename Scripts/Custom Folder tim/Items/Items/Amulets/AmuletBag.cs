//Created by David aka EvilPounder
//Shard: Lords of UO
using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class AmuletBag : Bag 
   { 
      [Constructable] 
      public AmuletBag( int amount ) 
      { 
		  Weight = 5000.0;
              Movable = true; 
		  Hue = 1152; 
		  Name = "Test em out!";
      } 
	   [Constructable]
	   public AmuletBag()
	   {
		   DropItem( new AmuletOfTheAlligator() );
		   DropItem( new AmuletOfTheBoneDemon() );
		   DropItem( new AmuletOfTheBoneKnight() );	   
               DropItem( new AmuletOfTheBrownBear() );
		   DropItem( new AmuletOfTheBullFrog() );
		   DropItem( new AmuletOfTheCat() );
		   DropItem( new AmuletOfTheCentaur() );
               DropItem( new AmuletOfTheChicken() );
               DropItem( new AmuletOfTheCylops() );
               DropItem( new AmuletOfTheDarkFather() );
               DropItem( new AmuletOfTheDragon() );
		   DropItem( new AmuletOfTheDrake() );
		   DropItem( new AmuletOfTheEarthElemental() );	   
               DropItem( new AmuletOfTheEtherealWarrior() );
		   DropItem( new AmuletOfTheEvilMage() );
		   DropItem( new AmuletOfTheGazer() );
		   DropItem( new AmuletOfTheGiantPixie() );
               DropItem( new AmuletOfTheGiantSerpent() );
               DropItem( new AmuletOfTheGiantSpider() );
               DropItem( new AmuletOfTheGiantToad() );
               DropItem( new AmuletOfTheGorilla() );
		   DropItem( new AmuletOfTheHarpy() );
		   DropItem( new AmuletOfTheImp() );	   
               DropItem( new AmuletOfTheLightElemental() );
		   DropItem( new AmuletOfTheOphidianMatriarch() );
		   DropItem( new AmuletOfTheMommy() );
		   DropItem( new AmuletOfTheMongbat() );
               DropItem( new AmuletOfTheOphidianKnight() );
               DropItem( new AmuletOfTheOphidianMage() );
               DropItem( new AmuletOfTheOrc() );
               DropItem( new AmuletOfTheOrcBrut() );
		   DropItem( new AmuletOfTheOrcLord() );
		   DropItem( new AmuletOfThePanther() );	   
               DropItem( new AmuletOfThePig() );
		   DropItem( new AmuletOfThePixie() );
		   DropItem( new AmuletOfThePolorBear() );
		   DropItem( new AmuletOfTheRabbit() );
               DropItem( new AmuletOfTheRatman() );
               DropItem( new AmuletOfTheRedDragon() );
               DropItem( new AmuletOfTheRedDrake() );
               DropItem( new AmuletOfTheScorpion() );
		   DropItem( new AmuletOfTheShadowKnight() );
		   DropItem( new AmuletOfTheSheep() );	   
               DropItem( new AmuletOfTheSnake() );
		   DropItem( new AmuletOfTheSuccubus() );
		   DropItem( new AmuletOfTheTerathanAvenger() );
		   DropItem( new AmuletOfTheTerathanDrone() );
               DropItem( new AmuletOfTheTerathanMatriarch() );
               DropItem( new AmuletOfTheTerathanWarrior() );
               DropItem( new AmuletOfTheTitan() );
               DropItem( new AmuletOfTheTreeFellow() );	   
               DropItem( new AmuletOfTheWaterElemental() );
		   DropItem( new AmuletOfTheWisp() );
		   DropItem( new AmuletOfTheWolf() );
		   DropItem( new AmuletOfTheWTF() );
               DropItem( new AmuletOfTheWyvern() );
               DropItem( new AmuletOfTheZombie() );
               DropItem( new SexChangeingAmulet() );
	   }


      public AmuletBag( Serial serial ) : base( serial ) 
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
