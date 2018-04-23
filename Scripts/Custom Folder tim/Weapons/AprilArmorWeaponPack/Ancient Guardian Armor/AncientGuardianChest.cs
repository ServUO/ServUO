// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class AncientGuardianChest : LeatherChest
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public AncientGuardianChest()
     {
	 Hue = 0x8e4;
         Name = "Ancient Guardian Chest";
         ColdBonus = 20;
         EnergyBonus = 25;
         PhysicalBonus = 20;
         PoisonBonus = 25;
         FireBonus = 15;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusHits = 30;
			ArmorAttributes.MageArmor = 1;
         Attributes.BonusInt = 20;
         Attributes.BonusMana = 25;
         Attributes.BonusStr = 15;
         Attributes.CastSpeed = 1;
         Attributes.CastRecovery = 1;
         Attributes.EnhancePotions = 20;
         Attributes.LowerManaCost = 10;
         Attributes.LowerRegCost = 15;
         Attributes.Luck = 85;
         Attributes.ReflectPhysical = 5;
         Attributes.RegenHits = 15;
         Attributes.RegenMana = 25;
         Attributes.SpellDamage = 19;
         IntRequirement = 50;
     }
public AncientGuardianChest( Serial serial ) : base( serial )
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
