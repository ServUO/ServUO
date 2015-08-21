using System;
using Server;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class TestSocketedWeapon : Katana
	{

		[Constructable]
		public TestSocketedWeapon()
		{
			Name = "Test socketed weapon";

            switch(Utility.Random(4))
            {
                case 0:
                    // make the weapon socketable up to 4 sockets using the default blacksmithing requirements
                    // and add 2 sockets to start
                    XmlAttach.AttachTo(this, new XmlSocketable(4));
                    XmlAttach.AttachTo(this, new XmlSockets(2));
                    break;
                case 1:
                    // make the weapon socketable up to 4 sockets, and set specific socketing requirements
                    // minimum of 100 skill in Tinkering required to socket it, and it uses 50 Agapipe ingots
                    XmlAttach.AttachTo(this, new XmlSocketable(4, SkillName.Tinkering, 100.0, typeof(AgapiteIngot), 50));
                    break;
                case 2:
                    // give it 2 sockets and dont allow it to be further socketed
                    XmlAttach.AttachTo(this, new XmlSocketable(0));
                    XmlAttach.AttachTo(this, new XmlSockets(2));
                    break;
                case 3:
                    // give it 2 sockets, fill one of them with an augment, nd dont allow it to be further socketed
                    
                    // create 2 sockets
                    XmlSockets s = new XmlSockets(2);
                    // fill the sockets (starting at 0) with an ancient diamond augment (which only takes up 1 slot)
                    // the augment call has this form: public static bool Augment( Mobile from, object parent, XmlSockets sock, int socketnum, IXmlSocketAugmentation a)
                    // Note that the new augment will be automatically deleted after augmenting
                    XmlSockets.Augment(null, this, s, 0, new AncientDiamond());
                    // and put the sockets onto the katana
                    XmlAttach.AttachTo(this, s);
                    // and dont allow it to be further socketed
                    XmlAttach.AttachTo(this, new XmlSocketable(0));
                    break;
            }
			
		}

		public TestSocketedWeapon( Serial serial ) : base( serial )
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
