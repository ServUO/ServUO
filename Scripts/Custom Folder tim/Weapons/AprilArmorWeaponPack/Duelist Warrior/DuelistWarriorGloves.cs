// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class DuelistWarriorGloves : PlateGloves
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public DuelistWarriorGloves()
     {
         Hue = 2881;
         Name = "Duelist Warrior Gloves";
         ColdBonus = 10;
         EnergyBonus = 10;
         PhysicalBonus = 10;
         PoisonBonus = 10;
         FireBonus = 10;
         ArmorAttributes.SelfRepair = 15;
         Attributes.BonusHits = 20;
         Attributes.BonusMana = 15;
         Attributes.BonusStam = 15;
	 Attributes.WeaponDamage = 25;
	 Attributes.WeaponSpeed = 20;
         Attributes.BonusStr = 15;
         Attributes.LowerRegCost = 15;
         Attributes.Luck = 20;
         Attributes.ReflectPhysical = 15;
         Attributes.RegenHits = 15;
         StrRequirement = 100;
         LootType = LootType.Blessed;
     }
public DuelistWarriorGloves( Serial serial ) : base( serial )
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
