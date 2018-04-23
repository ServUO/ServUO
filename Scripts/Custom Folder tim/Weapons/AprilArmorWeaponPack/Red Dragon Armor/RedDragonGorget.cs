// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class RedDragonNeck : PlateGorget
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public RedDragonNeck()
     {
         Hue = 0x8F5;
         Name = "Red Dragon Neck";
         ColdBonus = 35;
         EnergyBonus = 35;
         PhysicalBonus = 35;
         PoisonBonus = 35;
         FireBonus = 50;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusHits = 20;
         Attributes.BonusMana = 15;
         Attributes.BonusStam = 20;
         Attributes.WeaponSpeed = 15;
         Attributes.BonusStr = 25;
         Attributes.WeaponDamage = 40;
         Attributes.LowerRegCost = 30;
         Attributes.Luck = 20;
         Attributes.ReflectPhysical = 15;
         Attributes.RegenHits = 10;
         StrRequirement = 130;
         LootType = LootType.Blessed;
     }
public RedDragonNeck( Serial serial ) : base( serial )
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
