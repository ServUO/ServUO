// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class NorsePlateGorget : PlateGorget
 {
 public override int InitMinHits{ get{ return 175;}}
 public override int InitMaxHits{ get{ return 175;}}
 [Constructable]
 public NorsePlateGorget()
     {
         Hue = 2554;
         Name = "Norse Plate Gorget";
         ColdBonus = 20;
         EnergyBonus = 20;
         PhysicalBonus = 20;
         PoisonBonus = 20;
         FireBonus = 20;
         ArmorAttributes.SelfRepair = 5;
         Attributes.BonusDex = 5;
         Attributes.BonusHits = 10;
         Attributes.BonusInt = 10;
         Attributes.BonusMana = 20;
         Attributes.BonusStam = 10;
         Attributes.BonusStr = 10;
         Attributes.LowerManaCost = 20;
         Attributes.LowerRegCost = 25;
         Attributes.ReflectPhysical = 5;
         Attributes.RegenHits = 10;
         Attributes.RegenMana = 20;
         Attributes.RegenStam = 5;
         LootType = LootType.Blessed;
     }
public NorsePlateGorget( Serial serial ) : base( serial )
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
