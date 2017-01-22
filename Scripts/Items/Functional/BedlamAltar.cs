using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class BedlamAltar : PeerlessAltar
	{		
		public override int KeyCount{ get{ return 3; } }
		public override MasterKey MasterKey{ get{ return new BedlamKey(); } }
		
		public override Type[] Keys{ get{ return new Type[]
		{
			typeof( LibrariansKey )
		}; }}
		
		public override BasePeerless Boss{ get{ return new MonstrousInterredGrizzle(); } }		
	
		[Constructable]
		public BedlamAltar() : base( 0x207E )
		{			
			BossLocation = new Point3D( 106, 1615, 90 );
			TeleportDest = new Point3D( 101, 1623, 50 );
			ExitDest = new Point3D( 2068, 1372, -75 );
		}

        public override Rectangle2D[] BossBounds
        {
            get { return m_Bounds; }
        }

        private Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(99, 1609, 14, 18),
        };
	
		public BedlamAltar( Serial serial ) : base( serial )
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