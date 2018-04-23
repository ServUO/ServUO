//Made By Silver
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class ElfHat : WizardsHat
  {


		public override int BaseColdResistance{ get{ return 5; } } 
      
      [Constructable]
		public ElfHat()
		{
          Name = "Elf Hat";
          Hue = 62;
      Attributes.BonusInt = 5;
      Attributes.BonusMana = 8;
      Attributes.CastRecovery = 1;
      Attributes.Luck = 25;
      Attributes.RegenMana = 2;
      Attributes.BonusMana = 8;
		}

		public ElfHat( Serial serial ) : base( serial )
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
