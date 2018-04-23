// Created by Script Creator
// From Aries at Revenge of the Gods
using System;
using Server;
namespace Server.Items
{
 public class DuelistMagersCap : BaseHat
 {
 public override int InitMinHits{ get{ return 100;}}
 public override int InitMaxHits{ get{ return 100;}}
 [Constructable]
 public DuelistMagersCap() : base( 0x1718 )
     {
	 Hue = 326;
	LootType = LootType.Blessed;
         Name = "Mage Cap";
         Attributes.BonusHits = 4;
         Attributes.BonusInt = 4;
         Attributes.BonusMana = 4;
         Attributes.BonusStr = 4;
         Attributes.EnhancePotions = 5;
         Attributes.LowerManaCost = 5;
         Attributes.LowerRegCost = 5;
         Attributes.Luck = 40;
         Attributes.ReflectPhysical = 3;
         Attributes.RegenHits = 3;
         Attributes.RegenMana = 3;
         Attributes.SpellDamage = 3;
     }
public DuelistMagersCap( Serial serial ) : base( serial )
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
