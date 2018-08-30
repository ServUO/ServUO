using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1db9, 0x1dba )]
	public class WornExplorerCap : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 10; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 200; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 15; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }

		[Constructable]
		public WornExplorerCap() : base( 0x1DB9 )
		{
            Name = "-Worn Explorer Cap-";
            Hue = 1366;
            Attributes.LowerRegCost = 15;
            Attributes.LowerManaCost = 5;
            Attributes.AttackChance = 5;
            Attributes.Luck = 25;
            Attributes.SpellDamage = 5;
            LootType = LootType.Cursed;
			Weight = 2.0;
		}

        public WornExplorerCap(Serial serial)
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

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}