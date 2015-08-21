using System;
using Server;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class TestCustomShield : Buckler
	{

		[Constructable]
		public TestCustomShield()
		{
			Name = "Test shield";

            switch(Utility.Random(3))
            {
                case 0:
                    // add a custom defense attachment with 3 random defenses
                    XmlAttach.AttachTo(this, new XmlCustomDefenses( "random", 3));
                    break;
                case 1:
                    // add a named custom defense configuration
                    XmlAttach.AttachTo(this, new XmlCustomDefenses( "brogan"));
                    break;
                case 2:
                    // add a specific list of custom defenses like this
                    XmlAttach.AttachTo(this, 
                        new XmlCustomDefenses(
                            new XmlCustomDefenses.SpecialDefenses []
                            { 
                            XmlCustomDefenses.SpecialDefenses.MindDrain,
                            XmlCustomDefenses.SpecialDefenses.SpikeShield
                            }
                        )
                    );
                    break;
            }
			
		}

		public TestCustomShield( Serial serial ) : base( serial )
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
