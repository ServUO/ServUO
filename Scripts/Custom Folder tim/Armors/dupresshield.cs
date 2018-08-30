using System;
using Server;

namespace Server.Items
{
    public class Dupresshield : BaseShield
	{
		public override int LabelNumber{ get{ return 1075196; } }

		public override int BasePhysicalResistance{ get{ return 1; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
		public override int BaseEnergyResistance{ get{ return 1; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
        public Dupresshield() : base(0x2B01)
		{
            Name = "Dupre's Shield";
            Hue = 0x226;

            SkillBonuses.SetValues(0, SkillName.Parry, 5.0);
			Attributes.SpellChanneling = 1;
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 1;
			
		}

        public Dupresshield(Serial serial) : base(serial)
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
