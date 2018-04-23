using System;
using Server.Items;

namespace Server.Items
{
    public class LeggingsOfEnlightenment : LeafLegs, ITokunoDyable
	{

		public override int BaseFireResistance{ get{ return 9; } }
		public override int BasePoisonResistance{ get{ return 12; } }
		public override int BasePhysicalResistance{ get{ return 11; } }
		public override int BaseEnergyResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 5; } }
public override int ArtifactRarity{ get{ return 13; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

		[Constructable]
		public LeggingsOfEnlightenment()
		{
			Name = "Leggings Of Enlightenment";
			Hue = 0x487;

			SkillBonuses.SetValues( 0, SkillName.EvalInt, 10.0 );

			Attributes.BonusInt = 8;
			Attributes.SpellDamage = 10;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 5;
		}

		public LeggingsOfEnlightenment( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}