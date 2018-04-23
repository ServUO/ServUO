// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class WarriorsBarricadeFemaleChest : FemalePlateChest
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public WarriorsBarricadeFemaleChest()
     {
         Hue = 0x8dc;
         Name = "Warriors Barricade Plate Chest";
         ColdBonus = 30;
         EnergyBonus = 30;
         PhysicalBonus = 30;
         PoisonBonus = 30;
         FireBonus = 30;
         ArmorAttributes.SelfRepair = 5;
         Attributes.BonusHits = 15;
         Attributes.WeaponSpeed = 25;
	 Attributes.WeaponDamage = 35;
         Attributes.BonusMana = 10;
         Attributes.BonusStam = 10;
         Attributes.BonusStr = 20;
         Attributes.LowerRegCost = 25;
         Attributes.Luck = 20;
         Attributes.ReflectPhysical = 10;
         Attributes.RegenHits = 7;
         StrRequirement = 130;
         LootType = LootType.Blessed;
     }
public WarriorsBarricadeFemaleChest( Serial serial ) : base( serial )
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
