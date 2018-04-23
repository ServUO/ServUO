// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class FateLegs : PlateLegs
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public FateLegs()
     {
         Hue = 2052;
         Name = "Fate Legs";
         ColdBonus = 30;
         EnergyBonus = 30;
         PhysicalBonus = 30;
         PoisonBonus = 30;
         FireBonus = 30;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusHits = 35;
         Attributes.WeaponSpeed = 30;
	 Attributes.WeaponDamage = 40;
         Attributes.BonusMana = 20;
         Attributes.BonusStam = 20;
         Attributes.BonusStr = 30;
         Attributes.LowerRegCost = 25;
         Attributes.Luck = 200;
         Attributes.ReflectPhysical = 15;
         Attributes.RegenHits = 15;
         StrRequirement = 130;
         LootType = LootType.Blessed;
     }
public FateLegs( Serial serial ) : base( serial )
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
