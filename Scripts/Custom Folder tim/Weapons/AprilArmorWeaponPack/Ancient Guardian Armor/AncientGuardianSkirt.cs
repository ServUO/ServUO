// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class AncientGuardianSkirt : LeatherSkirt
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public AncientGuardianSkirt()
     {
         Name = "Ancient Guardian Skirt";
         ColdBonus = 5;
         EnergyBonus = 5;
         PhysicalBonus = 5;
         PoisonBonus = 5;
	 Hue = 0x8e4;
         FireBonus = 5;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusHits = 30;
         Attributes.BonusInt = 20;
			ArmorAttributes.MageArmor = 1;
         Attributes.BonusMana = 20;
         Attributes.CastRecovery = 1;
         Attributes.BonusStr = 15;
         Attributes.CastSpeed = 1;
         Attributes.EnhancePotions = 10;
         Attributes.LowerManaCost = 12;
         Attributes.LowerRegCost = 15;
         Attributes.Luck = 40;
         Attributes.ReflectPhysical = 10;
         Attributes.RegenHits = 15;
         Attributes.RegenMana = 25;
         Attributes.SpellDamage = 19;
         IntRequirement = 50;
     }
public AncientGuardianSkirt( Serial serial ) : base( serial )
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
