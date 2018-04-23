// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class MerlinsGloves : LeatherGloves
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public MerlinsGloves()
     {
	 Hue = 2438;
         Name = "Merlins Gloves";
         ColdBonus = 20;
         EnergyBonus = 15;
         PhysicalBonus = 15;
         PoisonBonus = 15;
         FireBonus = 35;
         //ArmorAttributes.SelfRepair = 0;
         Attributes.BonusHits = 20;
         Attributes.BonusInt = 20;
         Attributes.BonusMana = 15;
         Attributes.BonusStr = 5;
         Attributes.CastSpeed = 1;
         //Attributes.CastRecovery = 1;
         Attributes.EnhancePotions = 10;
         Attributes.LowerManaCost = 25;
         Attributes.LowerRegCost = 25;
         Attributes.Luck = 50;
         Attributes.ReflectPhysical = 10;
         Attributes.RegenHits = 15;
         Attributes.RegenMana = 15;
         Attributes.SpellDamage = 13;
         IntRequirement = 50;
     }
public MerlinsGloves( Serial serial ) : base( serial )
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
