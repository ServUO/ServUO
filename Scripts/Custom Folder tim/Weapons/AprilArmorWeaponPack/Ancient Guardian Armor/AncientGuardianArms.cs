// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class AncientGuardianArms : LeatherArms
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public AncientGuardianArms()
     {
         Name = "Ancient Guardian Arms";
         ColdBonus = 5;
	 Hue = 0x8e4;
         EnergyBonus = 15;
         PhysicalBonus = 35;
         PoisonBonus = 25;
         FireBonus = 15;
         ArmorAttributes.SelfRepair = 5;
         Attributes.BonusHits = 28;
         Attributes.CastRecovery = 1;
			ArmorAttributes.MageArmor = 1;
         Attributes.BonusInt = 20;
         Attributes.BonusMana = 20;
         Attributes.BonusStr = 15;
         Attributes.CastSpeed = 1;
         Attributes.EnhancePotions = 20;
         Attributes.LowerManaCost = 15;
         Attributes.LowerRegCost = 15;
         Attributes.Luck = 400;
         Attributes.ReflectPhysical = 10;
         Attributes.RegenHits = 10;
         Attributes.RegenMana = 25;
         Attributes.SpellDamage = 19;
         IntRequirement = 50;
     }
public AncientGuardianArms( Serial serial ) : base( serial )
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
