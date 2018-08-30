//Created By Aremihca
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class TurtleHand : LeatherGloves
  {
public override int ArtifactRarity{ get{ return 501; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		public override int BasePhysicalResistance{ get{ return 15; } } 
      
      [Constructable]
		public TurtleHand()
		{
			Weight = 1;
          Name = "Turtle Hands";
          Hue = 2871;
      //ArmorAttributes.DurabilityBonus = 10;
      ArmorAttributes.SelfRepair = 1;
      Attributes.BonusInt = 10;
      Attributes.BonusMana = 10;
      Attributes.LowerManaCost = 10;
      Attributes.Luck = 100;
      Attributes.ReflectPhysical = 10;
      Attributes.RegenHits = 15;
      Attributes.RegenStam = 15;
      Attributes.SpellDamage = 25;
      Attributes.WeaponDamage = 25;
      LootType = LootType.Blessed;
		}

		public TurtleHand( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
