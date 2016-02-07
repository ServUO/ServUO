using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    [FlipableAttribute( 0x13E4, 0x13E3 )]
    public class HammerOfRecovery : Item
    {
        private int m_UsesRemaining;                // if set to less than zero, becomes unlimited uses
        
        [CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

        [Constructable]
        public HammerOfRecovery() : this(50)
        {
        }
        
        [Constructable]
        public HammerOfRecovery(int nuses) : base(0x13E4)
        {
            Name = "A Hammer of Augmentation Recovery";
            Hue = 5;
            UsesRemaining = nuses;
        }

        public HammerOfRecovery( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

            if(m_UsesRemaining >= 0)
                list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}
		
		public override void OnDoubleClick( Mobile from)
        {
            if(UsesRemaining == 0)
            {
                from.SendMessage("This hammer is now useless");
                return;
            }
            if ( IsChildOf( from.Backpack ) || Parent == from )
			{
                from.Target = new XmlSockets.RecoverAugmentationFromTarget();
                if(UsesRemaining > 0)
                    UsesRemaining--;
            } 
            else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			writer.Write(m_UsesRemaining);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_UsesRemaining = reader.ReadInt();
		}
    }
}
