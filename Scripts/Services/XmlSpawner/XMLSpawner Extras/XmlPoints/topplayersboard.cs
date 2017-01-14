using System;
using Server;
using Server.Gumps;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    [Flipable( 0x1E5E, 0x1E5F )]
	public class TopPlayersBoard : Item
   {

        public TopPlayersBoard( Serial serial ) : base( serial )
        {
        }

        [Constructable]
        public TopPlayersBoard() : base( 0x1e5e )
        {
            Movable = false;
            Name = "Top Players Board";
        }
        
        public override void OnDoubleClick( Mobile from )
        {

            from.SendGump( new XmlPoints.TopPlayersGump( XmlAttach.FindAttachment(from,typeof(XmlPoints)) as XmlPoints) );

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

