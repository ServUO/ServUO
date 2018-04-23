// Created by Neptune
using System;
using Server;
namespace Server.Items
{
 public class TergusChest : StuddedChest
 {
 public override int InitMinHits{ get{ return 150;}}
 public override int InitMaxHits{ get{ return 150;}}
 [Constructable]
 public TergusChest()
     {
         Name = "Chest of Tergus";
         ColdBonus = 20;
         EnergyBonus = 20;
         PhysicalBonus = 20;
         PoisonBonus = 20;
         FireBonus = 20;
         ArmorAttributes.SelfRepair = 10;
         Attributes.BonusDex = 15;
         Attributes.BonusHits = 10;
         Attributes.BonusInt = 15;
         Attributes.BonusMana = 10;
         Attributes.BonusStam = 10;
         Attributes.BonusStr = 15;
         Attributes.CastRecovery = 1;
         Attributes.CastSpeed = 1;
         Attributes.LowerManaCost = 10;
         Attributes.LowerRegCost = 20;
         Attributes.ReflectPhysical = 10;
         Attributes.SpellDamage = 15;
     }
public TergusChest( Serial serial ) : base( serial )
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
