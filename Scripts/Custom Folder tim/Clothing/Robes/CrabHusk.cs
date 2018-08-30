using System;
using Server;

namespace Server.Items
{
    public class CrabHusk : BaseQuiver
	{
       

		[Constructable]
        public CrabHusk() : base(0x2FB7)
		{
			Hue = 1192;
            Name = "Crab's Husk {Grove Artifact}";
            //SetTitle = "[Heavenly Artifact]"; //*this is new edit, wanted to have it so if someone renames thier weapon the title will still be there
			Attributes.BonusStr = 5;
			Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
			Attributes.Luck = 100;
            Attributes.WeaponDamage = 15;
            WeightReduction = 25;   
		}

		public CrabHusk( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (Weight == 4.0)
                Weight = 5.0;
		}
	}
}