using Server;
using System;

namespace Server.Items
{
    public class ValkyrieArmor : LeatherBustierArms
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1149957; } }

		public override int BasePhysicalResistance{ get{ return 11; } }
		public override int BaseFireResistance{ get{ return 14; } }
		public override int BaseColdResistance{ get{ return 8; } }
		public override int BasePoisonResistance{ get{ return 11; } }
		public override int BaseEnergyResistance{ get{ return 8; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public ValkyrieArmor() 
		{
			Hue = 2112;

			Attributes.BonusStr = 5;
			Attributes.BonusDex = 5;
			Attributes.BonusInt = 5;

			Attributes.BonusStam = 5;
			Attributes.RegenStam = 3;
			Attributes.LowerManaCost = 10;
		}
				
		public ValkyrieArmor(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{	
			base.Serialize(writer);
            writer.Write((int)0);	
		}
	
		public override void Deserialize(GenericReader reader)
		{
		 	base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}