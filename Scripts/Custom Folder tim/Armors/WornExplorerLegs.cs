using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13cb, 0x13d2 )]
	public class WornExplorerLegs : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 10; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 200; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 10; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		[Constructable]
		public WornExplorerLegs() : base( 0x13CB )
		{
            Name = "-Worn Explorer Legs-";
            Hue = 1366;
            Attributes.LowerRegCost = 15;
            Attributes.LowerManaCost = 5;
            Attributes.WeaponDamage = 5;
            Attributes.Luck = 25;
            Attributes.SpellDamage = 5;
            Attributes.RegenMana = 3;
            LootType = LootType.Cursed;
			Weight = 4.0;
		}

        public WornExplorerLegs(Serial serial)
            : base(serial)
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