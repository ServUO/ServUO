using System;
using Server;

namespace Server.Items
{
	public class JadeArmband : GoldBracelet
	{
        public override int LabelNumber { get { return 1112407; } } //Jade Armband [Replica]

		[Constructable]
		public JadeArmband()
		{
			Hue = 2126;

            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 5;
            Resistances.Poison = 20;
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
