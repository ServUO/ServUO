using System;
using Server;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class TestCustomWeapon : Katana
	{

		[Constructable]
		public TestCustomWeapon()
		{
			Name = "Test weapon";

            switch(Utility.Random(3))
            {
                case 0:
                    // add a custom attack attachment with 3 random attacks
                    XmlAttach.AttachTo(this, new XmlCustomAttacks( "random", 3));
                    break;
                case 1:
                    // add a named custom attack attachment
                    XmlAttach.AttachTo(this, new XmlCustomAttacks( "tartan"));
                    break;
                case 2:
                    // add a specific list of custom attacks like this
                    XmlAttach.AttachTo(this, 
                        new XmlCustomAttacks( 
                            new XmlCustomAttacks.SpecialAttacks [] 
                            { 
                            XmlCustomAttacks.SpecialAttacks.MindDrain,
                            XmlCustomAttacks.SpecialAttacks.StamDrain
                            }
                        )
                    );
                    break;
            }
			
		}

		public TestCustomWeapon( Serial serial ) : base( serial )
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
