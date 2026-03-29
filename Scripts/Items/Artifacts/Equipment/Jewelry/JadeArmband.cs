using System;
using Server;

namespace Server.Items
{
	public class JadeArmband : GoldBracelet
	{
        public override int LabelNumber { get { return 1112407; } } //Jade Armband [Replica]
		public override bool IsArtifact { get { return true; } }
		
		public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }
		
		[Constructable]
		public JadeArmband()
		{
			Hue = 2126;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponSpeed = 5;
            Attributes.LowerRegCost = 15;
            Resistances.Poison = 20;
            SkillBonuses.SetValues(0, SkillName.Chivalry, 15.0);
		}		

		public JadeArmband( Serial serial ) : base( serial )
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
