using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class ParoxysmusAltar : PeerlessAltar
    {
        public static Dictionary<Mobile, Timer> ProtectionTable = new Dictionary<Mobile, Timer>();

        public override int KeyCount{ get{ return 3; } }
		public override MasterKey MasterKey{ get{ return new ParoxysmusKey(); } }
		
		public override Type[] Keys{ get{ return new Type[]
		{
			typeof( CoagulatedLegs ), typeof( PartiallyDigestedTorso ), 
			typeof( GelatanousSkull ), typeof( SpleenOfThePutrefier )
		}; }}
		
		public override BasePeerless Boss{ get{ return new ChiefParoxysmus(); } }		
			
		[Constructable]
		public ParoxysmusAltar() : base( 0x207A )
		{			
			Hue = 0x465;
			
			BossLocation = new Point3D( 6517, 357, 0 );
			TeleportDest = new Point3D( 6519, 381, 0 );
			ExitDest = new Point3D( 5623, 3038, 15 );			
		}

        public override Rectangle2D[] BossBounds
        {
            get { return m_Bounds; }
        }

        private Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(6501, 351, 35, 48),
        };

        public static void AddProtection(Mobile m)
        {
            if (ProtectionTable != null && !ProtectionTable.ContainsKey(m))
            {
                ProtectionTable[m] = Timer.DelayCall(TimeSpan.FromMinutes(5), () => Damage(m));
            }
        }

        public static bool IsUnderEffects(Mobile m)
        {
            return ProtectionTable != null && ProtectionTable.ContainsKey(m);
        }

        public static void Damage(Mobile m)
        {
            Timer t;

            if (ProtectionTable.TryGetValue(m, out t))
            {
                if (t != null)
                {
                    t.Stop();
                }

                ProtectionTable.Remove(m);
            }
        }

        public ParoxysmusAltar( Serial serial ) : base( serial )
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
