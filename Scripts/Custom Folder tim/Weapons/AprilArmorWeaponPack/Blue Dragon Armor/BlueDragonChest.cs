// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class BlueDragonCage : DragonChest
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public BlueDragonCage()
     {
         Hue = 0x9f7;
         Name = "Blue Dragon Cage";
         ColdBonus = 50;
         EnergyBonus = 15;
         PhysicalBonus = 35;
         PoisonBonus = 45;
         FireBonus = 10;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusHits = 25;
         Attributes.BonusMana = 35;
         Attributes.BonusStam = 20;
         Attributes.WeaponSpeed = 35;
         Attributes.BonusStr = 20;
         Attributes.WeaponDamage = 15;
         Attributes.LowerRegCost = 30;
         Attributes.Luck = 20;
         Attributes.ReflectPhysical = 15;
         Attributes.RegenHits = 15;
         StrRequirement = 110;
         LootType = LootType.Blessed;
     }
public BlueDragonCage( Serial serial ) : base( serial )
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
