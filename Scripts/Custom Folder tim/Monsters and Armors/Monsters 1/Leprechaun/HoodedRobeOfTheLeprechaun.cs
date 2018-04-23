
using System; 
using Server.Items; 

namespace Server.Items 
{ 
   public class HoodedRobeOfTheLeprechaun : BaseArmor
   { 
      public override int PhysicalResistance{ get{ return 2; } } 
                public override int FireResistance{ get{ return 2; } } 
                public override int ColdResistance{ get{ return 2; } } 
                public override int PoisonResistance{ get{ return 2; } } 
                public override int EnergyResistance{ get{ return 2; } } 

      public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

                public override int ArtifactRarity{ get{ return 10; } } 

      [Constructable] 
      public HoodedRobeOfTheLeprechaun() : base( 0x2684 ) 
      { 
         Weight = 1; 
                        Hue = 69; 
                        Name = "Hooded RobeOf The Leprechaun"; 
                        IntRequirement = 50;
                        Attributes.Luck = 1000;  
                        Attributes.RegenMana = 1; 
                        Attributes.SpellDamage = 50;
   

      } 

      public HoodedRobeOfTheLeprechaun( Serial serial ) : base( serial ) 
      { 
      } 
       
      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 
         writer.Write( (int) 0 ); 
      } 
        
      public override void Deserialize(GenericReader reader) 
      { 
         base.Deserialize( reader ); 
         int version = reader.ReadInt(); 
      } 
   } 
}  
 
