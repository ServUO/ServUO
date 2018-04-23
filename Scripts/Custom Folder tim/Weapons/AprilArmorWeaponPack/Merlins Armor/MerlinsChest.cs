// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class MerlinsChest : LeatherChest
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public MerlinsChest()
     {
	 Hue = 2438;
         Name = "Merlins Chest";
         ColdBonus = 30;
         EnergyBonus = 15;
         PhysicalBonus = 40;
         PoisonBonus = 15;
         FireBonus = 15;
         //ArmorAttributes.SelfRepair = 0;
         Attributes.BonusHits = 20;
         Attributes.BonusInt = 20;
         Attributes.BonusMana = 15;
         Attributes.BonusStr = 5;
         Attributes.CastSpeed = 1;
         //Attributes.CastRecovery = 1;
         Attributes.EnhancePotions = 10;
         Attributes.LowerManaCost = 25;
         Attributes.LowerRegCost = 25;
         Attributes.Luck = 50;
         Attributes.ReflectPhysical = 10;
         Attributes.RegenHits = 15;
         Attributes.RegenMana = 15;
         Attributes.SpellDamage = 13;
         IntRequirement = 50;
     }
public MerlinsChest( Serial serial ) : base( serial )
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
