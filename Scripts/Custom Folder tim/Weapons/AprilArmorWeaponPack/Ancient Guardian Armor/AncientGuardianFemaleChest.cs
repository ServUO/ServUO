// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class AncientGuardianFemaleChest : FemaleLeatherChest
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public AncientGuardianFemaleChest()
     {
         Name = "Ancient Guardian Female Chest";
         ColdBonus = 5;
         EnergyBonus = 5;
         PhysicalBonus = 5;
         PoisonBonus = 5;
         FireBonus = 5;
	 Hue = 0x8e4;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusHits = 35;
         Attributes.BonusInt = 20;
         Attributes.BonusMana = 20;
         Attributes.BonusStr = 10;
         Attributes.CastRecovery = 1;
         Attributes.CastSpeed = 1;
         Attributes.EnhancePotions = 10;
         Attributes.LowerManaCost = 10;
         Attributes.LowerRegCost = 20;
         Attributes.Luck = 40;
         Attributes.ReflectPhysical = 10;
         Attributes.RegenHits = 15;
         Attributes.RegenMana = 25;
         Attributes.SpellDamage = 19;
         IntRequirement = 50;
			ArmorAttributes.MageArmor = 1;
     }
public AncientGuardianFemaleChest( Serial serial ) : base( serial )
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
